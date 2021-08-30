using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using MinecraftUSkinEditor.Models;
using PckStudio.Properties;

namespace MinecraftUSkinEditor.Models
{
	public class TextureSelector : global::System.Windows.Forms.UserControl
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.textureNameBox = new global::System.Windows.Forms.TextBox();
			this.browseButton = new global::System.Windows.Forms.Button();
			this.openFileDialog = new global::System.Windows.Forms.OpenFileDialog();
			this.reloadButton = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.textureNameBox.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Left | global::System.Windows.Forms.AnchorStyles.Right);
			this.textureNameBox.Enabled = false;
			this.textureNameBox.Location = new global::System.Drawing.Point(3, 5);
			this.textureNameBox.Name = "textureNameBox";
			this.textureNameBox.Size = new global::System.Drawing.Size(0x86, 0x14);
			this.textureNameBox.TabIndex = 0;
			this.browseButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right);
			this.browseButton.Location = new global::System.Drawing.Point(0x8F, 3);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new global::System.Drawing.Size(0x20, 0x17);
			this.browseButton.TabIndex = 1;
			this.browseButton.Text = "...";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += this.OnBrowseButtonClick;
			this.openFileDialog.DefaultExt = "png";
			this.openFileDialog.Filter = "PNG (*.png)|*.png";
			this.reloadButton.Anchor = (global::System.Windows.Forms.AnchorStyles.Top | global::System.Windows.Forms.AnchorStyles.Right);
			this.reloadButton.Enabled = false;
			this.reloadButton.Image = global::PckStudio.Properties.Resources.BINKA_ICON;
			this.reloadButton.Location = new global::System.Drawing.Point(0xB5, 3);
			this.reloadButton.Name = "reloadButton";
			this.reloadButton.Size = new global::System.Drawing.Size(0x20, 0x17);
			this.reloadButton.TabIndex = 1;
			this.reloadButton.UseVisualStyleBackColor = true;
			this.reloadButton.Click += this.OnReloadButtonClick;
			this.AllowDrop = true;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.reloadButton);
			base.Controls.Add(this.browseButton);
			base.Controls.Add(this.textureNameBox);
			base.Margin = new global::System.Windows.Forms.Padding(0);
			base.Name = "TextureSelector";
			base.Size = new global::System.Drawing.Size(0xD8, 0x20);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public global::PckStudio.Models.Texture Texture
		{
			get
			{
				return this.texture;
			}
			set
			{
				this.texture = value;
				this.UpdateTextureName();
				this.UpdateReloadButton();
			}
		}

		private void UpdateTextureName()
		{
			this.textureNameBox.Text = this.texture.ToString();
		}

		private void UpdateReloadButton()
		{
			this.reloadButton.Enabled = (this.texture != null && !string.IsNullOrEmpty(this.texture.FileName));
		}

		public TextureSelector()
		{
			this.InitializeComponent();
			global::System.Windows.Forms.ToolTip toolTip = new global::System.Windows.Forms.ToolTip();
			toolTip.SetToolTip(this.browseButton, "Browse...");
			toolTip.SetToolTip(this.reloadButton, "Reload texture");
		}

		private void OnBrowseButtonClick(object sender, global::System.EventArgs e)
		{
			if (this.openFileDialog.ShowDialog() == global::System.Windows.Forms.DialogResult.OK)
			{
				string fileName = this.openFileDialog.FileName;
				using (global::System.IO.FileStream fileStream = global::System.IO.File.OpenRead(fileName))
				{
					global::System.Drawing.Image image = global::System.Drawing.Image.FromStream(fileStream);
					if (image.Width != this.texture.Width || image.Height != this.texture.Height)
					{
						global::System.Windows.Forms.MessageBox.Show("Image '" + global::System.IO.Path.GetFileName(fileName) + "' has wrong size.", "Minecraft Skin Viewer Extended", global::System.Windows.Forms.MessageBoxButtons.OK, global::System.Windows.Forms.MessageBoxIcon.Hand);
					}
					else
					{
						this.texture.Source = image;
						this.texture.FileName = fileName;
						this.UpdateTextureName();
						this.reloadButton.Enabled = true;
					}
				}
			}
		}

		private void OnReloadButtonClick(object sender, global::System.EventArgs e)
		{
			if (this.texture == null || this.texture.Source == null)
			{
				this.reloadButton.Enabled = false;
				return;
			}
			string fileName = this.texture.FileName;
			if (!global::System.IO.File.Exists(fileName))
			{
				global::System.Windows.Forms.MessageBox.Show("Image '" + global::System.IO.Path.GetFileName(fileName) + "' not found.", "Minecraft Skin Viewer Extended", global::System.Windows.Forms.MessageBoxButtons.OK, global::System.Windows.Forms.MessageBoxIcon.Hand);
				return;
			}
			using (global::System.IO.FileStream fileStream = global::System.IO.File.OpenRead(fileName))
			{
				global::System.Drawing.Image image = global::System.Drawing.Image.FromStream(fileStream);
				if (image.Width != this.texture.Width || image.Height != this.texture.Height)
				{
					global::System.Windows.Forms.MessageBox.Show("Image '" + global::System.IO.Path.GetFileName(fileName) + "' has wrong size.", "Minecraft Skin Viewer Extended", global::System.Windows.Forms.MessageBoxButtons.OK, global::System.Windows.Forms.MessageBoxIcon.Hand);
				}
				else
				{
					this.texture.Source = image;
					this.UpdateTextureName();
				}
			}
		}

		protected override void OnDragEnter(global::System.Windows.Forms.DragEventArgs drgevent)
		{
			if (drgevent.Data.GetDataPresent(global::System.Windows.Forms.DataFormats.FileDrop))
			{
				string[] array = (string[])drgevent.Data.GetData(global::System.Windows.Forms.DataFormats.FileDrop, false);
				if (array.Length == 1 && array[0].EndsWith(".PNG", true, global::System.Globalization.CultureInfo.CurrentCulture))
				{
					drgevent.Effect = global::System.Windows.Forms.DragDropEffects.Copy;
				}
			}
		}

		protected override void OnDragDrop(global::System.Windows.Forms.DragEventArgs drgevent)
		{
			string text = ((string[])drgevent.Data.GetData(global::System.Windows.Forms.DataFormats.FileDrop, false))[0];
			using (global::System.IO.FileStream fileStream = global::System.IO.File.OpenRead(text))
			{
				global::System.Drawing.Image image = global::System.Drawing.Image.FromStream(fileStream);
				if (image.Width != this.texture.Width || image.Height != this.texture.Height)
				{
					global::System.Windows.Forms.MessageBox.Show("Image '" + global::System.IO.Path.GetFileName(text) + "' has wrong size.", "Minecraft Skin Viewer Extended", global::System.Windows.Forms.MessageBoxButtons.OK, global::System.Windows.Forms.MessageBoxIcon.Hand);
				}
				else
				{
					this.texture.Source = image;
					this.texture.FileName = text;
					this.UpdateTextureName();
					this.reloadButton.Enabled = true;
				}
			}
		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.TextBox textureNameBox;

		private global::System.Windows.Forms.Button browseButton;

		private global::System.Windows.Forms.OpenFileDialog openFileDialog;

		private global::System.Windows.Forms.Button reloadButton;

		private global::PckStudio.Models.Texture texture;
	}
}
