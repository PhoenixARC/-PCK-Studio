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

namespace PckStudio.Features
{
    public partial class PckManager : MetroFramework.Forms.MetroForm
    {
        private Control currentlyShowingControl;
        private const string CemU = "Cemu";

        // TODO: Implement these Panels
        private const string WiiU = "Wii U";
        private const string PS3 = "Play Station 3";
        private const string PSVita = "PS Vita";
        private const string RPCS3 = "RPCS3";

        public PckManager()
        {
            InitializeComponent();
            supportedPlatformComboBox.Items.AddRange(new string[]
            {
                // WiiU,
                // PS3,
                // PSVita,
                CemU,
                // RPCS3,
            });
        }

        protected override void OnGotFocus(EventArgs e)
        {
            currentlyShowingControl?.Focus();
        }

        private void supportedPlatformComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mainPanel.Controls.Remove(currentlyShowingControl);
            if (supportedPlatformComboBox.SelectedIndex > -1)
            {
                string text = supportedPlatformComboBox.Items[supportedPlatformComboBox.SelectedIndex].ToString();
                try
                {
                    currentlyShowingControl = text switch
                    {
                        CemU => new CemuPanel(),
                        //WiiU => new WiiUPanel(),
                        //PS3 => throw new NotImplementedException($"{text}-Panel is currently not implemented."),
                        //PSVita => throw new NotImplementedException($"{text}-Panel is currently not implemented."),
                        //RPCS3 => throw new NotImplementedException($"{text}-Panel is currently not implemented."),
                        _ => throw new Exception($"No Panel found for: {text}"),
                    };
                    currentlyShowingControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                    mainPanel.SetColumnSpan(currentlyShowingControl, 2);
                    mainPanel.Controls.Add(currentlyShowingControl, 0, 1);
                }
                catch (NotImplementedException ex)
                {
                    MessageBox.Show(ex.Message, "Not Implemented");
                }

            }
        }
    }
}