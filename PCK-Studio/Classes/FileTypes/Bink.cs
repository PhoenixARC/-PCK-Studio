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

		public int temp_error_code;
		string binka_enc_loc;
		string mss32_loc;
		string binkawin_loc;
		public string working = null;

		public async void WavToBinka(string infile, string outFile, int compression)
		{
			var process = Process.Start(new ProcessStartInfo
			{
				FileName = binka_enc_loc,
				Arguments = $"\"{infile}\" \"{outFile}\" -s -b{compression}",
				UseShellExecute = true,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			});
			process.WaitForExit();
			temp_error_code = process.ExitCode;
		}

		public unsafe void BinkaToWav(string infile, string outFile)
		{
			string[] array2 = createArg(infile, outFile);
			byte[] array3 = File.ReadAllBytes(array2[0]);
			Console.WriteLine(array3.Length);
			uint num = 0U;
			AIL_set_redist_directory(".");
			AIL_startup();
			IntPtr intPtr;
			// crash happens in AIL_decompress_ASI
			if (AIL_decompress_ASI(array3, (uint)array3.Length, ".binka", &intPtr, &num, 0U) == 0)
				throw new Exception("AIL ERROR");
			byte[] array4 = new byte[num];
			Marshal.Copy(intPtr, array4, 0, array4.Length);
			AIL_mem_free_lock(intPtr);
			AIL_shutdown();
			File.WriteAllBytes(array2[1], array4);
		}

		public void SetUpBinka()
		{
			if (working == null)
			{
				working = (Path.GetTempPath() + "PCKStudio").Replace("\\","/");
				Directory.CreateDirectory(working);
				binka_enc_loc = ExtractResource("binka_encode.exe", working);
				mss32_loc = ExtractResource("mss32.dll", working);
				binkawin_loc = ExtractResource("binkawin.asi", working);
				library = LoadLibrary(mss32_loc);
			}
			else
			{
				binka_enc_loc = working + "\\binka_encode.exe";
				mss32_loc = working + "\\mss32.dll";
				binkawin_loc = working + "\\binkawin.asi";
			}
		}

		public void CleanUpBinka()
		{
			FreeLibrary(library);
			File.Delete(binka_enc_loc);
			File.Delete(binkawin_loc);
			while (File.Exists(mss32_loc))
			{
				try
				{
					File.Delete(mss32_loc);
				}
				catch
				{
					FreeLibrary(library);
				}
			}
			Directory.Delete(working);
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
			string type = getType(inFile);
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

		internal static string ExtractResource(string resource, string working)
		{
			object ob = Properties.Resources.ResourceManager.GetObject(Path.GetFileNameWithoutExtension(resource));
			byte[] myResBytes = (byte[])ob;
			if(File.Exists(Path.Combine(working, resource))) File.Delete(Path.Combine(working, resource));
			using (FileStream fsDst = new FileStream(Path.Combine(working, resource), FileMode.CreateNew, FileAccess.Write))
			{
				fsDst.Write(myResBytes, 0, myResBytes.Length);
				fsDst.Close();
				fsDst.Dispose();
			}
			return "\"" + working + "/" + resource + "\"";
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
