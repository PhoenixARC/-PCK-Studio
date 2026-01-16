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
            StartImport(fileInfo);
        }

        private void StartImport(FileInfo fileInfo)
        {
            importButton.Click -= Import_Click;
            importButton.Click += Cancel_Click;
            importButton.Text = "Cancel";
            var zip = new ZipArchive(fileInfo.OpenRead(), ZipArchiveMode.Read);

            string name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            if (_importer.StartImport(name, zip, ImportStatusReport.Debug))
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
                    // bold §l
                    case "§l":
                        if (richTextBox1.Font.FontFamily.IsStyleAvailable(FontStyle.Bold))
                            fontStyle |= FontStyle.Bold;
                        break;
                    // strikethrough §m
                    case "§m":
                        if (richTextBox1.Font.FontFamily.IsStyleAvailable(FontStyle.Strikeout))
                            fontStyle |= FontStyle.Strikeout;
                        break;
                    // underline §n
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
                        if (JavaConstants.JavaColorCodeToColor.TryGetValue(textSection.Key, out (Color foreground, Color background) textColor))
                        {
                            richTextBox1.AppendText(textSection.Value, new RichTextBoxColor(textColor.foreground, textColor.background));
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
