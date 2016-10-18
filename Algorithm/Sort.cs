using System;
using Zhengdi.Framework.Enum;

namespace Zhengdi.Framework.Algorithm
{
    public class AlgorithmSorting
    {
        #region 排序算法
        static int QuickSortLogic(int[] values, int left, int right)
        {
            int key = values[left];
            int i = left;
            int j = right;
            while (i < j)
            {

                while (values[j] >= key && i < j) j--;
                values[i] = values[j];
                while (values[i] <= key && i < j) i++;
                values[j] = values[i];
            }
            values[i] = key;
            return i;
        }
        static int QuickSortLogic<T>(T[] values, int left, int right, Func<T,T, Direction,bool> compatison)
        {
            T key = values[left];
            int i = left;
            int j = right;
            while (i < j)
            {
                while (compatison(key, values[j], Direction.Left) && i < j) j--;
                values[i] = values[j];
                while (compatison(key, values[i], Direction.Right) && i < j) i++;
                values[j] = values[i];
            }
            values[i] = key;
            return i;
        }
        public static void QuickSort(int[] values, int left, int right)
        {
            if (left >= right) return;
            int KeyIndex = QuickSortLogic(values, left, right);
            QuickSort(values, left, KeyIndex - 1);
            QuickSort(values, KeyIndex + 1, right);
        }
        public static void QuickSort<T>(T[] values, int left, int right, Func<T, T, Direction, bool> compatison)
        {
            if (left >= right) return;
            int KeyIndex = QuickSortLogic<T>(values, left, right, compatison);
            QuickSort<T>(values, left, KeyIndex - 1, compatison);
            QuickSort<T>(values, KeyIndex + 1, right, compatison);
        }
        #endregion
    }
}
