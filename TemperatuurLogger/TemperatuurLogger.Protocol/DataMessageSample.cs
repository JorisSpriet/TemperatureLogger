using System;
using System.Runtime.InteropServices;

namespace TemperatuurLogger.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataMessageSample
    {
        public int EncodedTimeStamp;

        public short Temperature;

        public DateTime GetTimeStamp()
        {

            /*
            17 010001    Y  0-63 = 6 bits
            10   1010    M  0-12 = 4 bits
            01  00001    d  0-31 = 5 bits 
            13  01101    h  0-24 = 5 bits
            17 010001   min 0-59 = 6 bits
            10 001010   sec 0-59 = 6 bits
            */
            var year = 2000+((EncodedTimeStamp >> 26) & 0x0000003F);
            var month = (EncodedTimeStamp >> 22) & 0x0000000F;
            var day = (EncodedTimeStamp >> 17) & 0x0000001F;
            var hour = (EncodedTimeStamp >> 12) & 0x0000001F;
            var min = (EncodedTimeStamp >> 6) & 0x0000003F;
            var sec = (EncodedTimeStamp >> 6) & 0x0000003F;
            return new DateTime(year, month, day, hour, min, sec);
        }

    }
}
