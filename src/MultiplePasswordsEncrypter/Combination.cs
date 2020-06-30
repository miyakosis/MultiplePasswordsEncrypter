using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplePasswordsEncrypter
{
    // Calculating Combination
    // precond:n <= 29  (if n >= 30, result may exceed int max)
    public class Combination
    {
        // Calculating number of Combination. nCk
        public static int combination(int n, int k)
        {
            // Recurrence formula
            //(J) 漸化式
            k = Math.Min(k, n - k);
            int sum = 1;
            for (int i = 1; i <= k; ++i)
            {
                int nextSum = sum * (n - i + 1) / i;
                if (nextSum <= sum)
                {
                    throw new OverflowException("too many questions.");
                }
                sum = nextSum;
            }
            return sum;
        }

        // Enumerate Combination. nCk
        public static int[][] enumerateCombination(int n, int k)
        {
            int[] currentResult = new int[k];

            int nCombination = combination(n, k);
            int[][] result = new int[nCombination][];   // Enumerated Combinations
            for (int i = 0; i < nCombination; ++i)
            {
                result[i] = new int[k];
            }
            int resultPos = 0;  // Position to store enumeration.

            enumerateCombinationRcsv(n, k, 0, currentResult, result, ref resultPos);

            return result;
        }

        // Recursively seek combinations
        //(J) 再帰的に組み合わせを求める
        protected static void enumerateCombinationRcsv(int n, int k, int col, int[] currentResult, int[][] result, ref int resultPos)
        {
            if (col == k)
            {
                Array.Copy(currentResult, 0, result[resultPos++], 0, k);
                return;
            }

            // Count up from the digit of the previous digit + 1
            //(J) 一つ前の桁の数字 + 1 から順にカウントアップ
            int startI = ((col == 0) ? 0 : currentResult[col - 1] + 1);
            int endI = n - k + col;
            for (int i = startI; i <= endI; ++i)
            {
                currentResult[col] = i;
                enumerateCombinationRcsv(n, k, col + 1, currentResult, result, ref resultPos);
            }
        }
    }
}
