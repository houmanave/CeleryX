using Celery.Controls.SubcontrolClasses;
using Dynamo.Graph.Nodes;
using Dynamo.UI;
using Dynamo.ViewModels;
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
    /// Interaction logic for cecoPointEnd.xaml
    /// </summary>
    public partial class cecoPointEnd : Thumb
    {
        private bool isstart = true;
        private double OFFSETVALUE = 7;

        //readonly NodeModel nodemodel;
        //private IViewModelView<NodeViewModel> ui;

        public controlline cline { get; set; }
        public beziercurve bcurve { get; set; }

        public linearcurve lcurve { get; set; }

        public double maxwidth { get; set; }
        public double maxheight { get; set; }

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

        public cecoPointEnd(Point p, /*NodeModel model, IViewModelView<NodeViewModel> nodeUI,*/ bool isStart = true)
        {
            InitializeComponent();

            //nodemodel = model;
            //ui = nodeUI;

            Point = p;

            Canvas.SetLeft(this, p.X - OFFSETVALUE);
            Canvas.SetTop(this, p.Y - OFFSETVALUE);

            isstart = isStart;
        }

        public override string ToString()
        {
            return Point.X.ToString() + "," + Point.Y.ToString() + "," + isstart.ToString();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double ehc = isstart ? 0 : maxwidth;//Canvas.GetLeft(this) + e.HorizontalChange + OFFSETVALUE;
            double evc = Canvas.GetTop(this) + e.VerticalChange + OFFSETVALUE;

            Update(ehc, evc);

            //if (ehc < 0)
            //{
            //    ehc = 0;
            //}
            //else if (ehc > maxwidth)
            //{
            //    ehc = maxwidth;
            //}

            //if (evc < 0)
            //{
            //    evc = 0;
            //}
            //else if (evc > maxheight)
            //{
            //    evc = maxheight;
            //}

            //Point = new Point(ehc, evc);

            //Canvas.SetLeft(this, ehc - OFFSETVALUE);
            //Canvas.SetTop(this, evc - OFFSETVALUE);

            //if (cline != null)
            //{
            //    cline.Regenerate(this);
            //}
            //if (bcurve != null)
            //{
            //    bcurve.Regenerate(this);
            //    double maxv = 0.0;
            //    double minv = 0.0;
            //    bcurve.GetMaximumMinimumOrdinates(maxheight, out minv, out maxv);
            //}

            //if (lcurve != null)
            //{
            //    lcurve.Regenerate(this);
            //}
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            //ui.ViewModel.WorkspaceViewModel.HasUnsavedChanges = true;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //nodemodel.MarkNodeAsModified(true);
        }

        public void Update(double ehc, double evc)
        {
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

            if (cline != null)
            {
                cline.Regenerate(this);
            }
            if (bcurve != null)
            {
                bcurve.Regenerate(this);
                double maxv = 0.0;
                double minv = 0.0;
                bcurve.GetMaximumMinimumOrdinates(maxheight, out minv, out maxv);
            }
        }
    }
}
