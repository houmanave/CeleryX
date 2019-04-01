using Celery.Controls.SubcontrolClasses;
using Celery.Controls.Subcontrols;
using Celery.UI;
using Dynamo.Graph.Nodes;
using Dynamo.UI;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Celery.Controls
{
    /// <summary>
    /// CeleryGraphControl.xaml の相互作用ロジック
    /// </summary>
    public partial class CeleryGraphControl : UserControl
    {
        readonly NodeModel nodeModel;
        private IViewModelView<NodeViewModel> ui;

        public CeleryGraphControl(NodeModel model, IViewModelView<NodeViewModel> nodeUI)
        {
            InitializeComponent();

            nodeModel = model;
            ui = nodeUI;
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //if (this.DataContext != null)
            //{
            //    GraphMap model = this.DataContext as GraphMap;
            //    if (model != null && model.CanvasWidth > 0.0 && model.CanvasHeight > 0.0)
            //    {
            //        if (this.thisCanvas.ActualWidth > 0.0)
            //            model.CanvasWidth = this.thisCanvas.ActualWidth;
            //        if (this.thisCanvas.ActualHeight > 0.0)
            //            model.CanvasHeight = this.thisCanvas.ActualHeight;
            //    }
            //}
        }

        private void ceallPointGrip_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double horizdim = ActualWidth + e.HorizontalChange;
            double vertdim = ActualHeight + e.VerticalChange;

            if (this.Parent.GetType() == typeof(Grid))
            {
                Width = horizdim >= 300.0 ? horizdim : 300.0;
                Height = vertdim >= 300.0 ? vertdim : 300.0;

                Debug.WriteLine(Width.ToString() + " " + Height.ToString());

                //
                GraphMap gmmodel = this.nodeModel as GraphMap;
                if (gmmodel != null)
                {
                    double canwid = _setLimiter(gmmodel.canvas.ActualWidth, e.HorizontalChange, 300.0);
                    double canhei = _setLimiter(gmmodel.canvas.ActualHeight, e.VerticalChange, 260.0);
                    //double poibezcont2x = _setLimiter(gmmodel.PointBezCont2.Point.X, e.HorizontalChange, 300.0);
                    //double poibezcont2y = _setLimiter(gmmodel.PointBezCont2.Point.Y, e.VerticalChange, 260.0);
                    double poibezcont1x = _setLimiterProportional(gmmodel.PointBezCont1.Point.X, gmmodel.canvas.ActualWidth, canwid);
                    double poibezcont1y = _setLimiterProportional(gmmodel.PointBezCont1.Point.Y, gmmodel.canvas.ActualHeight, canhei);
                    double poibezcont2x = _setLimiterProportional(gmmodel.PointBezCont2.Point.X, gmmodel.canvas.ActualWidth, canwid);
                    double poibezcont2y = _setLimiterProportional(gmmodel.PointBezCont2.Point.Y, gmmodel.canvas.ActualHeight, canhei);
                    double poibez1x = _setLimiter(gmmodel.PointBezier1.Point.X, e.HorizontalChange, 300.0);
                    double poibez1y = _setLimiter(gmmodel.PointBezier1.Point.Y, e.VerticalChange, 260.0);
                    double poibez2x = _setLimiter(gmmodel.PointBezier2.Point.X, e.HorizontalChange, 300.0);
                    double poibez2y = _setLimiter(gmmodel.PointBezier2.Point.Y, e.VerticalChange, 260.0);

                    //gmmodel.CanvasWidth = gmmodel.canvas.ActualWidth + e.HorizontalChange;
                    //gmmodel.CanvasHeight = gmmodel.canvas.ActualHeight + e.VerticalChange;
                    gmmodel.canvas.Width = canwid;
                    gmmodel.canvas.Height = canhei;

                    gmmodel.PointBezCont1.maxwidth = canwid;
                    gmmodel.PointBezCont1.maxheight = canhei;
                    gmmodel.PointBezCont1.Update(poibezcont1x, poibezcont1y);
                    gmmodel.PointBezCont2.maxwidth = canwid;
                    gmmodel.PointBezCont2.maxheight = canhei;
                    gmmodel.PointBezCont2.Update(poibezcont2x, poibezcont2y);
                    gmmodel.PointBezier1.maxwidth = canwid;
                    gmmodel.PointBezier1.maxheight = canhei;
                    gmmodel.PointBezier1.Update(gmmodel.PointBezier1.Point.X, poibez1y);
                    gmmodel.PointBezier2.maxwidth = canwid;
                    gmmodel.PointBezier2.maxheight = canhei;
                    gmmodel.PointBezier2.Update(poibez2x, poibez2y);

                    //gmmodel.CurveBezier.Regenerate(new cecoPointControl(new Point(poibezcont2x, poibezcont2y)));
                    //gmmodel.CurveBezier.Regenerate(new cecoPointEnd(new Point(poibez2x, poibez2y)));
                }
            }
        }

        private double _setLimiter(double fix, double delta, double lowLimit)
        {
            return (fix + delta) >= lowLimit ? fix + delta : lowLimit;
        }

        private double _setLimiterProportional(double prevValue, double prevPropValueNoDelta, double prevPropValue)
        {
            return prevValue * prevPropValueNoDelta / prevPropValue;
        }
    }
}
