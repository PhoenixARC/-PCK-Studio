using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace stonevox
{
    public static class ImportExportUtil
    {
        static List<IImporter> importers = new List<IImporter>();
        static List<IExporter> exporters = new List<IExporter>();

        static ImportExportUtil()
        {
            importers.Add(new ImporterQb());
            importers.Add(new ImporterSVP());

            exporters.Add(new ExporterQb());
            exporters.Add(new ExporterSVP());
            exporters.Add(new ExporterObj());
        }

        public static bool Import(string path, bool setActive = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                MessageBox.Show("File not found...", "StoneVox");
                return false;
            }

            try
            {
                foreach (var importer in importers)
                {
                    if (path.EndsWith(importer.extension))
                    {
                        var model = importer.read(path);
                        //model.Sort();

                        Singleton<QbManager>.INSTANCE.AddModel(model, setActive);
                        if (setActive)
                            Singleton<Camera>.INSTANCE.LookAtModel();

                        // hacks
                        if (!Client.window.isfocused)
                        {
                            Program.SetForegroundWindow(Client.window.WindowInfo.Handle);
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        public static bool Export(string extension, string name, string path, QbModel model)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name))
            {
                return false;
            }

            try
            {
                foreach (var exporter in exporters)
                {
                    if (extension.Contains(exporter.extension))
                    {
                        exporter.write(path, name, model);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("StoneVox had trouble trying to save. This could be because the folder you are saving to is protected. Try saving somewhere else, then re-running SV as Admin.", "StoneVox - Saving Error");
                return false;
            }
            return false;
        }
    }
}