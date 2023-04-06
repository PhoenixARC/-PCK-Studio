/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System.Numerics;
using Newtonsoft.Json;

namespace PckStudio.Conversion.Bedrock.JsonDefinitions
{
    internal class GeometryBone
    {
        public GeometryBone(string name, string parent, Vector3 pivot, GeometryCube[] cubes, string metaBone = "")
        {
            Name = name;
            Parent = parent;
            pivot.CopyTo(Pivot);
            Cubes = cubes;
            //if (!String.IsNullOrEmpty(metaBone)) this.META_BoneType = metaBone;
        }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        //public string META_BoneType = null;

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name = "partName";

        [JsonProperty(PropertyName = "parent", NullValueHandling = NullValueHandling.Ignore)]
        public string Parent = "parentName";

        [JsonProperty(PropertyName = "pivot", NullValueHandling = NullValueHandling.Ignore)]
        public float[] Pivot = { 0, 0, 0 };

        [JsonProperty(PropertyName = "cubes", NullValueHandling = NullValueHandling.Ignore)]
        public GeometryCube[] Cubes { get; set; }
    }
}
