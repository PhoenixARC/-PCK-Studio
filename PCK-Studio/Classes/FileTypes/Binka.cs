using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace PckStudio.Classes
{
	internal static class Binka
	{
		private class LibHandle
		{
			[DllImport("kernel32.dll")]
			public static extern IntPtr LoadLibrary(string lpFileName);

			[DllImport("kernel32.dll")]
			public static extern int FreeLibrary(IntPtr library);

			private IntPtr libHandle;

			public IntPtr Handle => libHandle;

			public LibHandle(string libraryPath)
			{
				libHandle = LoadLibrary(libraryPath);
			}

			~LibHandle()
			{
				FreeLibrary(libHandle);
			}
		}

		private static string dataCache = Program.AppDataCache;

		public static uint LastAILErrorCode = 0xffffffff;

		public static int FromWav(string inputFilepath, string outputFilepath, int compressionLevel)
		{
			var process = Process.Start(new ProcessStartInfo
			{
				FileName = GetAndCacheResource("binka_encode.exe"),
				Arguments = $"\"{inputFilepath}\" \"{outputFilepath}\" -s -b{compressionLevel}",
				UseShellExecute = true,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			});
			process.WaitForExit();
			return process.ExitCode;
		}

		public static unsafe void ToWav(string inputFilename, string outputFilepath)
		{
			// handle should be closed when function gets out of scope
			LibHandle mss32LibHandle = new LibHandle(GetAndCacheResource("mss32.dll"));

            string ext = Path.GetExtension(inputFilename).ToLower();
			switch (ext)
			{
				case ".binka":
                    inputFilename = inputFilename.Replace(".binka", ".wav");
					break;
				case ".wav":
                    inputFilename = inputFilename.Replace(".wav", ".binka");
					break;
				default:
					throw new NotSupportedException(nameof(ext)+":"+ext);
			}
			string outputDirectory = Path.GetDirectoryName(outputFilepath);
            Directory.CreateDirectory(outputDirectory);
			string destinationFilepath = Path.Combine(outputDirectory, Path.GetFileName(inputFilename));

            byte[] inputFiledata = File.ReadAllBytes(inputFilename);
			Debug.WriteLine($"{nameof(inputFiledata)}: {inputFiledata.Length}");

			AILInternalCalls.SetRedistDirectory(".");
			AILInternalCalls.Startup(); // __fastcall

			int resultBufferSize = 0;
			IntPtr resultBuffer = new IntPtr();
            // crash happens in AIL_decompress_ASI
            LastAILErrorCode = (uint)AILInternalCalls.DecompressASI(inputFiledata, inputFiledata.Length, ".binka", resultBuffer, &resultBufferSize);
			Debug.WriteLine("AIL Error Code: " + LastAILErrorCode.ToString());

			byte[] buffer = new byte[resultBufferSize];
			Marshal.Copy(resultBuffer, buffer, 0, resultBufferSize);
			AILInternalCalls.MemFreeLock(resultBuffer);
            AILInternalCalls.Shutdown();
			File.WriteAllBytes(destinationFilepath, buffer);
		}

		// Move to a cache class ?
		private static string GetAndCacheResource(string resourceName)
		{
			_ = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
			string destinationFilepath = Path.Combine(dataCache, resourceName);
			if (!File.Exists(destinationFilepath))
			{
				byte[] resourceData = ExtractResource(resourceName);
				using (FileStream fsDst = File.OpenWrite(destinationFilepath))
				{
					fsDst.Write(resourceData, 0, resourceData.Length);
				}
			}
			return destinationFilepath;
		}

		internal static byte[] ExtractResource(string resourceName)
		{
			byte[] resourceData = (byte[])Properties.Resources.ResourceManager.GetObject(Path.GetFileNameWithoutExtension(resourceName));
			return resourceData;
		}

		private class AILInternalCalls
		{
			public delegate int AIL_decomp_func();

			[DllImport("mss32.dll", EntryPoint = "_AIL_decompress_ASI@24")]
			public unsafe static extern int DecompressASI(
				[MarshalAs(UnmanagedType.LPArray)] byte[] indata, 
				int insize,
				[MarshalAs(UnmanagedType.LPStr)] string ext,
				IntPtr result,
				int *resultSize,
                [MarshalAs(UnmanagedType.FunctionPtr)] AIL_decomp_func func = null);

			[DllImport("mss32.dll", EntryPoint = "_AIL_last_error@0")]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string LastError();

            [DllImport("mss32.dll", EntryPoint = "_AIL_set_redist_directory@4")]
            [return: MarshalAs(UnmanagedType.LPStr)]
            public static extern string SetRedistDirectory([MarshalAs(UnmanagedType.LPStr)] string redistDir);

            [DllImport("mss32.dll", EntryPoint = "_AIL_mem_free_lock@4")]
			public static extern void MemFreeLock(IntPtr ptr);

			[DllImport("mss32.dll", EntryPoint = "_AIL_startup@0")]
			public static extern int Startup();

			[DllImport("mss32.dll", EntryPoint = "_AIL_shutdown@0")]
			public static extern int Shutdown();
		}
	}
}
