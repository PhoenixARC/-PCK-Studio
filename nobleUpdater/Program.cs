using System;
using System.Windows.Forms;

namespace nobleUpdater
{
	internal static class Program
	{
		[global::System.STAThread]
		private static void Main()
		{

			global::System.Windows.Forms.Application.EnableVisualStyles();
			global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
			global::System.Windows.Forms.Application.Run(new global::nobleUpdater.FormMain());
		}
	}
}
