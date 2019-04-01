using Celery.Controls.Subcontrols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Celery.Adorners
{
    class Celery2DGraphSelectorAdorner : Adorner
    {
        const double GAP = 4.0;

        public Celery2DGraphSelectorAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            UIElement uiel = this.AdornedElement;

            double wid = uiel.DesiredSize.Width;
            double hei = uiel.DesiredSize.Height;

            double locx = Canvas.GetLeft(uiel);
            double locy = Canvas.GetTop(uiel);

            string text = " ";

            cegrPointItemThumb poithum = uiel as cegrPointItemThumb;
            if (poithum != null)
            {
                text = "x = " + poithum.XValue.ToString("{0:0.000}") + ", y = " + poithum.YValue.ToString("{0:0.000}");

                System.Windows.Point p = poithum.Point;

                Size thissize = CeleryFunctions.MeasureString(text);
                FormattedText formattedtext = CeleryFunctions.FormattedString(text);

                double px = p.X + GAP;
                double py = p.Y - thissize.Height - GAP;

                if ((thissize.Width + GAP) > (poithum.CanvasWidth - p.X))
                {
                    px = px - thissize.Width - GAP * 2;
                }

                if ((thissize.Height + GAP) > p.Y)
                {
                    py = p.Y + GAP;
                }

                Rect rect = new Rect(thissize);

                drawingContext.DrawText(formattedtext, new Point(px, py));
            }

            //base.OnRender(drawingContext);
        }
    }
}
