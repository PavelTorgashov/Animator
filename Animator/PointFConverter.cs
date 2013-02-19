using System;
using System.ComponentModel;
using System.Drawing;

namespace AnimatorNS
{
    /// <summary>
    /// PointFConverter
    /// Thanks for Jay Riggs
    /// </summary>
    public class PointFConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Creates a new instance of PointFConverter
        /// </summary>
        public PointFConverter()
        {
        }

        /// <summary>
        /// Boolean, true if the source type is a string
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the specified string into a PointF
        /// </summary>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string s = (string)value;
                    string[] converterParts = s.Split(',');
                    float x = 0;
                    float y = 0;
                    if (converterParts.Length > 1)
                    {
                        x = float.Parse(converterParts[0].Trim().Trim('{', 'X', 'x','='));
                        y = float.Parse(converterParts[1].Trim().Trim('}', 'Y', 'y','='));
                    }
                    else if (converterParts.Length == 1)
                    {
                        x = float.Parse(converterParts[0].Trim());
                        y = 0;
                    }
                    else
                    {
                        x = 0F;
                        y = 0F;
                    }
                    return new PointF(x, y);
                }
                catch
                {
                    throw new ArgumentException("Cannot convert [" + value.ToString() + "] to pointF");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the PointF into a string
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value.GetType() == typeof(PointF))
                {
                    PointF pt = (PointF)value;
                    return string.Format("{{X={0}, Y={1}}}", pt.X, pt.Y);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
