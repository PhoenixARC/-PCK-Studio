using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using PckStudio.Interfaces;
using PckStudio.Core;

namespace PckStudio.Core
{
    public sealed class DelegatedFileSaveContext<T> : ISaveContext<T>
    {
        public delegate void SerializeDataToStreamDelegate(T value, Stream stream);

        public bool AutoSave { get; }
        public string Filepath { get; private set; }
        private SerializeDataToStreamDelegate _serializeDataDelegate;
        private FileDialogFilter _dialogFilter;

        public DelegatedFileSaveContext(string filepath, bool autoSave, FileDialogFilter dialogFilter, SerializeDataToStreamDelegate serializeDataDelegate)
        {
            AutoSave = autoSave;
            Filepath = filepath;
            _serializeDataDelegate = serializeDataDelegate;
            _dialogFilter = dialogFilter;
        }

        public void Save(T value)
        {
            if (!File.Exists(Filepath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = _dialogFilter.ToString();
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                Filepath = saveFileDialog.FileName;
            }
            using (Stream stream = File.OpenWrite(Filepath))
            {
                _serializeDataDelegate(value, stream);
            }
        }
    }
}
