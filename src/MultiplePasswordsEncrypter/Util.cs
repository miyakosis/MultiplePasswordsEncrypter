using CSharp.Japanese.Kanaxs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiplePasswordsEncrypter
{
    public class Util
    {
        public static bool isEmpty(string s, bool? isTrim)
        {
            if (s == "")
            {
                return true;
            }
            else if ((bool)isTrim && s.Trim() == "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string toRemoveInnerSpaces(string s)
        {
            return Regex.Replace(s, @"\s", "");
        }

        public static string    toUpperCase(string s)
        {
            return s.ToUpper();
        }

        public static string toZenkaku(string s)
        {
            s = Kana.ToZenkaku(s);
            s = KanaEx.ToZenkakuKana(s);
            s = toNormalizedHyphen(s);

            return s;
        }
        
        public static string toHiragana(string s)
        {
            s = Kana.ToHiragana(s);
            s = toNormalizedHyphen(s);

            return s;
        }

        // http://d.hatena.ne.jp/y-kawaz/20101112/1289554290
        // -  U+002D HYPHEN-MINUS
        //    U+00AD SOFT HYPHEN
        // ‐ U+2010 HYPHEN
        // ‑  U+2011 NON-BREAKING HYPHEN
        // ‒  U+2012 FIGURE DASH
        // –  U+2013 EN DASH
        // —  U+2014 EM DASH
        // ― U+2015 HORIZONTAL BAR
        // −  U+2212 MINUS SIGN
        // ー U+30FC KATAKANA-HIRAGANA PROLONGED SOUND MARK
        // ｰ  U+FF70 Halfwidth Katakana-hiragana Prolonged Sound Mark
        // － U+FF0D FULLWIDTH HYPHEN-MINUS
        private static readonly char[] HYPHEN_TABLE = { '\u002D', '\u00AD', '\u2010', '\u2011', '\u2012', '\u2013', '\u2014', '\u2015', '\u2212', '\u30FC', '\uFF70', '\uFF0D' };
        private static readonly char REPLACED_HYPHEN = '\uFF0D';

        public static string toNormalizedHyphen(string s)
        {
            char[] ca = s.ToCharArray();
            for (int i = 0; i < ca.Length; ++i)
            {
                foreach (var replC in HYPHEN_TABLE)
                {
                    if (ca[i] == replC)
                    {
                        ca[i] = REPLACED_HYPHEN;
                        break;
                    }                    
                }
            }
            return new string(ca);
        }
    }
}
