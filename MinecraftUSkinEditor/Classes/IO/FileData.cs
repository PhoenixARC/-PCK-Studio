using System;
using System.Collections.Generic;
using System.IO;

namespace PckStudio
{
	public class FileData
	{
		private byte[] b;

		private int p;

		public Endianness Endian;

		public FileData(string f)
		{
			b = File.ReadAllBytes(f);
		}

		public FileData(byte[] b)
		{
			this.p = 0;
			this.b = b;
		}

		public int eof()
		{
			return b.Length;
		}

		public byte[] read(int length)
		{
			if (length + p > b.Length)
			{
				throw new IndexOutOfRangeException();
			}
			byte[] array = new byte[length];
			int num = 0;
			while (num < length)
			{
				array[num] = b[p];
				num++;
				p++;
			}
			return array;
		}

		public int readInt()
		{
			if (Endian == Endianness.Little)
			{
				return (b[p++] & 0xFF) | ((b[p++] & 0xFF) << 8) | ((b[p++] & 0xFF) << 16) | ((b[p++] & 0xFF) << 24);
			}
			int d = p;
			int oot = ((b[p++] & 0xFF) << 24) | ((b[p++] & 0xFF) << 16) | ((b[p++] & 0xFF) << 8) | (b[p++] & 0xFF);
			return oot;
		}

		public int readThree()
		{
			if (Endian == Endianness.Little)
			{
				return (b[p++] & 0xFF) | ((b[p++] & 0xFF) << 8) | ((b[p++] & 0xFF) << 16);
			}
			return ((b[p++] & 0xFF) << 16) | ((b[p++] & 0xFF) << 8) | (b[p++] & 0xFF);
		}

		public int readShort()
		{
			if (Endian == Endianness.Little)
			{
				return (b[p++] & 0xFF) | ((b[p++] & 0xFF) << 8);
			}
			return ((b[p++] & 0xFF) << 8) | (b[p++] & 0xFF);
		}

		public int readIntVita()
		{
			if (Endian != Endianness.Little)
			{
				int d = p;
				return (b[p++] & 0xFF) | ((b[p++] & 0xFF) << 8) | ((b[p++] & 0xFF) << 16) | ((b[p++] & 0xFF) << 24);
			}
			int oot = ((b[p++] & 0xFF) << 24) | ((b[p++] & 0xFF) << 16) | ((b[p++] & 0xFF) << 8) | (b[p++] & 0xFF);
			return oot;
		}

		public int readThreeVita()
		{
			if (Endian != Endianness.Little)
			{
				return (b[p++] & 0xFF) | ((b[p++] & 0xFF) << 8) | ((b[p++] & 0xFF) << 16);
			}
			return ((b[p++] & 0xFF) << 16) | ((b[p++] & 0xFF) << 8) | (b[p++] & 0xFF);
		}

		public int readShortVita()
		{
			if (Endian != Endianness.Little)
			{
				return (b[p++] & 0xFF) | ((b[p++] & 0xFF) << 8);
			}
			return ((b[p++] & 0xFF) << 8) | (b[p++] & 0xFF);
		}

		public int readByte()
		{
				return b[p++] & 0xFF;
		}

		public byte[] readBytes(int length)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < length; i++)
            {
					list.Add((byte)readByte());
			}
			return list.ToArray();
		}

		public float readFloat()
		{
			byte[] array = new byte[4];
			array = ((Endian != 0) ? new byte[4]
			{
				b[p + 3],
				b[p + 2],
				b[p + 1],
				b[p]
			} : new byte[4]
			{
				b[p],
				b[p + 1],
				b[p + 2],
				b[p + 3]
			});
			p += 4;
			return BitConverter.ToSingle(array, 0);
		}

		public float readHalfFloat()
		{
			return toFloat((short)readShort());
		}

		public static float toFloat(int hbits)
		{
			int num = hbits & 0x3FF;
			int num2 = hbits & 0x7C00;
			switch (num2)
			{
				case 31744:
					num2 = 261120;
					break;
				default:
					num2 += 114688;
					if (num == 0 && num2 > 115712)
					{
						return BitConverter.ToSingle(BitConverter.GetBytes(((hbits & 0x8000) << 16) | (num2 << 13) | 0x3FF), 0);
					}
					break;
				case 0:
					if (num != 0)
					{
						num2 = 115712;
						do
						{
							num <<= 1;
							num2 -= 1024;
						}
						while ((num & 0x400) == 0);
						num &= 0x3FF;
					}
					break;
			}
			return BitConverter.ToSingle(BitConverter.GetBytes(((hbits & 0x8000) << 16) | ((num2 | num) << 13)), 0);
		}

		public static int fromFloat(float fval, bool littleEndian)
		{
			int num = FileOutput.SingleToInt32Bits(fval, littleEndian);
			int num2 = (num >> 16) & 0x8000;
			int num3 = (num & 0x7FFFFFFF) + 4096;
			if (num3 >= 1199570944)
			{
				if ((num & 0x7FFFFFFF) >= 1199570944)
				{
					if (num3 < 2139095040)
					{
						return num2 | 0x7C00;
					}
					return num2 | 0x7C00 | ((num & 0x7FFFFF) >> 13);
				}
				return num2 | 0x7BFF;
			}
			if (num3 >= 947912704)
			{
				return num2 | (num3 - 939524096 >> 13);
			}
			if (num3 < 855638016)
			{
				return num2;
			}
			num3 = (num & 0x7FFFFFFF) >> 23;
			return num2 | (((num & 0x7FFFFF) | 0x800000) + (8388608 >> num3 - 102) >> 126 - num3);
		}

		public static int sign12Bit(int i)
		{
			if (((i >> 11) & 1) == 1)
			{
				i = ~i;
				i &= 0xFFF;
				i++;
				i *= -1;
			}
			return i;
		}

		public void skip(int i)
		{
			p += i;
		}

		public void seek(int i)
		{
			p = i;
		}

		public int pos()
		{
			return p;
		}

		public int size()
		{
			return b.Length;
		}

		public string readString()
		{
			string text = "";
			while (b[p] != 0)
			{
				string str = text;
				char c = (char)b[p];
				text = str + c;
				p++;
			}
			return text;
		}

		public byte[] getSection(int offset, int size)
		{
			byte[] array = new byte[size];
			Array.Copy(b, offset, array, 0, size);
			return array;
		}

		public string readString(int p, int size)
		{
			if (size == -1)
			{
				string text = "";
				while (p < b.Length && (b[p] & 0xFFu) != 0)
				{
					text += (char)(b[p] & 0xFFu);
					p++;
				}
				return text;
			}
			string text2 = "";
			for (int i = p; i < p + size; i++)
			{
				if ((b[i] & 0xFFu) != 0)
				{
					text2 += (char)(b[i] & 0xFFu);
				}
			}
			return text2;
		}

		public void align(int i)
		{
			while (p % i != 0)
			{
				p++;
			}
		}

		public int readOffset()
		{
			return p + readInt();
		}
	}
}
