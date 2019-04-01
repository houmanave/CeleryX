using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Celery.Controls.Subcontrols
{
    /// <summary>
    /// Interaction logic for ceallPointGrip.xaml
    /// </summary>
    public partial class ceallPointGrip : Thumb
    {
        //public UserControl ParentControl;

        public ceallPointGrip()
        {
            InitializeComponent();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            //if (ParentControl.ActualHeight > 300)
            //{
            //    ParentControl.Height += e.VerticalChange;
            //}
            //if (ParentControl.ActualWidth > 300)
            //{
            //    ParentControl.Width += e.HorizontalChange;
            //}
        }
    }
}
