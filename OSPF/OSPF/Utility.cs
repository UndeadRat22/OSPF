namespace OSPF
{
    public static class Utility
    {
        public static int[][] Matrix(int size)
        {
            int[][] newArray = new int[size][];
            for (int i = 0; i < size; i++)
                newArray[i] = new int[size];
            return newArray;
        }
    }
}