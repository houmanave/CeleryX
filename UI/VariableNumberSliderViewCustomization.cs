using Celery.Controls;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.UI;
using Dynamo.ViewModels;
using Dynamo.Wpf;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Celery.UI
{
    class VariableNumberSliderViewCustomization : INodeViewCustomization<VariableNumberSlider>
    {
        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;
        private CelerySliderControl celerySliderControl;
        private VariableNumberSlider varianumsliNode;

        private IViewModelView<NodeViewModel> ui;

        public void CustomizeView(VariableNumberSlider model, Dynamo.Controls.NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);
            varianumsliNode = model;

            ui = nodeView;

            model.EngineController = nodeView.ViewModel.DynamoViewModel.EngineController;

            celerySliderControl = new CelerySliderControl(model, nodeView);
            nodeView.inputGrid.Children.Add(celerySliderControl);

            celerySliderControl.DataContext = model;
            
            model.RequestChangeVariableNumberSlider += UpdateVariaNumberSlider;

            UpdateVariaNumberSlider();

            celerySliderControl.drawSlider.PreviewMouseUp += drawSlider_PreviewMouseUp;
            celerySliderControl.drawSlider.ValueChanged += drawSlider_ValueChanged;
        }

        void drawSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            varianumsliNode.OutputviewValue = varianumsliNode.Minval + (varianumsliNode.Maxval - varianumsliNode.Minval) * (celerySliderControl.drawSlider.Value / varianumsliNode.SliderMaximum);
        }

        void drawSlider_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UpdateVariaNumberSlider();
        }

        private void UpdateVariaNumberSlider()
        {
            var s = dynamoViewmodel.Model.Scheduler;

            var t = new DelegateBasedAsyncTask(s, () =>
            {
                varianumsliNode.ComputeOutput(dynamoModel.EngineController);
            });

            t.ThenSend((_) =>
            {
                varianumsliNode.SliderValue = celerySliderControl.drawSlider.Value;
            }, syncContext);

            s.ScheduleForExecution(t);
        }

        public void Dispose()
        {
            celerySliderControl.drawSlider.PreviewMouseUp -= drawSlider_PreviewMouseUp;
            celerySliderControl.drawSlider.ValueChanged += drawSlider_ValueChanged;
        }
    }
}
