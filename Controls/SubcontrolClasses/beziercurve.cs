using Celery.Controls.Subcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Celery.Controls.SubcontrolClasses
{
    public class beziercurve : _curvebase
    {
        cecoPointControl tcp1 = null;
        cecoPointControl tcp2 = null;
        cecoPointEnd tep1 = null;
        cecoPointEnd tep2 = null;

        BezierSegment bseg = null;

        List<LineSegment> lilsegs = new List<LineSegment>();    //  use to find the values of x
        Dictionary<double, double> dicxy = new Dictionary<double, double>();    //  dictionary of x-y pairs
        double tfactor = 0.01;

        double maxwidth = double.PositiveInfinity;
        double maxheight = double.PositiveInfinity;

        public beziercurve(cecoPointEnd e1, cecoPointEnd e2, cecoPointControl c1, cecoPointControl c2, double maxw, double maxh)
        {
            tep1 = e1;
            tep2 = e2;
            tcp1 = c1;
            tcp2 = c2;

            maxwidth = maxw;
            maxheight = maxh;

            tfactor = 1.0 / maxwidth;

            bseg = new BezierSegment(tcp1.Point, tcp2.Point, tep2.Point, true);
            PathFigure = new PathFigure();
            PathFigure.StartPoint = tep1.Point;
            PathFigure.Segments.Add(bseg);
            PathGeometry = new PathGeometry();
            PathGeometry.Figures.Add(PathFigure);

            PathCurve = new Path();
            PathCurve.Data = PathGeometry;

            PathCurve.Stroke = Brushes.LightSkyBlue;
            PathCurve.StrokeThickness = 3;
            PathCurve.Opacity = 0.9;
        }

        public void GetValueAtT(double t, out double x, out double y)
        {
            Point p1 = PathFigure.StartPoint;
            Point p2 = bseg.Point1;
            Point p3 = bseg.Point2;
            Point p4 = bseg.Point3;

            x = Math.Pow(1 - t, 3) * p1.X +
                3 * Math.Pow(1 - t, 2) * t * p2.X +
                3 * (1 - t) * t * t * p3.X +
                t * t * t * p4.X;

            y = Math.Pow(1 - t, 3) * p1.Y +
                3 * Math.Pow(1 - t, 2) * t * p2.Y +
                3 * (1 - t) * t * t * p3.Y +
                t * t * t * p4.Y;
        }

        //public void GetValueAtX(double x, out double y)
        //{
        //    Point p1 = PathFigure.StartPoint;
        //    Point p2 = bseg.Point1;
        //    Point p3 = bseg.Point2;
        //    Point p4 = bseg.Point3;

        //    y = Math.Pow(1 - x, 3) * p1.Y +
        //        3 * Math.Pow(1 - x, 2) * x * p2.Y +
        //        3 * (1 - x) * x * x * p3.Y +
        //        x * x * x * p4.Y;
        //}

        public void GetMaximumMinimumOrdinates(double maxValue, out double min, out double max)
        {
            min = double.PositiveInfinity;
            max = 0.0;

            double xx = 0.0;
            double yy = 0.0;

            double val = double.NaN;
            for (double i = 0; i < maxValue; i += 1)
            {
                GetValueAtT(i / maxValue, out xx, out yy);
                val = Math.Round(maxValue - yy, 2);
                if (max < val)
                {
                    max = val;
                }
                if (min > val)
                {
                    min = val;
                }
            }

            GetValueAtT(1.0, out xx, out yy);
            val = Math.Round(maxValue - yy, 2);
            if (max < val)
            {
                max = val;
            }
            if (min > val)
            {
                min = val;
            }
        }

        public List<Point> GetPointsFromParameters(double maxValue, List<double> ts)
        {
            List<Point> retvals = new List<Point>();

            foreach (double o in ts)
            {
                double x = 0.0;
                double y = 0.0;
                GetValueAtT(o, out x, out y);

                Point p = new Point(x, y);
                retvals.Add(p);
            }

            return retvals;
        }

        //public List<Point> GetPointsFromAbscissas(List<double> xs)
        //{
        //    List<Point> retvals = new List<Point>();

        //    foreach (double x in xs)
        //    {
        //        double y = 0.0;
        //        GetValueAtX(x, out y);

        //        Point p = new Point(x, y);
        //        retvals.Add(p);
        //    }

        //    return retvals;
        //}

        private void GenerateXYPairs()
        {
            if (dicxy == null)
            {
                dicxy = new Dictionary<double, double>();
            }
            else
            {
                dicxy.Clear();
            }

            double xx = 0.0;
            double yy = 0.0;
            double tacqfac = maxwidth * 5.0;
            for (double t = 0.0; t <= 1.0; t += 1.0 / tacqfac)
            {
                GetValueAtT(t, out xx, out yy);
                xx = Math.Round(xx, 0);
                if (!dicxy.ContainsKey(xx))
                {
                    dicxy.Add(xx, yy);
                }
            }

            GetValueAtT(1.0, out xx, out yy);
            if (dicxy.ContainsKey(xx))
            {
                dicxy.Remove(xx);
            }
            dicxy.Add(xx, yy);
        }

        public void Regenerate(cecoPointControl tcp)
        {
            if (tcp1 == tcp)
            {
                tcp1 = tcp;
                bseg.Point1 = tcp.Point;
            }
            else if (tcp2 == tcp)
            {
                tcp2 = tcp;
                bseg.Point2 = tcp.Point;
            }

            //  regenerate x-y pairs
        }

        public void Regenerate(cecoPointEnd tep)
        {
            if (tep1 == tep)
            {
                tep1 = tep;
                PathFigure.StartPoint = tep.Point;
            }
            else if (tep2 == tep)
            {
                tep2 = tep;
                bseg.Point3 = tep.Point;
            }
        }

        public override List<double> GetValuesFromAssignedParameters(double lowLimit, double highLimit, int count)
        {
            if (count < 1)
                return null;

            GenerateXYPairs();

            List<double> livalues = new List<double>();

            for (int i = 0; i < count; i++)
            {
                double xx = 0.0;
                double yy = 0.0;
                //GetValueAtT((1.0 / (count - 1.0)) * i, out xx, out yy);
                xx = (int)((maxwidth / (count - 1.0)) * i);
                yy = dicxy[xx];

                double md = maxheight - yy;
                double rd = (highLimit - lowLimit) * md / maxheight;
                rd += lowLimit;
                livalues.Add(rd);
            }

            return livalues;
        }

        //public List<double> GetValuesFromAssignedParameters(double lowLimit, double highLimit, int count)
        //{
        //    if (count < 1)
        //        return null;

        //    List<double> livalues = new List<double>();

        //    int incount = count - 1;
        //    for (double d = 0.0; d < maxwidth; d += (maxwidth / incount))
        //    {
        //        double y = 0.0;
        //        GetValueAtX(d, out y);
        //        double md = maxheight - y;
        //        double rd = (highLimit - lowLimit) * md / maxheight;
        //        rd += lowLimit;
        //        livalues.Add(rd);
        //    }

        //    double yy = 0.0;
        //    GetValueAtX(maxwidth, out yy);
        //    double mdx = maxheight - yy;
        //    double rdx = (highLimit - lowLimit) * mdx / maxheight;
        //    rdx += lowLimit;
        //    livalues.Add(rdx);

        //    return livalues;
        //}
    }
}
