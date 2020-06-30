using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplePasswordsEncrypter
{
    public class AppConst
    {
        // History
        // 0.0-0 2018/02/06 着想。調査のためプロトタイプ作成着手。
        // 0.0-1 2018/02/23 リリース版開発着手
        // 1.0#0.0-0 2020/06/28 ソース公開のためのコメント整備等準備

        public const string COPYRIGHT = "(C) 2018 Hakutaku Software Co.,Ltd.";
        public const string VERSION_STR = VERSION + "\n\n" + COPYRIGHT;

        public const string VERSION = "Version:1.0#0.0-0\nRelease:2020-06-28";

        [Flags]
        public enum MPE_Flag
        {
            none = 0x00,
            isTrim = 0x01,
            isRemoveSpace = 0x02,
            isIgnoreCase = 0x04,
            isIgnoreZenHan = 0x08,
            isIgnoreHiraKata = 0x10,
            isNoCompress = 0x80,
        }

        public const int MPE_HEADER_SIZE = 3 + 3 + 3 + 4 + 4 + 8 + 4;
        public const string MPE_HEADER_CONST = "MPE";
        public const string MPE_FORMAT_VERSION = "000";
        public const string MPE_ENCODER_VERSION = "000";

        /*
        file format

        const					3	MPE
        archive format version	3	000
        encoder version			3	000
        question header len		4
        question header CRC32	4
        encryptedData len		8
        encryptedData CRC32		4
        question header			n
        encryptedData(*)		n


        question header
            flag					1
                isTrim				0x01
                isRemoveSpace		0x02
                isIgnoreCase		0x04
                isIgnoreZenHan		0x08
                isIgnoreHiraKata	0x10
                isNoCompress		0x80
            nQuestions				4
            nRequiredPasswords		4
            nPasswordCombinations	4
            encryptedDataKeys(*)	nPasswordCombinations * 48
            Question				n

        Question
            hint string len				4
            hint string					n
            nPasswords					4
            encryptedQuestionKeys(*)	nPasswords * 48
        */
    }
}
