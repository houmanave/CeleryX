using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Celery.Controls.SubcontrolClasses;
using System.Windows;

namespace Celery.Controls.Subcontrols
{
    /// <summary>
    /// Interaction logic for cegrPointItemThumb.xaml
    /// </summary>
    public partial class cegrPointItemThumb : Thumb
    {
        private double OFFSETVALUE = 5.0;

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
                //Canvas.SetLeft(this, point.X - OFFSETVALUE);
                //Canvas.SetTop(this, point.Y - OFFSETVALUE);
            }
        }

        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        public double XValue { get; private set; }
        public double YValue { get; private set; }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
            }
        }

        public double CanvasWidth { get; set; }
        public double CanvasHeight { get; set; }

        public iteminfotext CoordText
        {
            get
            {
                return coordText;
            }

            set
            {
                coordText = value;
            }
        }

        private Controls.SubcontrolClasses.iteminfotext coordText;

        private int index;
        private bool isSelected;

        public Brush ColorSelected
        {
            get
            {
                return Brushes.Orange;
            }
        }
            
        public Brush ColorDeselected
        {
            get
            {
                return Brushes.AliceBlue;
            }
        }

        public cegrPointItemThumb(Point p, int idx, double x, double y, string headerX, string headerY)
        {
            InitializeComponent();

            Point = p;
            Index = idx;
            XValue = x;
            YValue = y;

            CanvasHeight = 488.0;
            CanvasWidth = 488.0;

            Canvas.SetLeft(this, p.X - OFFSETVALUE);
            Canvas.SetTop(this, p.Y - OFFSETVALUE);

            //  reset point
            Point np = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            CoordText = new Controls.SubcontrolClasses.iteminfotext(np, CanvasWidth, CanvasHeight, XValue, YValue, headerX, headerY);
            CoordText.Visibility = Visibility.Hidden;
        }
    }
}
