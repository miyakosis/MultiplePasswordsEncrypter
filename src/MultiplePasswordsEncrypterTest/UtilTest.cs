using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiplePasswordsEncrypter;

namespace MultiplePasswordsEncrypterTest
{
    [TestClass]
    public class UtilTest
    {
        [TestMethod]
        public void toRemoveInnerSpaces()
        {
            Assert.AreEqual("", Util.toRemoveInnerSpaces(" "));
            Assert.AreEqual("ab", Util.toRemoveInnerSpaces(" a b "));
            Assert.AreEqual("abcd", Util.toRemoveInnerSpaces("a\tb\rc\nd"));
            Assert.AreEqual("あいう", Util.toRemoveInnerSpaces("　あ　い　う　"));
        }

        [TestMethod]
        public void toUpperCase()
        {
            Assert.AreEqual("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJKLMNOPQRSTUVWXYZ", Util.toUpperCase("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            Assert.AreEqual("ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ", Util.toUpperCase("ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ"));
        }

        [TestMethod]
        public void toZenkaku()
        {
            Assert.AreEqual("ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ", Util.toZenkaku("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ"));
            Assert.AreEqual("　！＂＃＄％＆＇（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［＼］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～", Util.toZenkaku(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~"));
            Assert.AreEqual("。「」、・ヲァィゥェォャュョッ－アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワン゛゜", Util.toZenkaku("｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ"));
            Assert.AreEqual("゛゜バピ", Util.toZenkaku("ﾞﾟﾊﾞﾋﾟ"));
        }

        [TestMethod]
        public void toHiragana()
        {
            Assert.AreEqual("ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをん", Util.toHiragana("ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをん"));
        }

        [TestMethod]
        public void toNormalizedHyphen()
        {
            char[] HYPHEN_TABLE = { '\u002D', '\u00AD', '\u2010', '\u2011', '\u2012', '\u2013', '\u2014', '\u2015', '\u2212', '\u30FC', '\uFF70', '\uFF0D' };
            string expect = "";
            for (int i = 0; i < HYPHEN_TABLE.Length; ++i)
            {
                expect += "－";
            }
            Assert.AreEqual(expect, Util.toNormalizedHyphen(new string(HYPHEN_TABLE)));
        }
    }
}
