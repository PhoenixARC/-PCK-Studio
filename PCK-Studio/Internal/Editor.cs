using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using PckStudio.Interfaces;

namespace PckStudio.Internal
{
    public abstract class Editor<T> : MetroForm where T : class
    {
        protected T EditorValue;
        private readonly ISaveContext<T> SaveContext;

        protected Editor(T value, ISaveContext<T> saveContext)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            EditorValue = value;
            SaveContext = saveContext;
        }

        protected void Save() => SaveContext.Save(EditorValue);

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (SaveContext.AutoSave)
                Save();
            base.OnFormClosing(e);
        }
    }
}