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
    public class sinecurve : _curvebase
    {
        PolyLineSegment pseg = null;

        cecoPointFree ep = null;
        cecoPointFree cp = null;

        double maxwidth = 300;
        double maxheight = 100;

        public bool isCosine = false;

        //  coefficients;
        private double coefA;   //  amplitude
        private double coefB;   //  2*PI/period
        private double coefC;   //  phase shift
        private double coefD;   //  vertical shift

        public sinecurve(Canvas canvas, cecoPointFree e, cecoPointFree c, double maxWidth, double maxHeight)
        {
            ep = e;
            cp = c;

            maxwidth = maxWidth;
            maxheight = maxHeight;

            PathFigure = new PathFigure();

            GenerateSineWave();

            PathFigure.Segments.Add(pseg);

            PathGeometry = new PathGeometry();
            PathGeometry.Figures.Add(PathFigure);

            PathCurve = new Path();
            PathCurve.Data = PathGeometry;
            PathCurve.Opacity = 0.9;
            PathCurve.Stroke = Brushes.Lavender;
            PathCurve.StrokeThickness = 3;

            //canvas.Children.Add(PathCurve);
        }

        private double ConvertXToTrigo(double x)
        {
            return Math.PI * x / maxwidth;
        }

        private double ConvertYToTrigo(double y)
        {
            return 2.0 * y / maxheight - 1.0;
        }

        private double ConvertTrigoToX(double trix)
        {
            return trix * maxwidth / Math.PI;
        }

        private double ConvertTrigoToY(double triy)
        {
            return (triy + 1.0) * maxheight / (2.0);
        }

        private void GetEquationCoefficients()
        {
            Point p1 = ep.Point;
            Point p2 = cp.Point;
            double p1x = ConvertXToTrigo(p1.X);
            double p2x = ConvertXToTrigo(p2.X);
            double p1y = ConvertYToTrigo(p1.Y);
            double p2y = ConvertYToTrigo(p2.Y);

            coefA = (p2y - p1y) / 2.0;
            coefB = Math.PI / (p2x - p1x);
            coefC = coefB * p1x;
            coefD = (p1y + p2y) / 2.0;
        }

        private double CosineEquation(double x)
        {
            return -(coefA * Math.Cos(coefB * x - coefC)) + coefD;
        }

        private void GenerateSineWave()
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

            pseg.IsSmoothJoin = true;

            double ud = ConvertXToTrigo(0.0);
            double udd = CosineEquation(ud);
            PathFigure.StartPoint = new Point(ConvertTrigoToX(ud), ConvertTrigoToY(udd));

            for (double d = 1.0; d < maxwidth; d += 2.0)
            {
                double vd = ConvertXToTrigo(d);
                double dd = CosineEquation(vd);
                pseg.Points.Add(new Point(ConvertTrigoToX(vd), ConvertTrigoToY(dd)));
            }
            double vx = ConvertXToTrigo(maxwidth);
            double dy = CosineEquation(vx);
            pseg.Points.Add(new Point(ConvertTrigoToX(vx), ConvertTrigoToY(dy)));
        }

        public void Regenerate(cecoPointFree e)
        {
            if (ep == e)
            {
                ep = e;
            }
            if (cp == e)
            {
                cp = e;
            }

            GetEquationCoefficients();
            GenerateSineWave();
        }

        public override List<double> GetValuesFromAssignedParameters(double lowLimit, double highLimit, int count)
        {
            if (count < 1)
                return null;

            List<double> livalues = new List<double>();

            if (ep.Point.X == cp.Point.X)
            {
                return null;
            }

            int incount = count - 1;
            for (double d = 0.0; d < maxwidth; d += (maxwidth / incount))
            {
                double md = maxheight - ConvertTrigoToY(CosineEquation(ConvertXToTrigo(d)));
                double rd = (highLimit - lowLimit) * md / maxheight;
                rd += lowLimit;
                livalues.Add(rd);
            }

            if (livalues.Count < count)
            {
                double mdx = maxheight - ConvertTrigoToY(CosineEquation(ConvertXToTrigo(maxwidth)));
                double rdx = (highLimit - lowLimit) * mdx / maxheight;
                rdx += lowLimit;
                livalues.Add(rdx);
            }

            return livalues;
        }
    }
}
