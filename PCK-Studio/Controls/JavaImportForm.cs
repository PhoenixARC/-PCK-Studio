using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PckStudio.Core;
using PckStudio.Core.DLC;
using PckStudio.Core.IO.Java;

namespace PckStudio.Controls
{
    public partial class JavaImportForm : ImmersiveForm
    {
        static Dictionary<string, RichTextBoxColor> _javaColorCodeToColor = new Dictionary<string, RichTextBoxColor>()
        {
            ["§0"] = new RichTextBoxColor(Color.FromArgb(0x00, 0x00, 0x00), Color.FromArgb(18, 18, 18)),
            ["§1"] = new RichTextBoxColor(Color.FromArgb(0x00, 0x00, 0xAA), Color.FromArgb(0x00, 0x00, 0x2A)),
            ["§2"] = new RichTextBoxColor(Color.FromArgb(0x00, 0xAA, 0x00), Color.FromArgb(0x00, 0x2A, 0x00)),
            ["§3"] = new RichTextBoxColor(Color.FromArgb(0x00, 0xAA, 0xAA), Color.FromArgb(0x00, 0x2A, 0x2A)),
            ["§4"] = new RichTextBoxColor(Color.FromArgb(0xAA, 0x00, 0x00), Color.FromArgb(0x2A, 0x00, 0x00)),
            ["§5"] = new RichTextBoxColor(Color.FromArgb(0xAA, 0x00, 0xAA), Color.FromArgb(0x2A, 0x00, 0x2A)),
            ["§6"] = new RichTextBoxColor(Color.FromArgb(0xFF, 0xAA, 0x00), Color.FromArgb(0x2A, 0x2A, 0x00)),
            ["§7"] = new RichTextBoxColor(Color.FromArgb(0xAA, 0xAA, 0xAA), Color.FromArgb(0x2A, 0x2A, 0x2A)),
            ["§8"] = new RichTextBoxColor(Color.FromArgb(0x55, 0x55, 0x55), Color.FromArgb(0x15, 0x15, 0x15)),
            ["§9"] = new RichTextBoxColor(Color.FromArgb(0x55, 0x55, 0xFF), Color.FromArgb(0x15, 0x15, 0x3F)),
            ["§a"] = new RichTextBoxColor(Color.FromArgb(0x55, 0xFF, 0x55), Color.FromArgb(0x15, 0x3F, 0x15)),
            ["§b"] = new RichTextBoxColor(Color.FromArgb(0x55, 0xFF, 0xFF), Color.FromArgb(0x15, 0x3F, 0x3F)),
            ["§c"] = new RichTextBoxColor(Color.FromArgb(0xFF, 0x55, 0x55), Color.FromArgb(0x3F, 0x15, 0x15)),
            ["§d"] = new RichTextBoxColor(Color.FromArgb(0xFF, 0x55, 0xFF), Color.FromArgb(0x3F, 0x15, 0x3F)),
            ["§e"] = new RichTextBoxColor(Color.FromArgb(0xFF, 0xFF, 0x55), Color.FromArgb(0x3F, 0x3F, 0x15)),
            ["§f"] = new RichTextBoxColor(Color.FromArgb(0xFF, 0xFF, 0xFF), Color.FromArgb(0x3F, 0x3F, 0x3F)),
            ["§g"] = new RichTextBoxColor(Color.FromArgb(0xDD, 0xD6, 0x05), Color.FromArgb(0x37, 0x35, 0x01)),
            ["§h"] = new RichTextBoxColor(Color.FromArgb(0xE3, 0xD4, 0xD1), Color.FromArgb(0x38, 0x35, 0x34)),
            ["§i"] = new RichTextBoxColor(Color.FromArgb(0xCE, 0xCA, 0xCA), Color.FromArgb(0x33, 0x32, 0x32)),
            ["§j"] = new RichTextBoxColor(Color.FromArgb(0x44, 0x3A, 0x3B), Color.FromArgb(0x11, 0x0E, 0x0E)),
            ["§m"] = new RichTextBoxColor(Color.FromArgb(0x97, 0x16, 0x07), Color.FromArgb(0x25, 0x05, 0x01)),
            ["§n"] = new RichTextBoxColor(Color.FromArgb(0xB4, 0x68, 0x4D), Color.FromArgb(0x2D, 0x1A, 0x13)),
            ["§p"] = new RichTextBoxColor(Color.FromArgb(0xDE, 0xB1, 0x2D), Color.FromArgb(0x37, 0x2C, 0x0B)),
            ["§q"] = new RichTextBoxColor(Color.FromArgb(0x47, 0xA0, 0x36), Color.FromArgb(0x04, 0x28, 0x0D)),
            ["§s"] = new RichTextBoxColor(Color.FromArgb(0x2C, 0xBA, 0xA8), Color.FromArgb(0x0B, 0x2E, 0x2A)),
            ["§t"] = new RichTextBoxColor(Color.FromArgb(0x21, 0x49, 0x7B), Color.FromArgb(0x08, 0x12, 0x1E)),
            ["§u"] = new RichTextBoxColor(Color.FromArgb(0x9A, 0x5C, 0xC6), Color.FromArgb(0x26, 0x17, 0x31)),
        };
        private ResourcePackImporter _importer;
        private readonly DLCManager _dlcManager;
        private readonly ImportStatusReport _importStatusReport;

        public DLCPackageContent Result { get; private set; }

        public JavaImportForm(FileInfo fileInfo, DLCManager dlcManager)
        {
            InitializeComponent();
            importWorker.DoWork += Import;
            importWorker.ProgressChanged += ImportProgressChanged;
            importWorker.RunWorkerCompleted += ImportCompleted;
            _importer = new ResourcePackImporter(default);
            _dlcManager = dlcManager;
            _importStatusReport = ImportStatusReport.CreateCustom(importWorker.ReportProgressInfo);
            StartImport(fileInfo);
        }

        private void StartImport(FileInfo fileInfo)
        {
            importButton.Click -= Import_Click;
            importButton.Click += Cancel_Click;
            importButton.Text = "Cancel";
            var zip = new ZipArchive(fileInfo.OpenRead(), ZipArchiveMode.Read);

            string name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            if (_importer.StartImport(name, zip, _importStatusReport))
            {
                FormatPackDescription(Path.GetFileNameWithoutExtension(fileInfo.Name), _importer.ReadPackMeta(zip).Description);
                importWorker.RunWorkerAsync(zip);
            }
        }

        private void ImportProgressChanged(object sender, ProgressChangedEventArgs eventArgs)
        {
            if (eventArgs?.UserState is not ProgressReportMessage reportMessage)
                return;
            richTextBox1.AppendLine($"[{reportMessage.Type}]: {reportMessage.Message}", reportMessage.MessageColor);
        }

        private void Import(object sender, DoWorkEventArgs eventArgs)
        {
            if (sender is not BackgroundWorker worker)
            {
                Debug.WriteLine("Sender was not a background worker.");
                eventArgs.Cancel = true;
                return;
            }
            if (eventArgs.Argument is not ZipArchive zip)
            {
                worker.ReportProgressError("Invalid argument passed to background worker.");
                eventArgs.Cancel = true;
                return;
            }
            worker.ReportProgressInfo($"Start import");
            while(!(eventArgs.Cancel = worker.CancellationPending))
            {
                ImportResult<DLCTexturePackage, ResourcePackImporter.TextureImportStats> res = _importer.ImportAsTexturePack();
                DLCPackageContent pck = _dlcManager.CompilePackage(res.Result);

                worker.ReportProgressDebug("Import Stats");
                worker.ReportProgressDebug($"Textures: {res.Stats.Textures}/{res.Stats.MaxTextures}({res.Stats.MissingTextures} missing)");
                worker.ReportProgressDebug($"Animations: {res.Stats.Animations}");
                // on success do:
                if (!worker.CancellationPending)
                {
                    worker.ReportProgressInfo($"Import successful");
                    eventArgs.Result = pck;
                    break;
                }
            }
        }

        private void ImportCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            importButton.Click -= Cancel_Click;
            importButton.Click += Import_Click;
            importButton.Text = "Import";
            if (e.Cancelled || importWorker.CancellationPending)
            {
                MessageBox.Show("Import cancelled", $"Import cancelled.");
                return;
            }
            MessageBox.Show("Import successful", $"");
            Result = (DLCPackageContent)e.Result;
            DialogResult = DialogResult.OK;
        }

        private void FormatPackDescription(string title, string description)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            richTextBox1.AppendLine("Name: ");
            FormatJavaFormatString(title);
            richTextBox1.AppendLine("Description: ");
            FormatJavaFormatString(description);
            richTextBox1.AppendLine("");
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        private bool FormatJavaFormatString(string text)
        {
            if (!text.Contains("§"))
            {
                richTextBox1.AppendLine(text);
                richTextBox1.AppendLine("");
                return true;
            }

            foreach (KeyValuePair<string, string> textSection in text.Split(['§'], StringSplitOptions.RemoveEmptyEntries).Select(s => new KeyValuePair<string, string>("§" + s[0], string.IsNullOrWhiteSpace(s) ? string.Empty : s.Substring(1))))
            {
                FontStyle fontStyle = FontStyle.Regular;
                switch (textSection.Key)
                {
                    // obfuscated/MTS*
                    case "§k":
                        var rng = new Random();
                        string value = new string(textSection.Value.Select(c => Convert.ToChar(c + rng.Next(26 - (char.ToLower(c) - 0x30)))).ToArray());
                        richTextBox1.AppendText(value);
                        continue;
                    // bold
                    case "§l":
                        if (richTextBox1.Font.FontFamily.IsStyleAvailable(FontStyle.Bold))
                            fontStyle |= FontStyle.Bold;
                        break;
                    // strikethrough
                    case "§m":
                        if (richTextBox1.Font.FontFamily.IsStyleAvailable(FontStyle.Strikeout))
                            fontStyle |= FontStyle.Strikeout;
                        break;
                    // underline
                    case "§n":
                        if (richTextBox1.Font.FontFamily.IsStyleAvailable(FontStyle.Underline))
                            fontStyle |= FontStyle.Underline;
                        break;
                    // italic
                    case "§o":
                        if (richTextBox1.Font.FontFamily.IsStyleAvailable(FontStyle.Italic))
                            fontStyle |= FontStyle.Italic;
                        break;
                    // reset
                    case "§r":
                        richTextBox1.SelectionColor = richTextBox1.ForeColor;
                        richTextBox1.SelectionFont = richTextBox1.Font;
                        break;
                    default:
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, fontStyle);
                        if (_javaColorCodeToColor.TryGetValue(textSection.Key, out RichTextBoxColor textColor))
                        {
                            richTextBox1.AppendText(textSection.Value, textColor);
                            break;
                        }
                        Debug.WriteLine(textSection);
                        richTextBox1.AppendText(textSection.Key);
                        richTextBox1.AppendText(textSection.Value);
                        richTextBox1.AppendText(textSection.Key.Substring(1));
                        richTextBox1.AppendText(textSection.Value);
                        break;
                }
            }
            richTextBox1.AppendLine("");
            return true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            importWorker.DoWork -= Import;
            importWorker.ProgressChanged -= ImportProgressChanged;
            importWorker.RunWorkerCompleted -= ImportCompleted;
            if (importWorker.IsBusy)
                importWorker.CancelAsync();
            base.OnFormClosing(e);
        }

        private void Import_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                Filter = "Minecraft texturepack|*.zip"
            };
            if (fileDialog.ShowDialog() != DialogResult.OK)
                return;
            StartImport(new FileInfo(fileDialog.FileName));
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (importWorker.IsBusy)
            {
                importWorker.CancelAsync();
                importButton.Click -= Cancel_Click;
                importButton.Click += Import_Click;
                importButton.Text = "Import";
            } 
        }
    }
}
