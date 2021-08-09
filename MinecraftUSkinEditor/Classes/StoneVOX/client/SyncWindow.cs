using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace stonevox
{
    public class SyncWindow : GameWindow, IGameWindow, INativeWindow, IDisposable
    {

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public int targetFps;

        public SyncWindow(int width, int height, GraphicsMode mode)
            : base(width, height, mode)
        {
        }

        public void Run_WithErrorCatching(int targetFps)
        {
            this.targetFps = targetFps;
            base.Visible = true;
            try
            {
                TargetUpdateFrequency = this.targetFps;
                TargetRenderFrequency = this.targetFps;

                FrameEventArgs updateArgs = new FrameEventArgs();
                FrameEventArgs renderArgs = new FrameEventArgs();

                OnLoad(EventArgs.Empty);
                OnResize(EventArgs.Empty);

                Stopwatch stopWatch = Stopwatch.StartNew();

                int[] sleepTimes = new int[15];
                for (int i = 0; i < sleepTimes.Length; i++) sleepTimes[i] = 1000 / this.targetFps;
                int frameSleepTime = 0;

                int frames = 0;
                double previousElapsedSeconds = 0;
                while (true)
                {
                    frames++;

                    double totalElapsedSeconds = stopWatch.Elapsed.TotalSeconds;
                    double frameElapsedSeconds = totalElapsedSeconds - previousElapsedSeconds;
                    previousElapsedSeconds = totalElapsedSeconds;

                    if (totalElapsedSeconds >= 0.25)
                    {
                        double fps = frames / totalElapsedSeconds;

                        if (fps < this.targetFps)
                        {
                            int max = 0;
                            for (int i = 1; i < sleepTimes.Length; i++)
                            {
                                if (sleepTimes[i] > sleepTimes[max]) max = i;
                            }
                            sleepTimes[max] = System.Math.Max(0, sleepTimes[max] - 1);
                        }
                        else
                        {
                            int min = 0;
                            for (int i = 1; i < sleepTimes.Length; i++)
                            {
                                if (sleepTimes[i] < sleepTimes[min]) min = i;
                            }
                            sleepTimes[min] += 1;
                        }

                        stopWatch.Reset();
                        stopWatch.Start();
                        frames = 0;
                        previousElapsedSeconds = 0;
                    }

                    ProcessEvents();

                    updateArgs = new FrameEventArgs(frameElapsedSeconds);
                    this.OnUpdateFrame(updateArgs);

                    renderArgs = new FrameEventArgs(frameElapsedSeconds);
                    this.OnRenderFrame(renderArgs);

                    System.Threading.Thread.Sleep(sleepTimes[frameSleepTime]);
                    frameSleepTime = (frameSleepTime + 1) % sleepTimes.Length;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("");
                Console.WriteLine("Exception Caught - exiting main loop.");

                string stacktrace = ex.ToString().ToLower();
                stacktrace = stacktrace.Replace("c:\\users\\daniel\\documents\\github\\stonevox3d\\", "");

                Console.WriteLine(stacktrace);
                Console.WriteLine("");
                SetForegroundWindow(GetConsoleWindow());
                var result = MessageBox.Show("Would you like to copy crash info to the clipboard?", "StoneVox Encountered An Error", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Clipboard.SetText(stacktrace);
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("");
                Console.WriteLine("Crash info copied to clipboard.");
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally
            {
                Debug.Print("Restoring priority.");
                Thread.CurrentThread.Priority = ThreadPriority.Normal;

                OnUnload(EventArgs.Empty);

                if (Exists)
                {
                    Context.Dispose();
                }
                while (this.Exists)
                    this.ProcessEvents();
            }
        }

        public void Run_NoErrorCatching(int targetFps)
        {
            this.targetFps = targetFps;
            base.Visible = true;
            while (!IsExiting)
            {
                TargetUpdateFrequency = this.targetFps;
                TargetRenderFrequency = this.targetFps;

                FrameEventArgs updateArgs = new FrameEventArgs();
                FrameEventArgs renderArgs = new FrameEventArgs();

                OnLoad(EventArgs.Empty);
                OnResize(EventArgs.Empty);

                Stopwatch stopWatch = Stopwatch.StartNew();

                int[] sleepTimes = new int[15];
                for (int i = 0; i < sleepTimes.Length; i++) sleepTimes[i] = 1000 / this.targetFps;
                int frameSleepTime = 0;

                int frames = 0;
                double previousElapsedSeconds = 0;
                while (true)
                {
                    frames++;

                    double totalElapsedSeconds = stopWatch.Elapsed.TotalSeconds;
                    double frameElapsedSeconds = totalElapsedSeconds - previousElapsedSeconds;
                    previousElapsedSeconds = totalElapsedSeconds;

                    if (totalElapsedSeconds >= 0.25)
                    {
                        double fps = frames / totalElapsedSeconds;

                        if (fps < this.targetFps)
                        {
                            int max = 0;
                            for (int i = 1; i < sleepTimes.Length; i++)
                            {
                                if (sleepTimes[i] > sleepTimes[max]) max = i;
                            }
                            sleepTimes[max] = System.Math.Max(0, sleepTimes[max] - 1);
                        }
                        else
                        {
                            int min = 0;
                            for (int i = 1; i < sleepTimes.Length; i++)
                            {
                                if (sleepTimes[i] < sleepTimes[min]) min = i;
                            }
                            sleepTimes[min] += 1;
                        }

                        stopWatch.Reset();
                        stopWatch.Start();
                        frames = 0;
                        previousElapsedSeconds = 0;
                    }

                    ProcessEvents();

                    updateArgs = new FrameEventArgs(frameElapsedSeconds);
                    this.OnUpdateFrame(updateArgs);

                    renderArgs = new FrameEventArgs(frameElapsedSeconds);
                    this.OnRenderFrame(renderArgs);

                    System.Threading.Thread.Sleep(sleepTimes[frameSleepTime]);
                    frameSleepTime = (frameSleepTime + 1) % sleepTimes.Length;
                }
            }

            Thread.CurrentThread.Priority = ThreadPriority.Normal;

            OnUnload(EventArgs.Empty);

            if (Exists)
            {
                Context.Dispose();
            }
            while (this.Exists)
                this.ProcessEvents();
        }
    }
}