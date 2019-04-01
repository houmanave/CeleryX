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
    public class perlinnoisecurve : _curvebase
    {
        PolyLineSegment pseg = null;

        cecoPointOrtho tsop1 = null;
        cecoPointOrtho tsop2 = null;
        cecoPointFree tsfp = null;

        double maxwidth = 258;
        double maxheight = 200;

        int PERLIN_MAX_VERTICES = 2000;
        int PERLIN_MAX_VERTICES_MASK = 0;
        double amplitude = 1.0;
        double scale = 1.0;

        List<double> r = null;

        Random rand;

        public perlinnoisecurve(Canvas canvas, cecoPointOrtho op1, cecoPointOrtho op2, cecoPointFree fp, int seed, double maxWidth, double maxHeight)
        {
            tsop1 = op1;
            tsop2 = op2;
            tsfp = fp;

            PERLIN_MAX_VERTICES_MASK = PERLIN_MAX_VERTICES - 1;

            rand = new Random(seed);

            //
            Set(4.0, 0.1, 1.0, 1, seed);
            //

            r = new List<double>();

            maxwidth = maxWidth;
            maxheight = maxHeight;

            for (int i = 0; i < PERLIN_MAX_VERTICES; i++)
            {
                r.Add(rand.NextDouble() - 0.5);
            }

            PathFigure = new PathFigure();

            GeneratePerlinCurve();

            PathFigure.Segments.Add(pseg);

            PathGeometry = new PathGeometry();
            PathGeometry.Figures.Add(PathFigure);

            PathCurve = new Path();
            PathCurve.Data = PathGeometry;

            PathCurve.Stroke = Brushes.Coral;
            PathCurve.StrokeThickness = 4;
            PathCurve.Opacity = 0.9;
            PathCurve.StrokeLineJoin = PenLineJoin.Round;
        }

        public Point PerlinAddPointsToCurve(double x)
        {
            return new Point(x, GetHeight(-tsfp.Point.X + x, 0.0) + tsfp.Point.Y);
        }

        public void GeneratePerlinCurve()
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

            scale = (maxwidth - tsop1.Point.X) / (maxwidth * 3.0);
            amplitude = tsop2.Point.Y * 1.1;// * 3.0 / maxheight;
            frequency = scale;

            int startindex = (PERLIN_MAX_VERTICES / 2) - (int)(tsfp.Point.X);

            if (PathFigure != null)
            {
                PathFigure.StartPoint = PerlinAddPointsToCurve(0.0);
            }

            if (pseg != null)
            {
                for (double x = 1.0; x < maxwidth; x += 1.0)
                {
                    pseg.Points.Add(PerlinAddPointsToCurve(x));
                }

                pseg.Points.Add(PerlinAddPointsToCurve(maxwidth));
            }
        }

        public void Regenerate(cecoPointOrtho e)
        {
            if (tsop1 == e)
            {
                tsop1 = e;
            }
            else if (tsop2 == e)
            {
                tsop2 = e;
            }

            GeneratePerlinCurve();
        }

        public void Regenerate(cecoPointFree e)
        {
            if (tsfp == e)
            {
                tsfp = e;
            }

            GeneratePerlinCurve();
        }

        public override List<double> GetValuesFromAssignedParameters(double lowLimit, double highLimit, int count)
        {
            if (count < 1)
                return null;

            List<double> livalues = new List<double>();

            int incount = count - 1;
            for (double d = 0.0; d < maxwidth; d += (maxwidth / incount))
            {
                double hei = GetHeight(-tsfp.Point.X + d, 0.0);
                double md = -hei + (maxheight - tsfp.Point.Y);
                double rd = (highLimit - lowLimit) * md / maxheight;
                rd += lowLimit;
                livalues.Add(rd);
            }

            if (livalues.Count < count)
            {
                double hei = GetHeight(-tsfp.Point.X + maxwidth, 0.0);
                double md = -hei + (maxheight - tsfp.Point.Y);
                double rd = (highLimit - lowLimit) * md / maxheight;
                rd += lowLimit;
                livalues.Add(rd);
            }

            return livalues;
        }

        //  SOURCED FROM STACK OVERFLOW
        private int octaves;
        private double frequency;
        private double persistence;
        private int randomseed;

        public void Set(double persist, double freq, double amp, int octvs, int randseed)
        {
            persistence = persist;
            randomseed = randseed;
            octaves = octvs;
            amplitude = amp;
            frequency = freq;
        }

        double GetHeight(double x, double y)
        {
            return amplitude * Total(x, y);
        }

        double Total(double i, double j)
        {
            double t = 0.0;
            double _amplitude = 1;
            double frq = frequency;

            for (int k = 0; k < octaves; k++)
            {
                t += GetValue(j * frq + randomseed + (PERLIN_MAX_VERTICES / 2), i * frq + randomseed + (PERLIN_MAX_VERTICES / 2)) * _amplitude;
                _amplitude *= persistence;
                frq *= 2;
            }

            return t;
        }

        double GetValue(double x, double y)
        {
            int xint = (int)x;
            int yint = (int)y;
            double xfrac = x - xint;
            double yfrac = y - yint;

            //noise values
            double n01 = Noise(xint - 1, yint - 1);
            double n02 = Noise(xint + 1, yint - 1);
            double n03 = Noise(xint - 1, yint + 1);
            double n04 = Noise(xint + 1, yint + 1);
            double n05 = Noise(xint - 1, yint);
            double n06 = Noise(xint + 1, yint);
            double n07 = Noise(xint, yint - 1);
            double n08 = Noise(xint, yint + 1);
            double n09 = Noise(xint, yint);

            double n12 = Noise(xint + 2, yint - 1);
            double n14 = Noise(xint + 2, yint + 1);
            double n16 = Noise(xint + 2, yint);

            double n23 = Noise(xint - 1, yint + 2);
            double n24 = Noise(xint + 1, yint + 2);
            double n28 = Noise(xint, yint + 2);

            double n34 = Noise(xint + 2, yint + 2);

            //find the noise values of the four corners
            double x0y0 = 0.0625 * (n01 + n02 + n03 + n04) + 0.125 * (n05 + n06 + n07 + n08) + 0.25 * (n09);
            double x1y0 = 0.0625 * (n07 + n12 + n08 + n14) + 0.125 * (n09 + n16 + n02 + n04) + 0.25 * (n06);
            double x0y1 = 0.0625 * (n05 + n06 + n23 + n24) + 0.125 * (n03 + n04 + n09 + n28) + 0.25 * (n08);
            double x1y1 = 0.0625 * (n09 + n16 + n28 + n34) + 0.125 * (n08 + n14 + n06 + n24) + 0.25 * (n04);

            //interpolate between those values according to the x and y fractions
            double v1 = Interpolate(x0y0, x1y0, xfrac); //interpolate in x direction (y)
            double v2 = Interpolate(x0y1, x1y1, xfrac); //interpolate in x direction (y+1)
            double fin = Interpolate(v1, v2, yfrac);  //interpolate in y direction

            return fin;
        }

        double Interpolate(double x, double y, double a)
        {
            double nega = 1.0 - a;
            double negasqr = nega * nega;
            double fac1 = 3.0 * negasqr - 2.0 * negasqr * nega;
            double asqr = a * a;
            double fac2 = 3.0 * asqr - 2.0 * (asqr * a);

            return x * fac1 + y * fac2;
        }

        private double Noise(int x, int y)
        {
            int n = x + y * 57;
            n = (n << 13) ^ n;
            int t = (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff;
            return 1.0 - (double)t * 0.93132257461547858515625e-9;
        }
        //
    }
}
