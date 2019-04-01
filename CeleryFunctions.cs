using Autodesk.DesignScript.Runtime;
//using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Celery
{
    [IsVisibleInDynamoLibrary(false)]
    public class CeleryFunctions
    {
        public static double CeleryTangent(double angle)
        {
            //return Trig.Tan(angle);
            return angle * angle;
        }

        /// <summary>
        /// To be used for serialization
        /// </summary>
        /// <param name="p">A point</param>
        /// <returns>A string of point coordinates.</returns>
        public static string ConvertFromPointToString(Point p)
        {
            return p.X.ToString() + "," + p.Y.ToString();
        }

        /// <summary>
        /// TO be used for deserialization
        /// </summary>
        /// <param name="value">A string of point coordinates.</param>
        /// <returns>The converted point.</returns>
        public static Point ConvertFromStringToPoint(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                string[] strspl = value.Split(',');
                if (strspl.Length == 2)
                {
                    double x = Convert.ToDouble(strspl[0]);
                    double y = Convert.ToDouble(strspl[1]);
                    return new Point(x, y);
                }
            }

            return new Point(0, 0);
        }

        public static Size MeasureString(string candidate, TextBlock tb = null)
        {
            Typeface tf = new Typeface("Arial");
            if (tb != null)
            {
                tf = new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
            }
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                tf,
                8,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public static FormattedText FormattedString(string candidate, TextBlock tb = null)
        {
            Typeface tf = new Typeface("Arial");
            if (tb != null)
            {
                tf = new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch);
            }
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                tf,
                8,
                Brushes.Black);

            return formattedText;
        }
    }
}
