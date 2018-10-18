using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnglishLearning.ExtendClasses
{
    public class Shuffle
    {
        static public List<T> ShuffleList<T>(List<T> list)
        {
            Random rand = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                Swap(i, rand.Next(i, list.Count), list);
            }
            return list;
        }

        static private void Swap<T>(int i, int j, List<T> list)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}