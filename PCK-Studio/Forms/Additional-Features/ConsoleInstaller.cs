/* Copyright (c) 2022-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Windows.Forms;
namespace PckStudio.Forms.Additional_Features
{
    public partial class ConsoleInstaller : MetroFramework.Forms.MetroForm
    {
        private Control shownControl;

        public ConsoleInstaller()
        {
            InitializeComponent();
        }

        private void selectedConsoleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Remove previous shown Control
            mainPanel.Controls.Remove(shownControl);
            if (selectedConsoleComboBox.SelectedIndex > -1)
            {
                string text = selectedConsoleComboBox.Items[selectedConsoleComboBox.SelectedIndex].ToString();
                shownControl = text switch
                {
                    "Wii U" => new WiiUInstallPanel(),
                    "Play Station 3" => new PS3InstallPanel(),
                    "PS Vita" => throw new NotImplementedException($"{text}-Installer is currently not implemented."),
                    "Cemu" => new CemuInstallPanel(),
                    "RPCS3" => throw new NotImplementedException($"{text}-Installer is currently not implemented."),
                    _ => throw new Exception($"No Control found for: {text}"),
                };

                shownControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

                mainPanel.SetColumnSpan(shownControl, 2);
                mainPanel.Controls.Add(shownControl, 0, 1);
            }
        }
    }
}