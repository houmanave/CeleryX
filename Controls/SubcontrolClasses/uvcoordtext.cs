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
    public class uvcoordtext : _textbase, IDisposable
    {
        //Point uvpoint;
        double maxwidth = 0.0;
        double maxheight = 0.0;
        double u = -1.0;
        double v = -1.0;

        public double OverU
        {
            get; set;
        }

        public double OverV
        {
            get; set;
        }

        int DECIMALPLACE = 3;

        double GAP = 4.0;

        public uvcoordtext(Point p, double maxWidth, double maxHeight)
        {
            maxwidth = maxWidth;
            maxheight = maxHeight;

            OverU = -1.0;
            OverV = -1.0;

            Regenerate(p);
        }

        public override void Regenerate(Point p)
        {
            ComputeUV(p);

            this.Text = string.Format("({0:0.000},{1:0.000})", (OverU < 0) ? u : OverU, (OverV < 0) ? v : OverV);
            Size thissize = MeasureString(this.Text);

            double px = p.X + GAP;
            double py = p.Y - thissize.Height - GAP;

            if ((thissize.Width + GAP) > (maxwidth - p.X))
            {
                px = px - thissize.Width - GAP * 2;
            }

            if ((thissize.Height + GAP) > p.Y)
            {
                py = p.Y + GAP;
            }

            Canvas.SetLeft(this, px);
            Canvas.SetTop(this, py);

            Canvas.SetZIndex(this, 80);
        }

        private void ComputeUV(Point p)
        {
            u = p.X / maxwidth;
            v = 1.0 - (p.Y / maxheight);

            u = Math.Round(u, DECIMALPLACE);
            v = Math.Round(v, DECIMALPLACE);
        }

        protected Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, this.FontStretch),
                this.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public void Dispose()
        {
        }
    }
}
