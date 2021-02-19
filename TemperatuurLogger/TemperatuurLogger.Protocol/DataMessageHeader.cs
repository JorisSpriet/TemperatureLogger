using System.Runtime.InteropServices;

namespace TemperatuurLogger.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataMessageHeader
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Header;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] SerialNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] AnswerCode;

        public short NumberOfMessages;

        public short MessageNumber;

        public byte MessageSampleCount;

    }
}