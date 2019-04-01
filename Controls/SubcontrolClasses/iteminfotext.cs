using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Celery.Controls.SubcontrolClasses
{
    public class iteminfotext : _textbase
    {
        double maxwidth = 0.0;
        double maxheight = 0.0;

        public string TitleX { get; private set; }
        public string TitleY { get; private set; }

        public double OverU
        {
            get; set;
        }

        public double OverV
        {
            get; set;
        }

        double GAP = 8.0;

        public iteminfotext(Point p, double maxWidth, double maxHeight, double overrideU = double.NaN, double overrideV = double.NaN, string titleX = null, string titleY = null)
        {
            maxheight = MaxHeight;
            maxwidth = maxWidth;

            OverU = overrideU;
            OverV = overrideV;

            TitleX = (string.IsNullOrEmpty(titleX) ? "x" : titleX);
            TitleY = (string.IsNullOrEmpty(titleY) ? "y" : titleY);

            ObservableText = string.Format(" {0} = {1:0.000}, {2} = {3:0.000} ", TitleX, (double.IsNaN(OverU)) ? p.X : OverU, TitleY, (double.IsNaN(OverV)) ? p.Y : OverV);
            Size thissize = MeasureString(ObservableText);

            double px = p.X + GAP * 2;
            double py = p.Y - thissize.Height;

            if ((thissize.Width + GAP) > (maxwidth - p.X))
            {
                px = px - thissize.Width - GAP * 4;
            }

            if ((thissize.Height + GAP) > (maxheight - p.Y))
            {
                py = p.Y + GAP;
            }
            if ((p.Y - (thissize.Height)) <= 0)
            {
                py = p.Y + thissize.Height;
            }

            Canvas.SetLeft(this, px);
            Canvas.SetTop(this, py);

            Canvas.SetZIndex(this, 80);

            this.Background = Brushes.White;
        }

        protected Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                this.FontSize,
                System.Windows.Media.Brushes.Black);

            return new Size((int)formattedText.Width, (int)formattedText.Height);
        }
    }
}
