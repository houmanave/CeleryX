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
    /// Interaction logic for cecoPointControl.xaml
    /// </summary>
    public partial class cecoPointControl : Thumb
    {
        private double OFFSETVALUE = 6;

        public controlline cline { get; set; }
        public beziercurve bcurve { get; set; }
        public sinecurve scurve { get; set; }

        public double maxwidth { get; set; }
        public double maxheight { get; set; }

        //private IViewModelView<NodeViewModel> ui;
        //readonly NodeModel nodemodel;

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
            }
        }

        public cecoPointControl(Point p/*, NodeModel model, IViewModelView<NodeViewModel> nodeUI*/)
        {
            InitializeComponent();

            //nodemodel = model;
            //ui = nodeUI;

            Point = p;

            Canvas.SetLeft(this, p.X - OFFSETVALUE);
            Canvas.SetTop(this, p.Y - OFFSETVALUE);
        }

        public override string ToString()
        {
            return Point.X.ToString() + "," + Point.Y.ToString();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double ehc = Canvas.GetLeft(this) + e.HorizontalChange + OFFSETVALUE;
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

            //point.X = ehc;
            //point.Y = evc;

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
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            //if (ui != null)
            //{
            //    ui.ViewModel.WorkspaceViewModel.HasUnsavedChanges = true;
            //}
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //if (nodemodel != null)
            //{
            //    nodemodel.MarkNodeAsModified(true);
            //}
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

            point.X = ehc;
            point.Y = evc;

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
