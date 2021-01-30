using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftUSkinEditor
{
    public class LOC
    {
        public LOC()
        {

        }

        public LOC(byte[] data)
        {
            Read(data);
        }

        public string readString(FileData f)
        {
            int length = f.readShort();
            string str = f.readString(f.pos(), length);
            f.skip(length);
            return str;
        }

        public class Language
        {
            public string name;
            public int unk1;
            public List<string> names = new List<string>();

            public string readString(FileData f)
            {
                int length = f.readShort();
                string str = f.readString(f.pos(), length);
                f.skip(length);
                return str;
            }

            public Language() { }

            public void Read(FileData f)
            {
                int idCount = f.readInt();
                for (int i = 0; i < idCount; i++)
                    names.Add(readString(f));
            }

            public byte[] Rebuild()
            {
                FileOutput f = new FileOutput();
                f.Endian = Endianness.Big;

                f.writeInt(names.Count);
                foreach(string name in names)
                {
                    f.writeShort(name.Length);
                    f.writeString(name);
                }

                return f.getBytes();
            }
        }

        public Language ids = new Language();
        public List<Language> langs = new List<Language>();

        public void Read(byte[] data)
        {
            FileData f = new FileData(data);
            f.Endian = Endianness.Big;

            int unk1 = f.readInt();
            if (unk1 != 2)
                throw new NotImplementedException("Not localization data");
            int langCount = f.readInt();
            f.skip(1);

            ids.Read(f);

            for(int i = 0; i < langCount; i++)
            {
                Language l = new Language();
                l.name = readString(f);
                l.unk1 = f.readInt();
                langs.Add(l);
            }

            foreach (Language l in langs)
            {
                f.skip(5);
                f.skip(f.readShort());
                l.Read(f);
            }
        }

        public byte[] Rebuild()
        {
            FileOutput f = new FileOutput();
            f.Endian = Endianness.Big;

            f.writeInt(2);
            f.writeInt(langs.Count);
            f.writeByte(0);

            f.writeBytes(ids.Rebuild());

            foreach(Language l in langs)
            {
                f.writeShort(l.name.Length);
                f.writeString(l.name);
                f.writeInt(7 + l.name.Length + l.Rebuild().Length);
            }

            foreach(Language l in langs)
            {
                f.writeInt(2);
                f.writeByte(0);
                f.writeShort(l.name.Length);
                f.writeString(l.name);
                f.writeBytes(l.Rebuild());
            }

            return f.getBytes();
        }
    }
}
