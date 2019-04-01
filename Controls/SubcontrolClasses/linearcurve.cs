using Celery.Controls.Subcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Celery.Controls.SubcontrolClasses
{
    public class linearcurve : _curvebase
    {
        LineSegment lseg = null;

        cecoPointFree tep1 = null;
        cecoPointFree tep2 = null;

        double maxwidth = double.PositiveInfinity;
        double maxheight = double.PositiveInfinity;

        //public linearcurve(cecoPointEnd e1, cecoPointEnd e2, double maxw, double maxh)
        //{
        //    tep1 = e1;
        //    tep2 = e2;

        //    maxwidth = maxw;
        //    maxheight = maxh;

        //    lseg = new LineSegment(e1.Point, true);

        //    PathFigure = new PathFigure();
        //    PathFigure.StartPoint = e1.Point;
        //    PathFigure.Segments.Add(lseg);

        //    PathGeometry = new PathGeometry();
        //    PathGeometry.Figures.Add(PathFigure);

        //    PathCurve = new Path();
        //    PathCurve.Data = PathGeometry;

        //    PathCurve.Stroke = Brushes.Cornsilk;
        //    PathCurve.StrokeThickness = 4;
        //    PathCurve.Opacity = 0.9;
        //}

        public linearcurve(cecoPointFree e1, cecoPointFree e2, double maxw, double maxh)
        {
            tep1 = e1;
            tep2 = e2;

            maxwidth = maxw;
            maxheight = maxh;

            lseg = new LineSegment(e1.Point, true);

            PathFigure = new PathFigure();
            PathFigure.StartPoint = e1.Point;
            PathFigure.Segments.Add(lseg);

            PathGeometry = new PathGeometry();
            PathGeometry.Figures.Add(PathFigure);

            PathCurve = new Path();
            PathCurve.Data = PathGeometry;

            PathCurve.Stroke = Brushes.Cornsilk;
            PathCurve.StrokeThickness = 3;
            PathCurve.Opacity = 0.9;
        }

        private double LineEquation(double x)
        {
            double m = (tep2.Point.Y - tep1.Point.Y) / (tep2.Point.X - tep1.Point.X);
            if (double.IsNaN(m))
            {
                return double.NaN;
            }

            //  y - y1 = m(x - x1);
            return m * (x - tep1.Point.X) + (tep1.Point.Y);
        }

        private double SolveForXGivenY(double y)
        {
            double m = (tep2.Point.Y - tep1.Point.Y) / (tep2.Point.X - tep1.Point.X);
            if (double.IsNaN(m))
            {
                return double.NaN;
            }

            //  x = (y - y1) / m + x1
            return ((y - tep1.Point.Y) / m) + tep1.Point.X;
        }

        public void Regenerate(cecoPointFree tep)
        {
            if (tep1 == tep)
            {
                tep1 = tep;
            }
            else if (tep2 == tep)
            {
                tep2 = tep;
            }

            //  solve for the equation
            double y01 = LineEquation(0);
            double y02 = LineEquation(maxwidth);
            if (double.IsNaN(y01) || double.IsNaN(y02))
            {
                //  do something to send error on divisiion by zero.
            }
            else
            {
                System.Windows.Point pp;
                if (y01 < 0)
                {
                    pp = new System.Windows.Point(SolveForXGivenY(0), 0);
                }
                else if (y01 > maxheight)
                {
                    pp = new System.Windows.Point(SolveForXGivenY(maxheight), maxheight);
                }
                else
                {
                    pp = new System.Windows.Point(0, y01);
                }
                PathFigure.StartPoint = pp;

                System.Windows.Point pq;
                if (y02 > maxheight)
                {
                    pq = new System.Windows.Point(SolveForXGivenY(maxheight), maxheight);
                }
                else if (y02 < 0)
                {
                    pq = new System.Windows.Point(SolveForXGivenY(0), 0);
                }
                else
                {
                    pq = new System.Windows.Point(maxwidth, y02);
                }
                lseg.Point = pq;
            }
        }

        public override List<double> GetValuesFromAssignedParameters(double lowLimit, double highLimit, int count)
        {
            if (count < 1)
                return null;

            List<double> livalues = new List<double>();

            //  test verticality
            if (tep1.Point.X == tep2.Point.X)
            {
                return null;
            }
            if (double.IsNaN(LineEquation(0.0)))
            {
                return null;
            }

            double incount = (double)(count - 1);
            for (double d = 0.0; d < maxwidth; d += (maxwidth / incount))
            {
                double md = maxheight - LineEquation(d);
                double rd = (highLimit - lowLimit) * md / maxheight;
                rd += lowLimit;
                livalues.Add(rd);
            }

            if (livalues.Count < count)
            {
                double mdx = maxheight - LineEquation(maxwidth);
                double rdx = (highLimit - lowLimit) * mdx / maxheight;
                rdx += lowLimit;
                livalues.Add(rdx);
            }

            return livalues;
        }
    }
}
