using Celery.Controls.Subcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Celery.Controls.SubcontrolClasses
{
    public class crosshair : _curvebase
    {
        LineSegment chline = null;

        double maxwidth = 250;
        double maxheight = 250;

        Point point = new Point();

        bool isvertical = false;

        public crosshair(Canvas canvas, Point p, double mw, double mh, bool isVert = false)
        {
            maxwidth = mw;
            maxheight = mh;

            point = p;

            isvertical = isVert;

            PathFigure = new PathFigure();
            if (isvertical)
            {
                PathFigure.StartPoint = new System.Windows.Point(point.X, 0);
                chline = new LineSegment(new System.Windows.Point(point.X, maxheight), true);
            }
            else
            {
                PathFigure.StartPoint = new System.Windows.Point(0, point.Y);
                chline = new LineSegment(new System.Windows.Point(maxwidth, point.Y), true);
            }
            PathFigure.Segments.Add(chline);

            PathGeometry = new PathGeometry();
            PathGeometry.Figures.Add(PathFigure);

            PathCurve = new System.Windows.Shapes.Path();
            PathCurve.Data = PathGeometry;

            PathCurve.Stroke = System.Windows.Media.Brushes.DimGray;
            PathCurve.StrokeThickness = 1;
            PathCurve.Opacity = 0.7;

            PathCurve.StrokeDashArray.Add(4);
            PathCurve.StrokeDashArray.Add(1);

            Canvas.SetZIndex(PathCurve, 10);
        }

        public void Regenerate(cecoPointFree p)
        {
            if (isvertical)
            {
                PathFigure.StartPoint = new System.Windows.Point(p.Point.X, 0);
                chline.Point = new System.Windows.Point(p.Point.X, maxheight);
            }
            else
            {
                PathFigure.StartPoint = new System.Windows.Point(0, p.Point.Y);
                chline.Point = new System.Windows.Point(maxwidth, p.Point.Y);
            }
        }
    }
}
