using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.API.Miles;
using PckStudio.Forms.Additional_Popups;
using System.Windows.Forms;
using System.IO;

namespace PckStudio.Classes.Utils
{
    internal static class BinkaConverter
    {

        public static void ToWav(Stream source, Stream destination)
        {
            throw new NotImplementedException();
        }

        public static void ToWav(string[] filenames, DirectoryInfo destination)
        {
            int convertedCount = 0;
            InProgressPrompt waitDiag = new InProgressPrompt();
            waitDiag.Show();
            foreach (string file in filenames)
            {
                Binka.ToWav(file, Path.Combine(destination.FullName, Path.GetFileNameWithoutExtension(file) + ".binka"));
                convertedCount++;
            }

            waitDiag.Close();
            waitDiag.Dispose();
            MessageBox.Show($"Successfully converted {convertedCount}/{filenames.Length} file{(filenames.Length != 1 ? "s" : "")}", "Done!");
        }

    }
}
