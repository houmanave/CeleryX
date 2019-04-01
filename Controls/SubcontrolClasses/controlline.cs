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
    public class controlline : _curvebase
    {
        LineSegment lseg = null;

        public controlline(Point sp, Point ep)
        {
            lseg = new LineSegment(ep, true);

            PathFigure = new PathFigure();
            PathFigure.StartPoint = sp;
            PathFigure.Segments.Add(lseg);

            PathGeometry = new PathGeometry();
            PathGeometry.Figures.Add(PathFigure);

            PathCurve = new Path();
            PathCurve.Data = PathGeometry;

            PathCurve.Stroke = Brushes.LightGray;
            PathCurve.StrokeThickness = 2;
            PathCurve.Opacity = 0.7;

            PathCurve.StrokeDashArray.Add(3);
            PathCurve.StrokeDashArray.Add(1);
        }

        public void Regenerate(cecoPointEnd s)
        {
            PathFigure.StartPoint = s.Point;
        }

        public void Regenerate(cecoPointControl c)
        {
            lseg.Point = c.Point;
        }
    }
}
