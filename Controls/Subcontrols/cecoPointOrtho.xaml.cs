using Celery.Controls.SubcontrolClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Celery.Controls.Subcontrols
{
    /// <summary>
    /// Interaction logic for cecoPointOrtho.xaml
    /// </summary>
    public partial class cecoPointOrtho : Thumb
    {
        public bool IsPositive { get; set; }
        public bool IsVertical { get; set; }
        public double FixedX { get; set; }
        public double FixedY { get; set; }

        public double maxwidth { get; set; }
        public double maxheight { get; set; }

        public perlinnoisecurve perlcurve { get; set; }

        private Point point;
        public Point Point
        {
            get
            {
                return point;
            }

            set
            {
                point = value;
                Canvas.SetLeft(this, point.X - OFFSETVALUE);
                Canvas.SetTop(this, point.Y - OFFSETVALUE);
            }
        }

        private double OFFSETVALUE = 9;
        public cecoPointOrtho(Point pt, double fixedXValue, double fixedYValue, bool isVertical)
        {
            InitializeComponent();

            Point = pt;

            IsVertical = isVertical;
            FixedX = fixedXValue;
            FixedY = fixedYValue;
        }

        public override string ToString()
        {
            return Point.X.ToString() + "," + Point.Y.ToString() + "," + FixedX.ToString() + "," + FixedY.ToString() + "," + IsVertical.ToString();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double ehc = IsVertical ? FixedX : Canvas.GetLeft(this) + e.HorizontalChange + OFFSETVALUE;
            double evc = IsVertical ? Canvas.GetTop(this) + e.VerticalChange + OFFSETVALUE : FixedY;

            if (ehc < 0)
            {
                ehc = 0;
            }
            else if (ehc > maxwidth)
            {
                ehc = maxwidth;
            }

            if (evc < 0)
            {
                evc = 0;
            }
            else if (evc > maxheight)
            {
                evc = maxheight;
            }

            Point = new Point(ehc, evc);

            Canvas.SetLeft(this, ehc - OFFSETVALUE);
            Canvas.SetTop(this, evc - OFFSETVALUE);

            if (perlcurve != null)
            {
                perlcurve.Regenerate(this);
            }
        }
    }
}
