using System;

namespace Library.LabServer.Drivers
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
