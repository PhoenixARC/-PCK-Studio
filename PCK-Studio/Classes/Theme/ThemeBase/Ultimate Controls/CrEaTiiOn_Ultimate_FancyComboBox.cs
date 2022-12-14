using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CBH.Ultimate.Controls
{
    [DefaultEvent("OnSelectedIndexChanged")]
    public sealed class CrEaTiiOn_Ultimate_FancyComboBox : UserControl
    {
        private Color _backColor = Color.FromArgb(20, 20, 20);
        private Color _iconColor = Color.White;
        private Color _listBackColor = Color.FromArgb(15, 15, 15);
        private Color _listForeColor = Color.White;
        private Color _borderColor = Color.FromArgb(250, 36, 38);
        private int _borderSize = 1;

        private ComboBox _comboList;
        private Label _labelString;
        private Button _buttonIcon;

        [Category("CrEaTiiOn")]
        public new Color BackColor { get => _backColor; set { _backColor = value; _labelString.BackColor = _backColor; _buttonIcon.BackColor = _backColor; } }
        [Category("CrEaTiiOn")]
        public Color IconColor { get => _iconColor; set { _iconColor = value; _buttonIcon.Invalidate(); } }
        [Category("CrEaTiiOn")]
        public Color ListBackColor { get => _listBackColor; set { _listBackColor = value; _comboList.BackColor = _listBackColor; } }
        [Category("CrEaTiiOn")]
        public Color ListForeColor { get => _listForeColor; set { _listForeColor = value; _comboList.ForeColor = _listForeColor; } }
        [Category("CrEaTiiOn")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; base.BackColor = _borderColor; } }
        [Category("CrEaTiiOn")]
        public int BorderSize { get => _borderSize; set { _borderSize = value; Padding = new Padding(_borderSize); AdjustComboBoxDimensions(); } }
        [Category("CrEaTiiOn")]
        public override Color ForeColor { get => base.ForeColor; set { base.ForeColor = value; _labelString.ForeColor = value; } }
        [Category("CrEaTiiOn")]
        public override Font Font { get => base.Font; set { base.Font = value; _labelString.Font = value; _comboList.Font = value; } }
        [Category("CrEaTiiOn")]
        public string String { get => _labelString.Text; set => _labelString.Text = value; }
        [Category("CrEaTiiOn")]
        public ComboBoxStyle DropDownStyle { get => _comboList.DropDownStyle; set { if(_comboList.DropDownStyle != ComboBoxStyle.Simple) _comboList.DropDownStyle = value; } }

        [Category("CrEaTiiOn")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Localizable(true)]
        [MergableProperty(false)]
        public ComboBox.ObjectCollection Items => _comboList.Items;

        [Category("CrEaTiiOn")]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue(null)]
        public object DataSource { get => _comboList.DataSource; set => _comboList.DataSource = value; }
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Localizable(true)]
        public AutoCompleteStringCollection AutoCompleteCustomSource { get => _comboList.AutoCompleteCustomSource; set => _comboList.AutoCompleteCustomSource = value; }
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteSource.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteSource AutoCompleteSource { get => _comboList.AutoCompleteSource; set => _comboList.AutoCompleteSource = value; }
        [Category("CrEaTiiOn")]
        [Browsable(true)]
        [DefaultValue(AutoCompleteMode.None)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteMode AutoCompleteMode { get => _comboList.AutoCompleteMode; set => _comboList.AutoCompleteMode = value; }
        [Category("CrEaTiiOn")]
        [Bindable(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedItem { get => _comboList.SelectedItem; set => _comboList.SelectedItem = value; }
        [Category("CrEaTiiOn")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedIndex { get => _comboList.SelectedIndex; set => _comboList.SelectedIndex = value; }
        [Category("CrEaTiiOn")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string DisplayMember { get => _comboList.DisplayMember; set => _comboList.DisplayMember = value; }
        [Category("CrEaTiiOn")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember { get => _comboList.ValueMember; set => _comboList.ValueMember = value; }

        public event EventHandler OnSelectedIndexChanged;

        public CrEaTiiOn_Ultimate_FancyComboBox()
        {
            _comboList = new ComboBox();
            _labelString = new Label();
            _buttonIcon = new Button();
            SuspendLayout();

            _comboList.BackColor = ListBackColor;
            _comboList.Font = new Font(Font.Name, 10f);
            _comboList.ForeColor = ListForeColor;
            _comboList.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            _comboList.TextChanged += ComboBox_StringChanged;

            _buttonIcon.Dock = DockStyle.Right;
            _buttonIcon.FlatStyle = FlatStyle.Flat;
            _buttonIcon.FlatAppearance.BorderSize = 0;
            _buttonIcon.BackColor = BackColor;
            _buttonIcon.Size = new Size(30, 30);
            _buttonIcon.Cursor = Cursors.Hand;
            _buttonIcon.Click += Icon_Click;
            _buttonIcon.Paint += Icon_Paint;

            _labelString.Dock = DockStyle.Fill;
            _labelString.AutoSize = false;
            _labelString.BackColor = BackColor;
            _labelString.TextAlign = ContentAlignment.MiddleLeft;
            _labelString.Padding = new Padding(8, 0, 0, 0);
            _labelString.Font = new Font(Font.Name, 10f);
            _labelString.Click += Surface_Click;
            _labelString.MouseEnter += Surface_MouseEnter;
            _labelString.MouseLeave += Surface_MouseLeave;

            Controls.Add(_labelString);
            Controls.Add(_buttonIcon);
            Controls.Add(_comboList);
            MinimumSize = new Size(200, 30);
            Size = new Size(200, 30);
            ForeColor = Color.DimGray;
            Padding = new Padding(BorderSize);
            base.BackColor = BorderColor;
            ResumeLayout();
            AdjustComboBoxDimensions();
        }

        private void Surface_MouseLeave(object sender, EventArgs e)
        {
            OnMouseLeave(e);
        }

        private void Surface_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void AdjustComboBoxDimensions()
        {
            _comboList.Width = _labelString.Width;
            _comboList.Location = new Point(Width - Padding.Right - _comboList.Width, _labelString.Bottom - _comboList.Height);
        }

        private void Surface_Click(object sender, EventArgs e)
        {
            OnClick(e);
            _comboList.Select();
            if (_comboList.DropDownStyle == ComboBoxStyle.DropDownList)
                _comboList.DroppedDown = true;
        }

        private void Icon_Paint(object sender, PaintEventArgs e)
        {
            int iconWidth = 14;
            int iconHeight = 6;
            var rectIcon = new Rectangle((_buttonIcon.Width - iconWidth) / 2, (_buttonIcon.Height - iconHeight) / 2, iconWidth, iconHeight);
            Graphics graphics = e.Graphics;

            using(GraphicsPath path = new GraphicsPath())
            using (Pen pen = new Pen(IconColor, 2))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                path.AddLine(rectIcon.X, rectIcon.Y, rectIcon.X + (iconWidth / 2), rectIcon.Bottom);
                path.AddLine(rectIcon.X + (iconWidth / 2), rectIcon.Bottom, rectIcon.Right, rectIcon.Y);
                graphics.DrawPath(pen, path);
            }
        }

        private void Icon_Click(object sender, EventArgs e)
        {
            _comboList.Select();
            _comboList.DroppedDown = true;
        }

        private void ComboBox_StringChanged(object sender, EventArgs e)
        {
            _labelString.Text = _comboList.Text;
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged.Invoke(sender, e);
            _labelString.Text = _comboList.Text;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustComboBoxDimensions();
        }
    }
}
