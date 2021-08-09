using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace stonevox
{
    public class CollectionEditorHook : CollectionEditor
    {
        public delegate void CollectionEditorEventHandler(object sender,
                                            FormClosedEventArgs e);

        public static event CollectionEditorEventHandler FormClosed;

        public CollectionEditorHook(Type Type) : base(Type) { }
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();
            collectionForm.FormClosed += new FormClosedEventHandler(collection_FormClosed);
            return collectionForm;
        }

        void collection_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (FormClosed != null)
            {
                FormClosed(this, e);
            }
        }
    }
}
