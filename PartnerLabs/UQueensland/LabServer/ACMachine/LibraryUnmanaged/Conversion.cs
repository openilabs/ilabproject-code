using System;

namespace Library.Unmanaged
{
    public class Conversion
    {
        unsafe public static float ToFloat(int inum)
        {
            float fnum = *(float*)&inum;
            return fnum;
        }
    }
}
