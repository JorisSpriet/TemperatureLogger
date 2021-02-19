using System.Runtime.InteropServices;

namespace TemperatuurLogger.Protocol
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataMessageTail
    {
        public byte Unknown;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Tail;
    }
}