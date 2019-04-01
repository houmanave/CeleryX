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
    public abstract class _curvebase : UIElement
    {
        private Path _pathCurve;
        private PathGeometry _pathGeometry;
        private PathFigure _pathFigure;

        public Path PathCurve
        {
            get
            {
                return _pathCurve;
            }

            set
            {
                _pathCurve = value;
            }
        }

        public PathGeometry PathGeometry
        {
            get
            {
                return _pathGeometry;
            }

            set
            {
                _pathGeometry = value;
            }
        }

        public PathFigure PathFigure
        {
            get
            {
                return _pathFigure;
            }

            set
            {
                _pathFigure = value;
            }
        }

        public _curvebase()
        {
            _pathGeometry = new PathGeometry();
            _pathFigure = new PathFigure();
        }

        public virtual List<double> GetValuesFromAssignedParameters(double lowLimit, double highLimit, int count)
        {
            return new List<double>();
        }
    }
}
