using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Forms.Additional_Popups;
using System.Windows.Forms;
using System.IO;
using PckStudio.Internal;
using System.Text.RegularExpressions;
using PckStudio.Internal.App;

namespace PckStudio.External.API.Miles
{
    internal static class BinkaConverter
    {
        public static void ToWav(string[] filenames, DirectoryInfo destination)
        {
            int convertedCount = 0;
            InProgressPrompt waitDiag = new InProgressPrompt();
            waitDiag.Show(Program.MainInstance);
            foreach (string file in filenames)
            {
                Binka.ToWav(file, Path.Combine(destination.FullName, Path.GetFileNameWithoutExtension(file) + ".binka"));
                convertedCount++;
            }

            waitDiag.Close();
            waitDiag.Dispose();
            MessageBox.Show(Program.MainInstance, $"Successfully converted {convertedCount}/{filenames.Length} file{(filenames.Length > 1 ? "s" : "")}", "Done!");
        }

        public static void ToBinka(string[] filenames, DirectoryInfo destination, int compressionLevel = 4)
        {
            int convertedCount = 0;
            Directory.CreateDirectory(ApplicationScope.DataCacher.CacheDirectory);

            InProgressPrompt waitDiag = new InProgressPrompt();
            waitDiag.Show(Program.MainInstance);
            
            foreach (string file in filenames)
            {
                string[] a = Path.GetFileNameWithoutExtension(file).Split(Path.GetInvalidFileNameChars());
                string songName = string.Join("_", a);
                // Replace UTF characters
                songName = Regex.Replace(songName, @"[^\u0000-\u007F]+", "_");
                string cacheSongFilepath = Path.Combine(ApplicationScope.DataCacher.CacheDirectory, songName + Path.GetExtension(file));

                using (var reader = new NAudio.Wave.WaveFileReader(file))
                {
                    var newFormat = new NAudio.Wave.WaveFormat(reader.WaveFormat.SampleRate, 16, reader.WaveFormat.Channels);
                    using (var conversionStream = new NAudio.Wave.WaveFormatConversionStream(newFormat, reader))
                    {
                        NAudio.Wave.WaveFileWriter.CreateWaveFile(cacheSongFilepath, conversionStream); //write to new location
                    }
                }

                Cursor.Current = Cursors.WaitCursor;
                int exitCode = Binka.ToBinka(cacheSongFilepath, Path.Combine(destination.FullName, Path.GetFileNameWithoutExtension(file) + ".binka"), compressionLevel);
                if (exitCode == 0)
                    convertedCount++;
            }

            waitDiag.Close();
            waitDiag.Dispose();
            MessageBox.Show(Program.MainInstance, $"Successfully converted {convertedCount}/{filenames.Length} file{(filenames.Length > 1 ? "s" : "")}", "Done!");
        }
    }
}
