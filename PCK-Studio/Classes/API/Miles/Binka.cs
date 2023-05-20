using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SharpMSS;

namespace PckStudio.API.Miles
{
	internal static class Binka
	{
		public static int FromWav(string inputFilepath, string outputFilepath, int compressionLevel)
		{
			ApplicationScope.AppDataCacher.Cache(Properties.Resources.binka_encode, "binka_encode.exe");
            var process = Process.Start(new ProcessStartInfo
			{
				FileName = ApplicationScope.AppDataCacher.GetCachedFilepath("binka_encode.exe"),
				Arguments = $"\"{inputFilepath}\" \"{outputFilepath}\" -s -b{compressionLevel}",
				UseShellExecute = true,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			});
			process.WaitForExit();
			return process.ExitCode;
		}

		public static void ToWav(string inputFilepath, string outputFilepath)
		{
			if (!inputFilepath.EndsWith(".binka"))
			{
				throw new Exception("Not a Bink Audio file.");
			}

			ApplicationScope.AppDataCacher.Cache(Properties.Resources.mss32, "mss32.dll");
            ApplicationScope.AppDataCacher.Cache(Properties.Resources.binkawin, "binkawin.asi");

			LibHandle mss32LibHandle = new LibHandle(ApplicationScope.AppDataCacher.GetCachedFilepath("mss32.dll"));

			string destinationFilepath = Path.Combine(
                Path.GetDirectoryName(outputFilepath),
				Path.GetFileNameWithoutExtension(inputFilepath) + ".wav");

			AILAPI.SetRedistDirectory(ApplicationScope.AppDataCacher.CacheDirectory.Replace('\\', '/'));
			
			RIBAPI.LoadApplicationProviders("*.asi");
            
            byte[] inputFiledata = File.ReadAllBytes(inputFilepath);

			int resultBufferSize = 0;
			IntPtr resultBuffer = new IntPtr();
            if (AILAPI.DecompressASI(inputFiledata, inputFiledata.Length, ".binka", ref resultBuffer, ref resultBufferSize, null) == 0)
			{
				Console.WriteLine(AILAPI.GetLastError());
				return;
			}

			byte[] buffer = new byte[resultBufferSize];
			Marshal.Copy(resultBuffer, buffer, 0, resultBufferSize);
            
			AILAPI.MemFreeLock(resultBuffer);
			RIBAPI.FreeProviderLibrary(0); // free all loaded providers
            AILAPI.Shutdown();

            File.WriteAllBytes(destinationFilepath, buffer);

        }
    }
}
