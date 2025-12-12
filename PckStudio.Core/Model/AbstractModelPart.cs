using System;
using System.Collections.Generic;
using System.Numerics;

namespace PckStudio.Core.Model
{
    public class AbstractModelPart
    {
        public string Name { get; }
        public AbstractModelPart Parent { get; }
        public Vector3 Translation { get; }
        public Vector3 Rotation { get; }

        private IList<Box> _boxes;
        private List<AbstractModelPart> _subParts;

        public AbstractModelPart(string name, AbstractModelPart parent, Vector3 translation, Vector3 rotation, IEnumerable<Box> boxes)
        {
            Name = name;
            Parent = parent;
            Translation = translation;
            Rotation = rotation;
            _boxes = new List<Box>(boxes);
            _subParts = new List<AbstractModelPart>();
        }

        public void AddBox(Box box) => _boxes.Add(box);

        internal void AddParts(IEnumerable<AbstractModelPart> parts) => _subParts.AddRange(parts);
    }
}
