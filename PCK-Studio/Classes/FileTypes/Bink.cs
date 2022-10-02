using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PckStudio.Classes
{
	internal class Bink
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32.dll")]
		public static extern IntPtr FreeLibrary(IntPtr library);

		public unsafe static string Binka(string infile, string outDir = null, bool last = true, string working = null)
		{
			bool flag = working == null;
			string text;
			string text2;
			string path;
			if (flag)
			{
				working = Path.GetTempPath() + DateTime.Now.Second.ToString();
				text = PckStudio.Classes.Bink.ExtractResource("Resources.binka_encode.exe", working, "binka_encode.exe");
				text2 = PckStudio.Classes.Bink.ExtractResource("Resources.mss32.dll", working, "mss32.dll");
				path = PckStudio.Classes.Bink.ExtractResource("Resources.binkawin.asi", working, "binkawin.asi");
				PckStudio.Classes.Bink.library = PckStudio.Classes.Bink.LoadLibrary(text2);
			}
			else
			{
				text = working + "\\binka_encode.exe";
				text2 = working + "\\mss32.dll";
				path = working + "\\binkawin.asi";
			}
			bool flag2 = PckStudio.Classes.Bink.getType(infile) == "WAV";
			if (flag2)
			{
				string[] array = PckStudio.Classes.Bink.createArg(infile, outDir);
				Process process = new Process();
				process.StartInfo.FileName = text;
				process.StartInfo.Arguments = string.Concat(new string[]
				{
					" \"",
					array[0],
					"\" \"",
					array[1],
					"\""
				});
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				process.Start();
				process.WaitForExit();
			}
			else
			{
				bool flag3 = PckStudio.Classes.Bink.getType(infile) == "BINKA";
				if (flag3)
				{
					string[] array2 = PckStudio.Classes.Bink.createArg(infile, outDir);
					byte[] array3 = File.ReadAllBytes(array2[0]);
					uint num = 0U;
					PckStudio.Classes.Bink.AIL_set_redist_directory(".");
					PckStudio.Classes.Bink.AIL_startup();
					IntPtr intPtr;
					bool flag4 = PckStudio.Classes.Bink.AIL_decompress_ASI(array3, (uint)array3.Length, ".binka", &intPtr, &num, 0U) == 0;
					if (flag4)
					{
						throw new Exception("AIL ERROR");
					}
					byte[] array4 = new byte[num];
					Marshal.Copy(intPtr, array4, 0, array4.Length);
					PckStudio.Classes.Bink.AIL_mem_free_lock(intPtr);
					PckStudio.Classes.Bink.AIL_shutdown();
					File.WriteAllBytes(array2[1], array4);
				}
			}
			if (last)
			{
				PckStudio.Classes.Bink.FreeLibrary(PckStudio.Classes.Bink.library);
				PckStudio.Classes.Bink.FreeLibrary(PckStudio.Classes.Bink.library);
				File.Delete(text);
				File.Delete(path);
				while (File.Exists(text2))
				{
					try
					{
						File.Delete(text2);
					}
					catch
					{
						PckStudio.Classes.Bink.FreeLibrary(PckStudio.Classes.Bink.library);
					}
				}
			}
			return working;
		}

		private static string getType(string loc)
		{
			string a = Path.GetExtension(loc).ToLower();
			bool flag = a == ".binka";
			string result;
			if (flag)
			{
				result = "BINKA";
			}
			else
			{
				bool flag2 = !(a == ".wav");
				if (flag2)
				{
					throw new Exception("File type not valid. To use MP3 or other audio formats, convert to wav format before using this tool");
				}
				result = "WAV";
			}
			return result;
		}

		private static string[] createArg(string inFile, string outdir = null)
		{
			string[] array = new string[2];
			array[0] = inFile;
			string[] array2 = array;
			string type = PckStudio.Classes.Bink.getType(inFile);
			bool flag = type == "BINKA";
			if (flag)
			{
				array2[1] = ((outdir.Length <= 3) ? Path.GetFullPath(inFile.Replace(".binka", ".wav")) : (outdir + "\\" + Path.GetFileName(inFile.Replace(".binka", ".wav"))));
			}
			else
			{
				bool flag2 = type == "WAV";
				if (flag2)
				{
					array2[1] = ((outdir.Length <= 3) ? Path.GetFullPath(inFile.Replace(".wav", ".binka")) : (outdir + "\\" + Path.GetFileName(inFile.Replace(".wav", ".binka"))));
				}
			}
			bool flag3 = !Directory.Exists(Path.GetDirectoryName(array2[1]));
			if (flag3)
			{
				Directory.CreateDirectory(Path.GetDirectoryName(array2[1]));
			}
			return array2;
		}

		internal static string ExtractResource(string resource, string path, string filename)
		{
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
			byte[] array = new byte[(int)manifestResourceStream.Length];
			manifestResourceStream.Read(array, 0, array.Length);
			manifestResourceStream.Close();
			bool flag = !Directory.Exists(path);
			if (flag)
			{
				Directory.CreateDirectory(path);
			}
			path = path + "\\" + filename;
			File.WriteAllBytes(path, array);
			return path;
		}

		[DllImport("mss32.dll", EntryPoint = "_AIL_decompress_ASI@24")]
		private unsafe static extern int AIL_decompress_ASI([MarshalAs(UnmanagedType.LPArray)] byte[] indata, uint insize, [MarshalAs(UnmanagedType.LPStr)] string ext, IntPtr* result, uint* resultsize, uint zero);

		[DllImport("mss32.dll", EntryPoint = "_AIL_last_error@0")]
		private static extern IntPtr AIL_last_error();

		[DllImport("mss32.dll", EntryPoint = "_AIL_set_redist_directory@4")]
		private static extern IntPtr AIL_set_redist_directory([MarshalAs(UnmanagedType.LPStr)] string redistDir);

		[DllImport("mss32.dll", EntryPoint = "_AIL_mem_free_lock@4")]
		private static extern void AIL_mem_free_lock(IntPtr ptr);

		[DllImport("mss32.dll", EntryPoint = "_AIL_startup@0")]
		private static extern int AIL_startup();

		[DllImport("mss32.dll", EntryPoint = "_AIL_shutdown@0")]
		private static extern int AIL_shutdown();

		public Bink()
		{
		}

		private static IntPtr library;
	}
}
