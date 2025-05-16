using System;

namespace mtarTool
{
    public class MtarData
    {
        public uint MtcmOffset { get; set; }
        public uint MtcmSize { get; set; }
        public uint MtexOffset { get; set; }
        public uint MtexSize { get; set; }

        public static MtarData FromBytes(byte[] data)
        {
            if (data.Length < 16)
                throw new ArgumentException("Insufficient data for MtarData.");

            MtarData mData = new MtarData
            {
                MtcmOffset = ReadUInt32BE(data, 0),
                MtcmSize = ReadUInt32BE(data, 4),
                MtexOffset = ReadUInt32BE(data, 8),
                MtexSize = ReadUInt32BE(data, 12)
            };
            return mData;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[16];
            WriteUInt32BE(bytes, 0, MtcmOffset);
            WriteUInt32BE(bytes, 4, MtcmSize);
            WriteUInt32BE(bytes, 8, MtexOffset);
            WriteUInt32BE(bytes, 12, MtexSize);
            return bytes;
        }

        private static uint ReadUInt32BE(byte[] data, int startIndex)
        {
            return (uint)((data[startIndex] << 24) |
                          (data[startIndex + 1] << 16) |
                          (data[startIndex + 2] << 8) |
                          (data[startIndex + 3]));
        }

        private static void WriteUInt32BE(byte[] data, int startIndex, uint value)
        {
            data[startIndex] = (byte)((value >> 24) & 0xFF);
            data[startIndex + 1] = (byte)((value >> 16) & 0xFF);
            data[startIndex + 2] = (byte)((value >> 8) & 0xFF);
            data[startIndex + 3] = (byte)(value & 0xFF);
        }

        public override string ToString()
        {
            return $"MtcmOffset: {MtcmOffset:X8}\n" +
                   $"MtcmSize: {MtcmSize:X8}\n" +
                   $"MtexOffset: {MtexOffset:X8}\n" +
                   $"MtexSize: {MtexSize:X8}";
        }
    }
}