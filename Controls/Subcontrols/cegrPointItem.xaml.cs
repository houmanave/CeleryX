using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for cegrPointItem.xaml
    /// </summary>
    public partial class cegrPointItem : UserControl
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
                Canvas.SetLeft(this, point.X - OFFSETVALUE);
                Canvas.SetTop(this, point.Y - OFFSETVALUE);
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

        private int index;
        private bool isSelected;

        public cegrPointItem(Point p, int idx)
        {
            InitializeComponent();

            point = p;
            index = idx;
            IsSelected = false;

            Canvas.SetTop(this, p.X - OFFSETVALUE);
            Canvas.SetLeft(this, p.Y - OFFSETVALUE);
        }
    }
}
