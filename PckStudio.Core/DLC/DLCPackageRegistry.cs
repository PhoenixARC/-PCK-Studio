using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Languages;
using PckStudio.Core.Interfaces;

namespace PckStudio.Core.DLC
{
    internal sealed class DLCPackageRegistry
    {
        private readonly IDictionary<int, IDLCPackage> _openPackages = new Dictionary<int, IDLCPackage>();
        private readonly IDictionary<int, LOCFile> _localisationFiles = new Dictionary<int, LOCFile>();

        public IDLCPackage this[int id] => _openPackages[id];

        public bool RegisterPackage(int identifier, IDLCPackage package, LOCFile localisation)
        {
            if (_openPackages.ContainsKey(identifier))
                return false;
            if (_localisationFiles.ContainsKey(identifier))
                _localisationFiles.Remove(identifier);
            _localisationFiles.Add(identifier, localisation);
            _openPackages.Add(identifier, package);
            return true;
        }

        internal bool ContainsPackage(int identifier) => _openPackages.ContainsKey(identifier) && _localisationFiles.ContainsKey(identifier);

        internal LOCFile GetLocalisation(int identifier) => _localisationFiles[identifier];
    }
}
