using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace stonevox
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000122-0000-0000-C000-000000000046")] // This is the value of IID_IDropTarget from the Platform SDK.
    [ComImport]
    public interface IDropTarget
    {
        void DragEnter([In] IDataObject dataObject, [In] uint keyState, [In] Point pt, [In, Out] ref uint effect);
        void DragOver([In] uint keyState, [In] Point pt, [In, Out] ref uint effect);
        void DragLeave();
        void Drop([In] IDataObject dataObject, [In] uint keyState, [In] Point pt, [In, Out] ref uint effect);
    }


    static class NativeMethods
    {
        public const int CF_HDROP = 15;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int DragQueryFile(HandleRef hDrop, int iFile, [Out] StringBuilder lpszFile, int cch);

        [DllImport("ole32.dll")]
        internal static extern void ReleaseStgMedium(ref STGMEDIUM medium);
    }

    public class DragDropTarget : stonevox.IDropTarget
    {
        public void DragEnter(IDataObject dataObject, uint keyState, Point pt, ref uint effect)
        {
        }

        public void DragOver(uint keyState, Point pt, ref uint effect)
        {
        }

        public void DragLeave()
        {
        }

        public void Drop(IDataObject dataObject, uint keyState, Point pt, ref uint effect)
        {
            FORMATETC format = new FORMATETC()
            {
                cfFormat = NativeMethods.CF_HDROP,
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                tymed = TYMED.TYMED_HGLOBAL
            };
            STGMEDIUM medium;
            string[] files;
            dataObject.GetData(ref format, out medium);
            try
            {
                IntPtr dropHandle = medium.unionmember;
                int fileCount = NativeMethods.DragQueryFile(new HandleRef(this, dropHandle), -1, null, 0);
                files = new string[fileCount];
                for (int x = 0; x < fileCount; ++x)
                {
                    int size = NativeMethods.DragQueryFile(new HandleRef(this, dropHandle), x, null, 0);
                    if (size > 0)
                    {
                        StringBuilder fileName = new StringBuilder(size + 1);
                        if (NativeMethods.DragQueryFile(new HandleRef(this, dropHandle), x, fileName, fileName.Capacity) > 0)
                            files[x] = fileName.ToString();
                    }
                }
            }
            finally
            {
                NativeMethods.ReleaseStgMedium(ref medium);
            }

            foreach (string file in files)
            {
                Client.OpenGLContextThread.Add(() => 
                {
                    ImportExportUtil.Import(file);
                });
            }
        }
    }
}