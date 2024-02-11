/*
 * 
 *The MIT License (MIT)

Copyright (c) 2016 Kareem Morsy

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
https://github.com/KareemMAX/Minecraft-Skiner
https://github.com/KareemMAX/Minecraft-Skiner/blob/master/src/Minecraft%20skiner/UserControls/Renderer3D.vb
 */

using System;
using System.Windows.Forms;
using OpenTK;
using PckStudio.Rendering.Camera;

namespace PckStudio.Rendering
{
    internal class Renderer3D : GLControl
    {
        /// <summary>
        /// Refresh rate at which the frame is updated. Default is 50(Hz)
        /// </summary>
        public int RefreshRate
        {
            get => refreshRate;
            set
            {
                refreshRate = Math.Max(value, 1);
                timer.Interval = TimeSpan.FromSeconds(1d / refreshRate).Milliseconds;
            }
        }

        protected PerspectiveCamera Camera;
        protected EventHandler OnTimerTick { get; set; }

        private int refreshRate = 50;
        private Timer timer;

        public Renderer3D() : base()
        {
            timer = new Timer();
            RefreshRate = refreshRate;
            timer.Tick += TimerTick;
            timer.Start();
            timer.Enabled = !DesignMode;
            VSync = true;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            OnTimerTick?.Invoke(sender, e);
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            MakeCurrent();
            if (Camera is not null)
            {
                Camera.ViewportSize = ClientSize;
                Camera.Update();
            }
            Renderer.SetViewportSize(Camera.ViewportSize);
        }
    }
}  
