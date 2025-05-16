using System;
using System.Linq;
using System.Text;

namespace mtarTool
{
    public class MtarHeader
    {
        public uint Magic { get; set; }
        public ushort MaxJoint { get; set; }
        public ushort MaxEffPos { get; set; }
        public ushort NumBoneName { get; set; }
        public ushort NumMotion { get; set; }
        public uint Flags { get; set; }
        public uint MtcmOffset { get; set; }
        public uint MtexOffset { get; set; }
        public uint BoneNameTableOffset { get; set; }
        public uint DataTableOffset { get; set; }

        public static MtarHeader FromBytes(byte[] data)
        {
            if (data.Length < 32)
                throw new ArgumentException("Insufficient data for MtarHeader.");

            MtarHeader header = new MtarHeader
            {
                Magic = ReadUInt32BE(data, 0),
                MaxJoint = ReadUInt16BE(data, 4),
                MaxEffPos = ReadUInt16BE(data, 6),
                NumBoneName = ReadUInt16BE(data, 8),
                NumMotion = ReadUInt16BE(data, 10),
                Flags = ReadUInt32BE(data, 12),
                MtcmOffset = ReadUInt32BE(data, 16),
                MtexOffset = ReadUInt32BE(data, 20),
                BoneNameTableOffset = ReadUInt32BE(data, 24),
                DataTableOffset = ReadUInt32BE(data, 28)
            };
            return header;
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[32];
            WriteUInt32BE(bytes, 0, Magic);
            WriteUInt16BE(bytes, 4, MaxJoint);
            WriteUInt16BE(bytes, 6, MaxEffPos);
            WriteUInt16BE(bytes, 8, NumBoneName);
            WriteUInt16BE(bytes, 10, NumMotion);
            WriteUInt32BE(bytes, 12, Flags);
            WriteUInt32BE(bytes, 16, MtcmOffset);
            WriteUInt32BE(bytes, 20, MtexOffset);
            WriteUInt32BE(bytes, 24, BoneNameTableOffset);
            WriteUInt32BE(bytes, 28, DataTableOffset);
            return bytes;
        }

        private static ushort ReadUInt16BE(byte[] data, int startIndex)
        {
            return (ushort)((data[startIndex] << 8) | data[startIndex + 1]);
        }

        private static uint ReadUInt32BE(byte[] data, int startIndex)
        {
            return (uint)((data[startIndex] << 24) |
                          (data[startIndex + 1] << 16) |
                          (data[startIndex + 2] << 8) |
                          (data[startIndex + 3]));
        }

        private static void WriteUInt16BE(byte[] data, int startIndex, ushort value)
        {
            data[startIndex] = (byte)(value >> 8);
            data[startIndex + 1] = (byte)(value & 0xFF);
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
            string magicStr = string.Empty;
            try
            {
                magicStr = Encoding.ASCII.GetString(BitConverter.GetBytes(Magic).Reverse().ToArray());
            }
            catch
            {
                magicStr = "Non-ASCII";
            }

            return $"Magic: {Magic:X8} ('{magicStr}')\n" +
                   $"MaxJoint: {MaxJoint}\n" +
                   $"MaxEffPos: {MaxEffPos}\n" +
                   $"NumBoneName: {NumBoneName}\n" +
                   $"NumMotion: {NumMotion}\n" +
                   $"Flags: {Flags:X8}\n" +
                   $"MtcmOffset: {MtcmOffset:X8}\n" +
                   $"MtexOffset: {MtexOffset:X8}\n" +
                   $"BoneNameTableOffset: {BoneNameTableOffset:X8}\n" +
                   $"DataTableOffset: {DataTableOffset:X8}";
        }
    }
}