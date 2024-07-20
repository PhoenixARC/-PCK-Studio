/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1.The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

using PckStudio.Extensions;
using PckStudio.Internal;
using System.Drawing;
using AnimatedGif;
using System.Drawing.Imaging;

namespace PckStudio.ToolboxItems
{
	internal class AnimationPictureBox : BlendPictureBox
    {
        public bool IsPlaying => _isPlaying;
        
        private bool _isPlaying;

        public new Image Image
        {
            get => base.Image;
            set
            {
                base.Image = value;
                this.Animate(false);
                if (value is null)
                    return;
                value.SelectActiveFrame(new FrameDimension(value.FrameDimensionsList[0]), 0);
            }
        }

        protected override void OnPaint(PaintEventArgs paintEventArgs)
        {
            base.OnPaint(paintEventArgs);
            if (!_isPlaying && this.IsAnimating())
            {
                Stop();
            }
        }

        public void Start()
		{
            _isPlaying = true;
            this.Animate(_isPlaying);
        }

        public void Stop()
		{
            _isPlaying = false;
            this.Animate(_isPlaying);
        }

		protected override void Dispose(bool disposing)
		{
			Stop();
			base.Dispose(disposing);
		}
	}
}
