using System;
using System.ComponentModel;
using System.Drawing;
using PckStudio.Core.Extensions;

namespace PckStudio.ToolboxItems
{
    internal class BlendPictureBox : InterpolationPictureBox
    {
        [DefaultValue(false)]
        [Category("Blending")]
        public bool UseBlendColor
        {
            get => _useBlendColor;
            set => _useBlendColor = value;
        }

        [DefaultValue(typeof(Color), "Color.White")]
        [Category("Blending")]
        public Color BlendColor
        {
            get => _blendColor;
            set
            {
                _blendColor = value;
                Image = _image;
            }
        }
        

        [DefaultValue(typeof(BlendMode), "BlendMode.Add")]
        [Category("Blending")]
        public BlendMode BlendMode
        {
            get => _blendMode;
            set => _blendMode = value;
        }

        private bool _useBlendColor = false;
        private Color _blendColor = Color.White;
        private Image _image;
        private BlendMode _blendMode = BlendMode.Add;

        public new Image Image
        {
            get => base.Image;
            set {
                if (value is null)
                    return;
                _image = value;
                base.Image = UseBlendColor && BlendColor != Color.White ? _image.Blend(BlendColor, BlendMode) : _image;
                Invalidate();
            }
        }
    }
}
