using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PckStudio.Extensions;

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
            set => _blendColor = value;
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
        private BlendMode _blendMode = BlendMode.Add;

        public new Image Image
        {
            get => base.Image;
            set {
                if (value is null)
                    return;
                base.Image = UseBlendColor && BlendColor != Color.White ? value.Blend(BlendColor, BlendMode) : value;
            }
        }
    }
}
