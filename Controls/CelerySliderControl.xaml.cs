using Dynamo.Graph.Nodes;
using Dynamo.UI;
using Dynamo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Celery.Controls
{
    /// <summary>
    /// Interaction logic for CelerySliderControl.xaml
    /// </summary>
    public partial class CelerySliderControl : UserControl
    {
        readonly NodeModel nodeModel;
        private IViewModelView<NodeViewModel> ui;

        public CelerySliderControl(NodeModel model, IViewModelView<NodeViewModel> nodeUI)
        {
            InitializeComponent();

            nodeModel = model;
            ui = nodeUI;

            //slider.PreviewMouseUp += delegate
            //{
            //    nodeUI.ViewModel.DynamoViewModel.retu
            //};
        }
    }
}
