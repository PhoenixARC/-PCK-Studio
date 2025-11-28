using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OMI.Formats.Pck;
using PckStudio.Core;
using PckStudio.Core.Extensions;
using PckStudio.Core.Skin;
using PckStudio.Forms.Additional_Popups;
using PckStudio.Forms.Editor;
using PckStudio.Properties;

namespace PckStudio.Controls
{
    public partial class DefaultPanel : ViewPanel
    {
        private PckAsset _currentAsset;
        private Action _onModified;

        private static readonly Skin _defaultSkin = new Skin("default", Resources.classic_template);

        private string ButtonText
        {
            get => buttonEdit.Text;
            set
            {
                buttonEdit.Visible = !string.IsNullOrWhiteSpace(value);
                buttonEdit.Text = value;
            }
        }

        public DefaultPanel()
        {
            InitializeComponent();
        }


        private void ReloadMetaTreeView()
        {
            treeMeta.Nodes.Clear();
            if (_currentAsset is not null)
            {
                foreach (KeyValuePair<string, string> property in _currentAsset.GetProperties())
                {
                    treeMeta.Nodes.CreateNode($"{property.Key}: {property.Value}", property);
                }
            }
        }

        private void treeMeta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                deleteEntryToolStripMenuItem_Click(sender, e);
        }

        private void editAllEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentAsset is not null)
            {
                IEnumerable<string> props = _currentAsset.SerializeProperties(seperater: " ");
                using (var input = new MultiTextPrompt(props))
                {
                    if (input.ShowDialog(this) == DialogResult.OK)
                    {
                        _currentAsset.ClearProperties();
                        _currentAsset.DeserializeProperties(input.TextOutput);
                        ReloadMetaTreeView();
                        _onModified();
                    }
                }
            }
        }

        private void addMultipleEntriesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_currentAsset is not null)
            {
                using var input = new MultiTextPrompt();
                if (input.ShowDialog(this) == DialogResult.OK)
                {
                    _currentAsset.DeserializeProperties(input.TextOutput);
                    ReloadMetaTreeView();
                    _onModified();
                }
            }
        }

        private void addBOXEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_currentAsset is not null)
            {
                using BoxEditor diag = new BoxEditor(SkinBOX.DefaultHead, false);
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    _currentAsset.AddProperty("BOX", diag.Result);
                    ReloadMetaTreeView();
                    _onModified();
                }
                return;
            }
        }

        private void addANIMEntryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_currentAsset is not null)
            {
                using ANIMEditor diag = new ANIMEditor(SkinANIM.Empty);
                if (diag.ShowDialog(this) == DialogResult.OK)
                {
                    _currentAsset.AddProperty("ANIM", diag.ResultAnim);
                    ReloadMetaTreeView();
                    _onModified();
                }
                return;
            }
        }

        private void deleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode is TreeNode t && t.Tag is KeyValuePair<string, string> property &&
                _currentAsset.RemoveProperty(property))
            {
                treeMeta.SelectedNode.Remove();
                _onModified();
            }
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_currentAsset is not null)
            {
                using AddPropertyPrompt addProperty = new AddPropertyPrompt();
                if (addProperty.ShowDialog(this) == DialogResult.OK)
                {
                    _currentAsset.AddProperty(addProperty.Property);
                    ReloadMetaTreeView();
                    _onModified();
                }
            }
        }

        private void treeMeta_DoubleClick(object sender, EventArgs e)
        {
            if (treeMeta.SelectedNode is TreeNode subnode && subnode.Tag is KeyValuePair<string, string> property &&
                _currentAsset is not null)
            {
                if (_currentAsset.HasProperty(property.Key))
                {
                    switch (property.Key)
                    {
                        case "ANIM" when _currentAsset.Type == PckAssetType.SkinFile:
                            try
                            {
                                using ANIMEditor diag = new ANIMEditor(SkinANIM.FromString(property.Value));
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    _currentAsset.SetProperty(_currentAsset.GetPropertyIndex(property), new KeyValuePair<string, string>("ANIM", diag.ResultAnim.ToString()));
                                    ReloadMetaTreeView();
                                    _onModified();
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                Trace.WriteLine("Invalid ANIM value: " + property.Value);
                                MessageBox.Show(this, "Failed to parse ANIM value, aborting to normal functionality. Please make sure the value only includes hexadecimal characters (0-9,A-F) and has no more than 8 characters.");
                            }
                            break;

                        case "BOX" when _currentAsset.Type == PckAssetType.SkinFile:
                            try
                            {
                                using BoxEditor diag = new BoxEditor(property.Value, false);
                                if (diag.ShowDialog(this) == DialogResult.OK)
                                {
                                    _currentAsset.SetProperty(_currentAsset.GetPropertyIndex(property), new KeyValuePair<string, string>("BOX", diag.Result.ToString()));
                                    ReloadMetaTreeView();
                                    _onModified();
                                }
                                return;
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                Trace.WriteLine("Invalid BOX value: " + property.Value);
                                MessageBox.Show(this, "Failed to parse BOX value, aborting to normal functionality.");
                            }
                            break;

                        default:
                            break;
                    }

                    using (AddPropertyPrompt addProperty = new AddPropertyPrompt(property))
                    {
                        if (addProperty.ShowDialog(this) == DialogResult.OK)
                        {
                            _currentAsset.SetProperty(_currentAsset.GetPropertyIndex(property), addProperty.Property);
                            ReloadMetaTreeView();
                            _onModified();
                        }
                    }
                }
            }
        }

        public override void Reset()
        {
            treeMeta.Nodes.Clear();
            previewPictureBox.Image = Resources.NoImageFound;
            skinRenderer.Visible = false;
            displayNameLabel.Visible = false;
            themeNameLabel.Visible = false;
            ButtonText = null;
            _currentAsset = null;
            _onModified = null;
        }

        public override void LoadAsset(PckAsset asset, Action onModified)
        {
            Reset();
            _currentAsset = asset;
            _onModified = onModified;
            switch (asset.Type)
            {
                case PckAssetType.SkinFile:
                case PckAssetType.CapeFile:
                case PckAssetType.TextureFile:
                {
                    previewPictureBox.Image = asset.GetTexture();

                    if (skinRenderer.IsInitialized)
                    {

                        if (asset.Type == PckAssetType.SkinFile || asset.Type == PckAssetType.CapeFile)
                        {
                            skinRenderer.Visible = true;
                            Skin skin = _defaultSkin;
                            if (asset.Type == PckAssetType.SkinFile)
                            {
                                skin = asset.GetSkin();
                                displayNameLabel.Visible = true;
                                themeNameLabel.Visible = true;
                                displayNameLabel.Text = skin.MetaData.Name;
                                themeNameLabel.Text = skin.MetaData.Theme;
                            }
                            skinRenderer.LoadSkin(skin);
                            if (asset.Type == PckAssetType.CapeFile)
                                skinRenderer.CapeTexture = previewPictureBox.Image;
                            break;
                        }
                    }

                    if (asset.Type != PckAssetType.TextureFile)
                        break;

                    ResourceLocation resourceLocation = ResourceLocations.GetFromPath(asset.Filename);
                    if (resourceLocation is null || resourceLocation.Category == ResourceCategory.Unknown)
                        break;

                    if ((resourceLocation.Category & ResourceCategory.Animation) != 0 && !asset.IsMipmappedFile())
                        ButtonText = "EDIT TILE ANIMATION";

                    if ((resourceLocation.Category & ResourceCategory.Atlas) != 0)
                        ButtonText = "EDIT TEXTURE ATLAS";

                    if ((resourceLocation.Category & ResourceCategory.Textures) != 0)
                        ButtonText = "EDIT TEXTURE";
                }
                break;

                case PckAssetType.LocalisationFile:
                    ButtonText = "EDIT LOC";
                    break;

                case PckAssetType.AudioFile:
                    ButtonText = "EDIT MUSIC CUES";
                    break;

                case PckAssetType.ColourTableFile when asset.Filename == "colours.col":
                    ButtonText = "EDIT COLORS";
                    break;

                case PckAssetType.BehavioursFile when asset.Filename == "behaviours.bin":
                    ButtonText = "EDIT BEHAVIOURS";
                    break;
                default:
                    break;
            }
            ReloadMetaTreeView();
        }
    }
}
