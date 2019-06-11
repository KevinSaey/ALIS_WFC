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
        public static Random RandomNR = new Random();

        public static List<U> CrossreferenceIncludeList<U>(List<U> baseList, List<U> filterList)
        {
            return baseList.Where(l => filterList.Contains(l)).ToList();
        }

        public static bool CheckIndex(Vector3IntShared index, Vector3IntShared dimensions)
        {
            if (index.X < 0 || index.X > dimensions.X - 1 || index.Y < 0 || index.Y > dimensions.Y - 1 || index.Z < 0 || index.Z > dimensions.Z - 1)
            {
                //SharedLogger.Log("Outside of bounds. -function CheckIndex.-");
                return false;
            }
            return true;
        }

        public static Vector3IntShared GetIndexInGrid(int i, Vector3IntShared dimensions)
        {
            int newX=0, newY=0, newZ=0;
            if (i<dimensions.X*dimensions.Y*dimensions.Z)
            {
                newZ = i / (dimensions.X * dimensions.Y);
                i %= (dimensions.X * dimensions.Y);
                newY = i / dimensions.X;
                i %= dimensions.X;
                newX = i;
            }
            else
            {
                SharedLogger.Log("Error: index doesn't fit within grid. -function GetIndexInGrid.-");
            }

            return new Vector3IntShared { X = newX, Y = newY, Z = newZ };

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
