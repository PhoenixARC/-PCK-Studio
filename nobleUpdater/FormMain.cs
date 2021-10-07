using System;
using System.Xml;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace nobleUpdater
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			this.InitializeComponent();

		}

		private void updateTool()
		{
			try
			{
				if (!Directory.Exists(this.appData + "backup\\"))
				{
					Directory.CreateDirectory(this.appData + "backup\\");
				}
				if (File.Exists(this.appData + "backup\\" + Path.GetFileName(this.localFile)))
				{
					File.Delete(this.appData + "backup\\" + Path.GetFileName(this.localFile));
				}
				try
				{
					File.Copy(this.localFile, this.appData + "backup\\" + Path.GetFileName(this.localFile));
				}
				catch (Exception)
				{
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				Application.Exit();
			}
			using (WebClient webClient = new WebClient())
			{
				webClient.DownloadFileCompleted += this.Completed;
				webClient.DownloadProgressChanged += this.ProgressChanged;
				try
				{
					webClient.DownloadFileAsync(new Uri(this.serverFile), this.localFile);
				}
				catch (Exception ex2)
				{
					MessageBox.Show(ex2.ToString());
					Application.Exit();
				}
			}
		}

		private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			this.progressBarUpdate.Value = e.ProgressPercentage;
			this.labelProgress.Text = string.Format("{0} MB's / {1} MB's", ((double)e.BytesReceived / 1024.0 / 1024.0).ToString("0.00"), ((double)e.TotalBytesToReceive / 1024.0 / 1024.0).ToString("0.00"));
		}

		private void Completed(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				MessageBox.Show("Download has been canceled.");
			}
			else
			{
				this.progressBarUpdate.Maximum = this.progressBarUpdate.Value;
				this.labelProgress.Text = "Download Complete";
			}
			new Process
			{
				StartInfo =
				{
					FileName = this.localFile
				}
			}.Start();
			Application.Exit();
		}

		private string serverFile = "http://www.pckstudio.xyz/programs/PCKSTUDIO_Update.exe";
		private string ServerXML = "http://www.pckstudio.xyz/studio/PCK/update.xml";

		private string appData = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\PCK Studio\\";

		private string localFile = Environment.CurrentDirectory + "\\PCK Studio.exe";

		private Thread thread;

		private WebClient webClient;

		private void FormMain_Load(object sender, EventArgs e)
		{
			Console.WriteLine(new WebClient().DownloadString(new Uri("http://www.pckstudio.xyz/studio/PCK/update.xml")));
			downloadUpdate();
		}

		public void downloadUpdate()
		{
			try
			{
				foreach (Process proc in Process.GetProcessesByName("PCK Studio"))
				{
					proc.Kill();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			catch { }

			string TryXMLDl = new WebClient().DownloadString(ServerXML);
			string[] raw = TryXMLDl.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
			XmlTextReader reader = new XmlTextReader(ServerXML);
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element: // The node is an element.
						Console.Write("<" + reader.Name + " || " + reader.LineNumber);
						Console.WriteLine(">");
						if (reader.Name == "FileUpdateTask")
						{
							try
							{
								Directory.CreateDirectory(Path.GetDirectoryName(Environment.CurrentDirectory + raw[reader.LineNumber - 1].Replace("	<FileUpdateTask localPath=\"", "").Replace("\">", "").Replace("/", "\\")));
								string url = ServerXML.Replace(".xml", "") + raw[reader.LineNumber - 1].Replace("	<FileUpdateTask localPath=\"", "").Replace("\">", "");
								new WebClient().DownloadFile(url, Environment.CurrentDirectory + raw[reader.LineNumber - 1].Replace("	<FileUpdateTask localPath=\"", "").Replace("\">", "").Replace("/", "\\"));
							}
							catch { }
						}
						break;
				}
			}
			new Process
			{
				StartInfo =
				{
					FileName = this.localFile
				}
			}.Start();
			Application.Exit();
		}

		public void mainus()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int num = 0;
			for (; ; )
			{
				if (num % 100000 == 0)
				{
					stopwatch.Stop();
					if (stopwatch.ElapsedMilliseconds > 5000L)
					{
						break;
					}
					stopwatch.Start();
				}
				num++;
			}
			this.updateTool();
		}

		private void progressBarUpdate_Click(object sender, EventArgs e)
		{

		}
	}
}
