using Celery.UI;
using Dynamo.Configuration;
using Dynamo.Controls;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.ViewModels;
using Dynamo.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;

namespace Celery.ViewCustomization
{
    public class ListItemSelectorViewCustomization : INodeViewCustomization<ListItemSelectorX>
    {
        private ListItemSelectorX model;
        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;

        public void CustomizeView(ListItemSelectorX model, NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);

            this.model = model;
            this.model.EngineControl = nodeView.ViewModel.DynamoViewModel.EngineController;

            var cbx = new ComboBox
            {
                Width = System.Double.NaN,
                MinWidth = 75,
                Height = Configurations.PortHeightInPixels,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };

            nodeView.inputGrid.Children.Add(cbx);
            System.Windows.Controls.Grid.SetColumn(cbx, 0);
            System.Windows.Controls.Grid.SetRow(cbx, 0);

            cbx.DropDownOpened += Cbx_DropDownOpened;
            cbx.SelectionChanged += delegate
            {
                if (cbx.SelectedIndex != -1)
                    model.OnNodeModified();
            };

            cbx.DataContext = model;

            var bnd = new Binding("Items")
            {
                Mode = BindingMode.TwoWay,
                Source = model
            };
            cbx.SetBinding(ItemsControl.ItemsSourceProperty, bnd);

            var idxbnd = new Binding("SelectedIndex")
            {
                Mode = BindingMode.TwoWay,
                Source = model
            };
            cbx.SetBinding(Selector.SelectedIndexProperty, idxbnd);

            model.RequestChangeListItemSelector += UpdateListItemSelector;

            UpdateListItemSelector();
        }

        private void UpdateListItemSelector()
        {
            var s = dynamoViewmodel.Model.Scheduler;

            var t = new DelegateBasedAsyncTask(s, () =>
            {
                this.model.PopulateItems();
            });

            t.ThenSend((_) =>
            {
                this.model.OnNodeModified();
            }, syncContext);

            s.ScheduleForExecution(t);
        }

        private void Cbx_DropDownOpened(object sender, EventArgs e)
        {
            this.model.PopulateItems();
        }

        public void Dispose()
        {
        }
    }
}
