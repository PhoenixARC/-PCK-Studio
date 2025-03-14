using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PckStudio.Interfaces;

namespace PckStudio.Internal
{
    internal class EditorControl<T> : UserControl, IEditor<T> where T : class
    {
        public T EditorValue { get; }

        public ISaveContext<T> SaveContext { get; }

        public EditorControl()
        {
        }

        protected EditorControl(T value, ISaveContext<T> saveContext)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value));
            EditorValue = value;
            SaveContext = saveContext;
        }

        public void Save() => SaveContext.Save(EditorValue);

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            if (SaveContext.AutoSave)
                Save();
            base.OnControlRemoved(e);
        }

        public virtual void SaveAs()
        {
        }

        public virtual void Close()
        {
        }

        public virtual void UpdateView()
        {
        }
    }
}
