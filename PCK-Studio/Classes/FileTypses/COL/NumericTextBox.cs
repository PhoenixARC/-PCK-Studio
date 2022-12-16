using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace ColorPicker
{
    /// <summary>A TextBox that only acceptes numbers</summary>
    public class NumericTextBox : TextBox
    {
        #region Private Fields

        private bool _allowDecimal = true;
        private bool _allowNull = true;
        private bool _allowSign = true;
        private bool _allowSpace = false;
        private string _format = string.Empty;
        private NumberFormatInfo _numberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;

        #endregion Private Fields

        #region Constructors

        /// <summary>Constructor</summary>
        public NumericTextBox()
            : base()
        {
            // By default right alignment
            TextAlign = HorizontalAlignment.Right;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>Decimal point allowed</summary>
        [DefaultValue(true),
        Category("Attribute")]
        public bool AllowDecimal
        {
            get => _allowDecimal;
            set => _allowDecimal = value;
        }

        /// <summary>NULL value allowed</summary>
        [DefaultValue(true),
        Category("Attribute")]
        public bool AllowNull
        {
            get => _allowNull;
            set => _allowNull = value;
        }

        /// <summary>Sign allowed</summary>
        [DefaultValue(true),
        Category("Attribute")]
        public bool AllowSign
        {
            get => _allowSign;
            set => _allowSign = value;
        }

        /// <summary>Spaces allowed</summary>
        [DefaultValue(false),
        Category("Attribute")]
        public bool AllowSpace
        {
            get => _allowSpace;
            set => _allowSpace = value;
        }

        /// <summary>Decimal separator character</summary>
        public string DecimalSeparator => _numberFormatInfo.NumberDecimalSeparator;

        /// <summary>Value as a Decimal</summary>
        public decimal DecimalValue
        {
            get
            {
                try
                {
                    if (Text == null || Text.Trim() == string.Empty)
                        return 0M;
                    return decimal.Parse(Text, _numberFormatInfo);
                }
                catch (Exception)
                {
                    return 0M;
                }
            }
            set
            {
                if (_format != string.Empty)
                {
                    try
                    {
                        Text = value.ToString(_format, _numberFormatInfo);
                    }
                    catch (Exception)
                    {
                        Text = value.ToString(_numberFormatInfo);
                    }
                }
                else
                    Text = value.ToString(_numberFormatInfo);
            }
        }

        /// <summary>Values as a Double</summary>
        public double DoubleValue
        {
            get
            {
                try
                {
                    if (Text == null || Text.Trim() == string.Empty)
                        return 0.0;
                    return double.Parse(Text, _numberFormatInfo);
                }
                catch (Exception)
                {
                    return 0.0;
                }
            }
            set
            {
                if (_format != string.Empty)
                {
                    try
                    {
                        Text = value.ToString(_format, _numberFormatInfo);
                    }
                    catch (Exception)
                    {
                        Text = value.ToString(_numberFormatInfo);
                    }
                }
                else
                    Text = value.ToString(_numberFormatInfo);
            }
        }

        /// <summary>Number format string</summary>
        public string Format
        {
            get => _format;
            set
            {
                if (value != null)
                    _format = value;
                else
                    _format = string.Empty;
            }
        }

        /// <summary>Group separator character</summary>
        public string GroupSeparator => _numberFormatInfo.NumberGroupSeparator;

        /// <summary>Valor as an Int32</summary>
        public int Int32Value
        {
            get
            {
                try
                {
                    if (Text == null || Text.Trim() == string.Empty)
                        return 0;
                    return int.Parse(Text, _numberFormatInfo);
                }
                catch (Exception)
                {
                    try
                    {
                        return Convert.ToInt32(Math.Round(Decimal.Parse(Text, _numberFormatInfo), 0));
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            set
            {
                if (Int32Value != value || string.IsNullOrEmpty(Text)) // To not overwrite the value passed as Double or Decimal
                {
                    if (_format != string.Empty)
                    {
                        try
                        {
                            Text = value.ToString(_format, _numberFormatInfo);
                        }
                        catch (Exception)
                        {
                            Text = value.ToString(_numberFormatInfo);
                        }
                    }
                    else
                        Text = value.ToString(_numberFormatInfo);
                }
            }
        }

        /// <summary>Valor as an UInt32</summary>
        public uint UInt32Value
        {
            get
            {
                try
                {
                    if (Text == null || Text.Trim() == string.Empty)
                        return 0;
                    return uint.Parse(Text, _numberFormatInfo);
                }
                catch (Exception)
                {
                    try
                    {
                        return Convert.ToUInt32(Math.Round(Decimal.Parse(Text, _numberFormatInfo), 0));
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            set
            {
                if (UInt32Value != value || string.IsNullOrEmpty(Text)) // To not overwrite the value passed as Double or Decimal
                {
                    if (_format != string.Empty)
                    {
                        try
                        {
                            Text = value.ToString(_format, _numberFormatInfo);
                        }
                        catch (Exception)
                        {
                            Text = value.ToString(_numberFormatInfo);
                        }
                    }
                    else
                        Text = value.ToString(_numberFormatInfo);
                }
            }
        }

        /// <summary>Negative sign character</summary>
        public string NegativeSign => _numberFormatInfo.NegativeSign;

        /// <summary>The NumberFormatInfo</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public NumberFormatInfo NumberFormatInfo
        {
            get => _numberFormatInfo;
            set
            {
                if (value != null)
                    _numberFormatInfo = value;
            }
        }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>Go the focus</summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            if (_format != string.Empty && !ReadOnly && (!_allowNull || Text.Trim() != string.Empty))
                Text = DecimalValue.ToString(_numberFormatInfo);
            base.OnGotFocus(e);
        }

        /// <summary>Restricts the entry of characters to digits, the negative sign, the decimal point, and editing keystrokes (backspace).</summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            string keyInput = e.KeyChar.ToString();
            if (!char.IsDigit(e.KeyChar) && // Digits are OK
                !((keyInput.Equals(_numberFormatInfo.NumberDecimalSeparator) || keyInput.Equals(_numberFormatInfo.NumberGroupSeparator)) && _allowDecimal) && // Decimal separator is OK
                !(keyInput.Equals(_numberFormatInfo.NegativeSign) && _allowSign) && // Sign is OK
                e.KeyChar != '\b' && // Backspace key is OK
                (ModifierKeys & (Keys.Control | Keys.Alt)) == 0 && // Let the edit control handle control and alt key combinations
                !(_allowSpace && e.KeyChar == ' ') // Space is OK
                )
            {
                // Swallow this invalid key
                e.Handled = true;
            }


            //if (char.IsDigit(e.KeyChar))
            //{
            //    // Digits are OK
            //}
            //else if ((keyInput.Equals(_numberFormatInfo.NumberDecimalSeparator) || keyInput.Equals(_numberFormatInfo.NumberGroupSeparator)) && _allowDecimal)
            //{
            //    // Decimal separator is OK
            //}
            //else if (keyInput.Equals(_numberFormatInfo.NegativeSign) && _allowSign)
            //{
            //    // Sign is OK
            //}
            //else if (e.KeyChar == '\b')
            //{
            //    // Backspace key is OK
            //}
            //else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            //{
            //    // Let the edit control handle control and alt key combinations
            //}
            //else if (_allowSpace && e.KeyChar == ' ')
            //{
            //    // Space is OK
            //}
            //else
            //{
            //    // Swallow this invalid key
            //    e.Handled = true;
            //}
        }

        /// <summary>Lost the focus</summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (_format != string.Empty && !ReadOnly && (!_allowNull || Text.Trim() != string.Empty))
            {
                try
                {
                    Text = DecimalValue.ToString(_format, _numberFormatInfo);
                }
                catch (Exception)
                {
                    Text = DecimalValue.ToString(_numberFormatInfo);
                }
            }
            base.OnLostFocus(e);
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>Serialize the TextAlign</summary>
        public void ResetTextAlign()
        {
            TextAlign = HorizontalAlignment.Right;
        }

        /// <summary>Serialize the TextAlign</summary>
        /// <returns></returns>
        public bool ShouldSerializeTextAlign()
        {
            //return this.TextAlign != HorizontalAlignment.Right;
            return true;
        }

        #endregion Public Methods
    }
}