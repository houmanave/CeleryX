using Celery.Controls.Subcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Celery.Controls.SubcontrolClasses
{
    public class paraboliccurve : _curvebase
    {
        PolyLineSegment pseg = null;

        cecoPointFree tsvtx = null;
        cecoPointFree tsppp = null;

        double maxwidth = double.PositiveInfinity;
        double maxheight = double.PositiveInfinity;

        const double PARABOLIC_X_INCREMENT = 1.0;

        public paraboliccurve(Canvas canvas, cecoPointFree vp, cecoPointFree fp, double maxWidth, double maxHeight)
        {
            tsvtx = vp;
            tsppp = fp;

            maxwidth = maxWidth;
            maxheight = maxHeight;

            PathFigure = new PathFigure();

            GenerateParabola();

            PathFigure.Segments.Add(pseg);

            PathGeometry = new PathGeometry();
            PathGeometry.Figures.Add(PathFigure);

            PathCurve = new Path();
            PathCurve.Data = PathGeometry;

            PathCurve.Stroke = Brushes.Thistle;
            PathCurve.StrokeThickness = 4;
            PathCurve.Opacity = 0.9;
        }

        private double ParabolicEqSolveForX(double y, bool isNeg = false)
        {
            //  (X-h)^2 = 4a(Y-k)

            //  get the value of X
            //  (X-h) = sqrt(4a(Y-k))
            //  X = sqrt(4a(Y-k)) + h
            double a = Math.Pow(tsppp.Point.X - tsvtx.Point.X, 2) / (4.0 * (tsppp.Point.Y - tsvtx.Point.Y));
            double h = tsvtx.Point.X;
            double k = tsvtx.Point.Y;

            return ((isNeg) ? -1.0 : 1.0) * Math.Sqrt(4.0 * a * (y - k)) + h;
        }

        public double ParabolicEquation(double x)
        {
            //  (X-h)^2 = 4a(Y-k)

            //  get the value of a
            //  (X-h)^2 / (Y-k) = 4a
            //  a = (X-h)^2 / 4(Y-k)
            double a = Math.Pow(tsppp.Point.X - tsvtx.Point.X, 2) / (4.0 * (tsppp.Point.Y - tsvtx.Point.Y));
            //a *= (tsppp.Point.Y > tsvtx.Point.Y) ? 1.0 : -1.0;
            double h = tsvtx.Point.X;
            double k = tsvtx.Point.Y;

            //  Y = ((X-h)^2 / 4a) + k
            return (Math.Pow(x - h, 2) / (4 * a)) + k;
        }

        public void GenerateParabola()
        {
            if (pseg == null)
            {
                pseg = new PolyLineSegment();
                pseg.IsStroked = true;
            }
            else
            {
                pseg.Points.Clear();
            }

            double solvey = (tsppp.Point.Y > tsvtx.Point.Y) ? maxheight : 0.0;

            double idxtrigmin = ParabolicEqSolveForX(solvey, true);
            double idxtrigmax = ParabolicEqSolveForX(solvey);
            double idxtrig0 = (idxtrigmin > idxtrigmax) ? idxtrigmax : idxtrigmin;
            double idxtrigwid = (idxtrigmin < idxtrigmax) ? idxtrigmax : idxtrigmin;
            idxtrig0 = (idxtrig0 > 0) ? idxtrig0 : 0;
            idxtrigwid = (idxtrigwid < maxwidth) ? idxtrigwid : maxwidth;

            if (PathFigure != null)
            {
                double vyy = ParabolicEquation(idxtrig0);
                PathFigure.StartPoint = new Point(idxtrig0, vyy);
            }

            for (double d = idxtrig0; d < idxtrigwid; d += PARABOLIC_X_INCREMENT)
            {
                double vy = ParabolicEquation(d);
                if (vy >= 0 && vy < maxheight)
                    pseg.Points.Add(new Point(d, vy));
            }

            double vmy = ParabolicEqSolveForX(idxtrigwid);
            pseg.Points.Add(new Point(idxtrigwid, ParabolicEquation(idxtrigwid)));
        }

        public void Regenerate(cecoPointFree e)
        {
            if (tsvtx == e)
            {
                tsvtx = e;
            }
            else if (tsppp == e)
            {
                tsppp = e;
            }

            GenerateParabola();
        }

        public override List<double> GetValuesFromAssignedParameters(double lowLimit, double highLimit, int count)
        {
            if (count < 1)
                return null;

            List<double> livalues = new List<double>();

            if (tsvtx.Point.X == tsppp.Point.X)
                return null;

            int incount = count - 1;
            for (double d = 0.0; d < maxwidth; d += (maxwidth / incount))
            {
                double md = maxheight - ParabolicEquation(d);
                double rd = (highLimit - lowLimit) * md / maxheight;
                rd += lowLimit;
                livalues.Add(rd);
            }

            if (livalues.Count < count)
            {
                double mdx = maxheight - ParabolicEquation(maxwidth);
                double rdx = (highLimit - lowLimit) * mdx / maxheight;
                rdx += lowLimit;
                livalues.Add(rdx);
            }

            return livalues;
        }
    }
}
