/*
 * Copyright (c) 2025 EternalModz
 * 
 * This software is freely available for use, modification, and distribution.
 * 
 * Terms of Use:
 * 1. Credit must be given to EternalModz in any project using this software.
 * 2. This software may not be sold or used for commercial purposes without explicit permission.
 * 3. Redistributions must include this copyright notice.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY.
 */

using CBH.Ultimate.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PckStudio.Classes.FormColoring
{
    /// <summary>
    /// Provides methods to load, save, and apply color settings for the button borders.
    /// </summary>
    public static class ColorSettingsManager
    {
        private static readonly string FilePath = Path.Combine(Application.StartupPath, "color_settings.cfg");

        public static void LoadColorSettings(Form form)
        {
            if (!File.Exists(FilePath)) return;

            foreach (var line in File.ReadLines(FilePath))
            {
                var parts = line.Split('=');
                if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int argbValue))
                {
                    ApplyColorSetting(form, parts[0].Trim(), Color.FromArgb(argbValue));
                }
            }
        }

        public static void SaveColorSettings(Color primaryColor, Color secondaryColor)
        {
            File.WriteAllLines(FilePath, new[]
            {
                $"GradientColorPrimary={primaryColor.ToArgb()}",
                $"GradientColorSecondary={secondaryColor.ToArgb()}"
            });
        }

        private static void ApplyColorSetting(Control control, string propertyName, Color color)
        {
            var property = control.GetType().GetProperty(propertyName);
            property?.SetValue(control, color);

            if (control is CrEaTiiOn_Ultimate_GradientButton gradientButton)
            {
                switch (propertyName)
                {
                    case "HoverColor": gradientButton.HoverOverColor = color; break;
                    case "ClickColor": gradientButton.ClickedColor = color; break;
                    case "GradientColorPrimary": gradientButton.GradientColorPrimary = color; break;
                    case "GradientColorSecondary": gradientButton.GradientColorSecondary = color; break;
                }
            }

            foreach (Control child in control.Controls)
            {
                ApplyColorSetting(child, propertyName, color);
            }
        }

        public static void ApplyColorSettings(Form form, Color primaryColor, Color secondaryColor)
        {
            ApplyControlColorSettings(form, "GradientColorPrimary", primaryColor);
            ApplyControlColorSettings(form, "GradientColorSecondary", secondaryColor);

            foreach (Form childForm in form.OwnedForms)
            {
                ApplyControlColorSettings(childForm, "GradientColorPrimary", primaryColor);
                ApplyControlColorSettings(childForm, "GradientColorSecondary", secondaryColor);
            }
        }

        private static void ApplyControlColorSettings(Control control, string propertyName, Color color)
        {
            control.GetType().GetProperty(propertyName)?.SetValue(control, color);
            foreach (Control child in control.Controls)
            {
                ApplyControlColorSettings(child, propertyName, color);
            }
        }
    }
}