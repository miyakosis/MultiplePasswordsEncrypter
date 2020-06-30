Multiple Passwords Encrypter
====
複数パスワード暗号化ソフトウェア

## Description
世の中の多くのシステムでパスワードやパスフレーズによる認証が使われています。  
しかしパスワードは文字列長や文字種に制限があったり、制限が無いとしても記憶しにくく、それに伴って複数システムで同じパスワードの使いまわしなどが問題となります。  
パスワードに似たものとして、パスワードを忘れた際のリセット再発行などの際に「秘密の質問」が使われることがあります。  
「あなたの好きなフルーツは？」とか「ペットの名前は？」といった質問に回答を登録しておく方式です。  
しかしこれも定型の質問しかないこともありますし、また「好きなフルーツ」に回答するにしても「林檎」も「梨」も同じくらい好きという場合や、
「リンゴ」で登録したのか「りんご」なのか、はたまた「apple」なのか忘れてしまうこともあります。  
パスフレーズでも「Imagine all the people」という一節にしようと思っても、最初を大文字で始めたのか全部小文字か忘れてしまったり、単語間に間違えてスペースを2つ入れるだけで認証に失敗します。

まとめると、パスワード認証には  
- 一つのフレーズしか設定できない
- 表記ゆれを許容しない

といった問題があります。逆に考えれば、

- 複数のフレーズを設定できる
- 表記ゆれを許容する
パスワード認証システムがよいと考え、その実装の例としてこの認証方式を搭載した暗号化ソフトウェアを実装しました。


## User Interface
ここでは3つの質問を設定してファイルを暗号化する例と、そのファイルを復号化する際のUIを例として挙げました。  
最初の質問では「りんご」「リンゴ」「梨」などの回答を許容します。  
ただこのような表記ゆれを許容するということは、その分パスワードの強度が下がることを意味します。  
そこで複数の質問を設定可能とし、ここでは「3つの質問のうち2つ以上正答すると復号できる」という設定にすることで機密性を上げています。
(3つの質問に全て正答しないと復号できないという設定も可能です)


## Download
bin フォルダ配下を参照してください。  
このソフトウェアは無保証です。バックアップを保存せずに重要性の高いファイルを暗号化するのはリスクがあることをご承知ください。  
(不具合報告は歓迎いたします)

## Licence
This software is distributed under the MIT License.  
このソフトウェアは MIT License のもとで配布いたします。



## 暗号化の流れ
最初に暗号化対象のファイルを Zip アーカイブ化して 1 ファイルにします(以降data)。  
256bit乱数 で作成した 暗号化キー(dataKey)を用いて、data を AES-256 で暗号化します。  
```
    data + dataKey => encryptedData
```

次に質問ごとに暗号化キー(questionKey)を 256bit乱数 で作成します。  
上記例のように質問が3つある場合は、questionKey0 ～ questionKey2 を生成します。  
次に質問の数と正答の数の組み合わせで dataKey を暗号化するキー(combinedQuestionKey)を作成します。  
上記例のように3つの質問のうち2つを正答する必要がある場合は、3C2 で 3通りの組み合わせとなります。  
```
combinedQuestionKey0_1 = questionKey0 + questionKey1
combinedQuestionKey0_2 = questionKey0 + questionKey2
combinedQuestionKey1_2 = questionKey1 + questionKey2
(ここでの+は単に結合を意味します)
```

もし3つの質問のうち3つを正答する必要がある場合は、dataKey を暗号化するキーは以下の一つだけとなります。  
```
combinedQuestionKey0_1_2 = questionKey0 + questionKey1 + questionKey2
```

combinedQuestionKey を用いて、dataKey を暗号化します。  
```
dataKey + combinedQuestionKey0_1 => encryptedDataKey0_1
dataKey + combinedQuestionKey0_2 => encryptedDataKey0_2
dataKey + combinedQuestionKey1_2 => encryptedDataKey1_2
```

最後に、ユーザーが入力したパスワードで questionKey を暗号化します。  
例のように、最初の質問に二つのパスワードが設定されている場合、それぞれのパスワードで暗号化します。  
```
questionKey0 + "りんご" -> encryptedQuestionKey0-0
questionKey0 + "なし" -> encryptedQuestionKey0-1
```

「パスワードに含まれる空白を無視する」「大文字と小文字を区別しない」などのオプションが設定されている場合は、
パスワード文字列をそのように正規化して暗号化します。(復号化の際も入力パスワードを同様に正規化して使用する)

一つのファイルに encryptedData、encryptedDataKey0_1 ...、encryptedQuestionKey0-0 ... を記録して暗号化ファイルとします。



## 復号化の流れ
質問0ではユーザーが入力したパスワードで、encryptedQuestionKey0-0、encryptedQuestionKey0-1、…と順に復号を試みていきます。  
復号できればその質問の questionKey0 が取り出せます。  
質問0 と 質問2 が正答できれば、 questionKey0 + questionKey2 = combinedQuestionKey0_2 が取得できますので、encryptedDataKey0_2 から dataKey が取得できます。  
```
encryptedDataKey0_2 - combinedQuestionKey0_2 => dataKey
```
dataKey を用いて data が復号できます。  
```
encryptedData - dataKey => data
```



## 暗号ファイルフォーマット

file format
constant				3	定数 "MPE"
archive format version	3	暗号ファイルフォーマットのバージョン。"000"
encoder version			3	エンコーダーのバージョン。"000"
Question header len		4	Question header 構造体のサイズ
Question header CRC32	4	Question header 構造体の CRC
encryptedData len		8	暗号化データのサイズ
encryptedData CRC32		4	暗号化データの CRC
Question header			(1)	Question header 構造体
encryptedData(*)		n	暗号化データ


Question header
	flag					1
		isTrim				0x01
		isRemoveSpace		0x02
		isIgnoreCase		0x04
		isIgnoreZenHan		0x08
		isIgnoreHiraKata	0x10
		isNoCompress		0x80
	nQuestions				4
	nRequiredPasswords		4
	nDataKeyCombinations	4	(nQuestions と nRequiredPasswords から計算もできるが、一応保持)
	encryptedDataKeys(*)	nDataKeyCombinations * 48
	Question				(nQuestions)	nQuestions * Question 構造体

Question
	hint string len				4
	hint string					n
	nPasswords					4
	encryptedQuestionKeys(*)	nPasswords * 48




