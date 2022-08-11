using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckStudio.Classes.Utils.grf
{
    public class CRC32
    {
        static readonly private byte[] offsets =
        {
            0x05, 0x06, 0x07, 0x03,
            0x04, 0x05, 0x06, 0x07,
            0x01, 0x02, 0x06, 0x01,
            0x01, 0x02, 0x03, 0x04,
            0x05, 0x05, 0x06, 0x07,
            0x03, 0x04, 0x05, 0x06,
            0x07, 0x01, 0x02, 0x06,
            0x01, 0x01, 0x02, 0x03,
            0x04, 0x05, 0x05, 0x06,
            0x07, 0x03, 0x04, 0x05,
            0x06, 0x07, 0x07, 0x05,
            0x04, 0x07, 0x05, 0x04,
            0x07, 0x05, 0x04, 0x07,
            0x06, 0x05, 0x04, 0x03,
        };

        static private uint[] CRCTable;
        static private bool hasCRCTable = false;

        static void MakeCRCTable()
        {
            const uint val = 0xedb88320;
            CRCTable = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                uint temp = (uint)i;

                for (int j = 0; j < 8; j++)
                {
                    if ((temp & 1) == 1)
                    {
                        temp >>= 1;
                        temp ^= val;
                    }
                    else { temp >>= 1; }
                }
                CRCTable[i] = temp;
            }
            hasCRCTable = true;
        }
        public static uint UpdateCRC(uint _crc, byte[] data)
        {
            uint crc = _crc;
            if (!hasCRCTable)
                MakeCRCTable();
            long pos = 0;
            long offset = 0;
            do
            {
                var value = data[pos];
                pos += offsets[offset];
                crc = CRCTable[(crc ^ value) & 0xff] ^ crc >> 8;
                offset = offset + 1U + ((offset + 1U >> 3) / 7) * -0x38;
            } while (pos < data.Length);
            return crc;
        }

        public static uint CRC(byte[] data)
        {
            return ~UpdateCRC(0xffffffff, data);
        }
    }
}
