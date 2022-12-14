using CBH_Ultimate_Theme_Library.Theme.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CBH.Controls
{
    #region CrEaTiiOn_ModernButton

    public class CrEaTiiOn_ModernButton : Button
    {
        private Graphics G;

        private Helpers.MouseState State;

        private Schemes SchemeClone = Schemes.Black;
        public Schemes Scheme
        {
            get { return SchemeClone; }
            set
            {
                SchemeClone = value;
                Invalidate();
            }
        }

        public enum Schemes : byte
        {
            Black = 0,
            Green = 1,
            Red = 2,
            Blue = 3
        }

        public CrEaTiiOn_ModernButton()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            G = e.Graphics;

            base.OnPaint(e);

            G.Clear(Parent.BackColor);

            switch (Scheme)
            {

                case Schemes.Black:


                    if (Enabled)
                    {

                        if (State == Helpers.MouseState.None)
                        {
                            using (SolidBrush Background = new SolidBrush(Color.FromArgb(20, 20, 20)))
                            {
                                using (Pen Border = new Pen(Color.FromArgb(250, 36, 38)))
                                {
                                    G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                }
                            }


                        }
                        else if (State == Helpers.MouseState.Over)
                        {
                            using (SolidBrush Background = new SolidBrush(Color.FromArgb(58, 58, 57)))
                            {
                                using (Pen Border = new Pen(Color.FromArgb(46, 46, 45)))
                                {
                                    G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                }
                            }


                        }
                        else if (State == Helpers.MouseState.Down)
                        {
                            using (SolidBrush Background = new SolidBrush(Color.FromArgb(50, 50, 49)))
                            {
                                using (Pen Border = new Pen(Color.FromArgb(38, 38, 37)))
                                {
                                    G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                    G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                }
                            }

                        }


                    }
                    else
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(40, 40, 39)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(38, 38, 37)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }

                    }

                    break;
                case Schemes.Green:


                    if (State == Helpers.MouseState.None)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(123, 164, 93)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(119, 160, 89)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }


                    }
                    else if (State == Helpers.MouseState.Over)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(127, 168, 97)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(123, 164, 93)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }


                    }
                    else if (State == Helpers.MouseState.Down)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(119, 160, 93)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(115, 156, 85)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }

                    }

                    break;
                case Schemes.Red:


                    if (State == Helpers.MouseState.None)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(164, 93, 93)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(160, 89, 89)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }


                    }
                    else if (State == Helpers.MouseState.Over)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(168, 97, 97)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(164, 93, 93)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }


                    }
                    else if (State == Helpers.MouseState.Down)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(160, 89, 89)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(156, 85, 85)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }

                    }

                    break;
                case Schemes.Blue:


                    if (State == Helpers.MouseState.None)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(93, 154, 164)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(89, 150, 160)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }


                    }
                    else if (State == Helpers.MouseState.Over)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(97, 160, 168)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(93, 154, 164)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }


                    }
                    else if (State == Helpers.MouseState.Down)
                    {
                        using (SolidBrush Background = new SolidBrush(Color.FromArgb(89, 150, 160)))
                        {
                            using (Pen Border = new Pen(Color.FromArgb(85, 146, 156)))
                            {
                                G.FillPath(Background, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                                G.DrawPath(Border, Helpers.RoundRect(new Rectangle(0, 0, Width - 1, Height - 1), 2));
                            }
                        }

                    }

                    break;
            }


            if (Scheme == Schemes.Black)
            {
                if (Enabled)
                {
                    using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(220, 220, 219)))
                    {
                        using (Font TextFont = new Font("Segoe UI", 9))
                        {
                            using (StringFormat SF = new StringFormat { Alignment = StringAlignment.Center })
                            {
                                G.DrawString(Text, TextFont, TextBrush, new Rectangle(0, Height / 2 - 9, Width, Height), SF);
                            }
                        }
                    }


                }
                else
                {
                    using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(140, 140, 139)))
                    {
                        using (Font TextFont = new Font("Segoe UI", 9))
                        {
                            using (StringFormat SF = new StringFormat { Alignment = StringAlignment.Center })
                            {
                                G.DrawString(Text, TextFont, TextBrush, new Rectangle(0, Height / 2 - 9, Width, Height), SF);
                            }
                        }
                    }

                }


            }
            else
            {
                if (!Enabled)
                {
                    Scheme = Schemes.Black;
                }

                using (SolidBrush TextBrush = new SolidBrush(Color.FromArgb(250, 250, 249)))
                {
                    using (Font TextFont = new Font("Segoe UI", 9))
                    {
                        using (StringFormat SF = new StringFormat { Alignment = StringAlignment.Center })
                        {
                            G.DrawString(Text, TextFont, TextBrush, new Rectangle(0, Height / 2 - 9, Width, Height), SF);
                        }
                    }
                }

            }



        }

        protected override void OnMouseEnter(EventArgs e)
        {
            State = Helpers.MouseState.Over;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            State = Helpers.MouseState.None;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            State = Helpers.MouseState.Over;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            State = Helpers.MouseState.Down;
            Invalidate();
            base.OnMouseDown(e);
        }

    }

    #endregion
}
