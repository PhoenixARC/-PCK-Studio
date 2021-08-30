using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PckStudio.Models;

namespace PckStudio.Forms
{
    public partial class Testx_12 : Form
    {
        public Testx_12()
        {
            InitializeComponent();
            foreach (ModelBase modelBase in this.models)
            {
                modelBase.Updated += this.model_Updated;
            }
        }


        private void model_Updated(object sender, EventArgs e)
        {
            this.minecraftModelView1.Model = (sender as ModelBase);
        }

        private void Testx_12_Load(object sender, EventArgs e)
        {
            PckStudio.Classes.CSM.TryParse(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\export.CSM"), minecraftModelView1);
            //ModelBase modelBase = models[0];
            //this.minecraftModelView1.Model = modelBase;
            //this.minecraftModelView1.Invalidate();
            
        }

		private ModelBase[] models = new ModelBase[]
		{
			new CharacterModel()
		};
	}
}
