using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using PckStudio.Internal;
using SharpMSS;

namespace PckStudio.API.Miles
{
	internal static class Binka
	{
		private static ProcessStartInfo WavProcessStartInfo = new ProcessStartInfo
		{
			UseShellExecute = true,
			CreateNoWindow = true,
			WindowStyle = ProcessWindowStyle.Hidden
		};

		public static int FromWav(string inputFilepath, string outputFilepath, int compressionLevel)
		{
			ApplicationScope.DataCacher.Cache(Properties.Resources.binka_encode, "binka_encode.exe");
			WavProcessStartInfo.FileName = ApplicationScope.DataCacher.GetCachedFilepath("binka_encode.exe");
			WavProcessStartInfo.Arguments = $"\"{inputFilepath}\" \"{outputFilepath}\" -s -b{compressionLevel}";
            var process = Process.Start(WavProcessStartInfo);
			process.WaitForExit();
			return process.ExitCode;
		}

        public static void ToWav(Stream source, Stream destination)
        {
			_ = source ?? throw new ArgumentNullException(nameof(source));
			_ = destination ?? throw new ArgumentNullException(nameof(destination));
			if (!source.CanRead)
			{
                throw new NotSupportedException("Can not read from source stream");
            }

            if (!destination.CanWrite)
            {
                throw new NotSupportedException("Can not read to destination stream");
            }

            using var sourceFs = new MemoryStream();
            source.CopyTo(sourceFs);
            byte[] buffer = ToWav(sourceFs.ToArray());

			using(var ms = new MemoryStream(buffer))
			{
				ms.CopyTo(destination);
			}
        }

        public static void ToWav(string inputFilepath, string outputFilepath)
		{
			byte[] buffer = ToWav(inputFilepath);
            string destinationFilepath = Path.Combine(
                Path.GetDirectoryName(outputFilepath),
                Path.GetFileNameWithoutExtension(inputFilepath) + ".wav");
            File.WriteAllBytes(destinationFilepath, buffer);
        }

        public static byte[] ToWav(string inputFilepath)
		{
			if (!File.Exists(inputFilepath))
			{
				throw new FileNotFoundException(nameof(inputFilepath));
			}
            if (!inputFilepath.EndsWith(".binka"))
            {
                throw new ArgumentException("Not a Bink Audio file.");
            }
            return ToWav(File.ReadAllBytes(inputFilepath));
        }

        public static byte[] ToWav(byte[] input)
		{
			ApplicationScope.DataCacher.Cache(Properties.Resources.mss32, "mss32.dll");
            ApplicationScope.DataCacher.Cache(Properties.Resources.binkawin, "binkawin.asi");

			LibHandle mss32LibHandle = new LibHandle(ApplicationScope.DataCacher.GetCachedFilepath("mss32.dll"));

			AILAPI.SetRedistDirectory(ApplicationScope.DataCacher.CacheDirectory.Replace('\\', '/'));
			
			RIBAPI.LoadApplicationProviders("*.asi");

			int resultBufferSize = 0;
			IntPtr resultBuffer = new IntPtr();
            if (AILAPI.DecompressASI(input, input.Length, ".binka", ref resultBuffer, ref resultBufferSize, null) == 0)
			{
				Console.WriteLine(AILAPI.GetLastError());
				return Array.Empty<byte>();
			}

			byte[] buffer = new byte[resultBufferSize];
			Marshal.Copy(resultBuffer, buffer, 0, resultBufferSize);
            
			AILAPI.MemFreeLock(resultBuffer);
			RIBAPI.FreeProviderLibrary(0); // free all loaded providers
            AILAPI.Shutdown();
			mss32LibHandle.Release();
            return buffer;
        }
    }
}
