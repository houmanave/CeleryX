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
    /// Interaction logic for cecoPointFree.xaml
    /// </summary>
    public partial class cecoPointFree : Thumb
    {
        private double OFFSETVALUE = 7;

        public sinecurve scurve { get; set; }
        public linearcurve lcurve { get; set; }
        public paraboliccurve parabcurve { get; set; }
        public perlinnoisecurve perlcurve { get; set; }

        public crosshair chairhor { get; set; }
        public crosshair chairver { get; set; }
        public uvcoordtext uvtext { get; set; }

        public double maxwidth { get; set; }
        public double maxheight { get; set; }

        //readonly NodeModel nodemodel;
        //private IViewModelView<NodeViewModel> ui;

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

        public cecoPointFree(Point p/*, NodeModel model, IViewModelView<NodeViewModel> nodeUI*/)
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

            if (scurve != null)
            {
                scurve.Regenerate(this);
            }

            if (lcurve != null)
            {
                lcurve.Regenerate(this);
            }

            if (parabcurve != null)
            {
                parabcurve.Regenerate(this);
            }

            if (perlcurve != null)
            {
                perlcurve.Regenerate(this);
            }

            if (chairhor != null)
            {
                chairhor.Regenerate(this);
            }
            if (chairver != null)
            {
                chairver.Regenerate(this);
            }
            if (uvtext != null)
            {
                uvtext.Regenerate(Point);
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            //ui.ViewModel.WorkspaceViewModel.HasUnsavedChanges = true;
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //nodemodel.MarkNodeAsModified(true);
        }
    }
}
