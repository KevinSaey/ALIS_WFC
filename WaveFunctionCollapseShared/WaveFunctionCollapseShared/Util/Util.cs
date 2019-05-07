using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse.Shared
{
    static class UtilShared
    {
        public static List<U> CrossreferenceIncludeList<U>(List<U> baseList, List<U> filterList)
        {
            return baseList.Where(l => filterList.Contains(l)).ToList();
        }

        public static List<int> ToIntegerList(BitArray bitArray)
        {
            List<int> integerList = new List<int>();

            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i] == true) integerList.Add(i);
            }

            return integerList;
        }

       public static BitArray ToBitArray(List<int> integerList, int length)
        {
            BitArray bitArray = new BitArray(length, false);
            foreach (var i in integerList)
            {
                bitArray[i] = true;
            }
            return bitArray;
        }

        public static int CountBitarrayTrue(BitArray bitArray)
        {
            int counter = 0;
            for (int i = 0; i < bitArray.Count; i++)
            {
                if (bitArray[i]) counter++;
            }
               
            return counter;
        }

        public static void SetFalseBut(BitArray bitArray, int i)
        {
            if (i > bitArray.Count)
            {
                SharedLogger.Log($"Index {i} bigger than BitArray size {bitArray.Count}");
            }
            else
            {
                bitArray.SetAll(false);
                bitArray[i] = true;
            }

        }

        public static bool CheckIndex(Vector3IntShared index, Vector3IntShared dimensions)
        {
            SharedLogger.Log(index.ToString());
            if (index.x < 0 || index.x > dimensions.x - 1 || index.y <0 || index.y > dimensions.y - 1 ||index.z<0|| index.z > dimensions.z - 1)
            {
                SharedLogger.Log("Outside of bounds");
                return false;
            }
            return true;
        }
    }

    public interface ISharedLogger
    {
        void Log(object message);
    }

    public static class SharedLogger
    {
        public static ISharedLogger CurrentLogger { get; set; }

        public static void Log(object message)
        {
            CurrentLogger.Log(message);
        }
    }

    

}
