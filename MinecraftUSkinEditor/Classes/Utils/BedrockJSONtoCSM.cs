using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PckStudio.Classes.Utils.ModelConversion
{
    public class BedrockJSONtoCSM
    {
        public string JSONtoCSM(string JsonString)
        {
            dynamic jsonDe = JsonConvert.DeserializeObject<dynamic>(JsonString);
            string NewJSON = JsonConvert.SerializeObject(jsonDe["minecraft:geometry"]);
            JObject[] NewJObject = JsonConvert.DeserializeObject<JObject[]>(NewJSON);

            string CSMData = "";
            foreach (JBone bone in NewJObject[0].bones)
            {
                int i = 0;
                string PARENT = bone.name;
                foreach (JCube Cube in bone.cubes)
                {
                    string name = PARENT + " " + i;

                    float PosXModifier = 0;
                    float PosYModifier = 0;
                    float PosZModifier = 0;

                    switch (PARENT)
                    {
                        case "ARM0":
                            PosXModifier = 5;
                            PosYModifier = 22;
                            break;
                        case "ARM1":
                            PosXModifier = -5;
                            PosYModifier = 22;
                            break;
                        case "LEG0":
                            PosXModifier = 1.9f;
                            PosYModifier = 12;
                            break;
                        case "LEG1":
                            PosXModifier = -1.9f;
                            PosYModifier = 12;
                            break;
                        case "BODY":
                            PosYModifier = 24;
                            break;
                        case "HEAD":
                            PosYModifier = 24;
                            break;
                    }


                    float PosX = Cube.origin[0] + PosXModifier;
                    float PosY = Cube.origin[1] + PosYModifier;
                    float PosZ = Cube.origin[2] + PosZModifier;
                    float SizeX = Cube.size[0];
                    float SizeY = Cube.size[1];
                    float SizeZ = Cube.size[2];
                    float UvX = Cube.uv[0];
                    float UvY = Cube.uv[1];

                    CSMData += name + "\n" + PARENT + "\n" + name + "\n" + PosX + "\n" + PosY + "\n" + PosZ + "\n" + SizeX + "\n" + SizeY + "\n" + SizeZ + "\n" + UvX + "\n" + UvY + "\n";
                    i++;
                }
            }
            return CSMData;
        }
    }

    internal class WholeJSON
    {
        public string format_version = "1.12.0";
        public Dictionary<string, object> entries = new Dictionary<string, object>();
    }

    internal class JObject
    {
        public Dictionary<string, object> description = new Dictionary<string, object>();
        public JBone[] bones = { };
    }
    internal class JBone
    {
        public string name = "";
        public int[] pivot = {0, 0, 0};
        public JCube[] cubes = { };
    }
    internal class JCube
    {
        public float[] origin = new float[3];
        public float[] size = new float [3];
        public float[] uv = new float[2];
    }
}
