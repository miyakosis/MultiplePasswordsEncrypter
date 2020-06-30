using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiplePasswordsEncrypter;

namespace MultiplePasswordsEncrypterTest
{
    /// <summary>
    /// CombinationTest の概要の説明
    /// </summary>
    [TestClass]
    public class CombinationTest
    {
        [TestMethod]
        public void combination()
        {
            int[] table = { 0, 0, 2, 3, 6, 10, 20, 35, 70, 126,
                252, 462, 924, 1716, 3432, 6435, 12870, 24310, 48620, 92378,
                184756, 352716, 705432, 1352078, 2704156, 5200300, 10400600, 20058300, 40116600, 77558760,
            };

            for (int i = 2; i <= 29; ++i)
            {
//                System.Diagnostics.Trace.WriteLine(Combination.combination(i, i / 2));
                Assert.AreEqual(table[i], Combination.combination(i, i / 2));
            }

            try
            {
                int n = 30;
                Combination.combination(n, n / 2);
                Assert.Fail();
            }
            catch (OverflowException)
            {
                ;
            }
        }

        [TestMethod]
        public void enumerateCombination()
        {
            Combination.enumerateCombination(3, 2);

            for (int n = 1; n <= 10; ++n)
            {
                for (int k = 1; k <= n; ++k)
                {
                    int[][] result = Combination.enumerateCombination(n, k);
                    string[] results = new string[result.Length];
                    for (int i = 0; i < result.Length; ++i)
                    {
                        results[i] = "[" + string.Join(",", result[i]) + "]";
                    }
                    System.Diagnostics.Trace.WriteLine(n + "C" + k + " [" + string.Join(",", results));
                }
            }
        }
    }
}
