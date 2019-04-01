using Autodesk.Revit.DB;
using Celery.Controls;
using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.ViewModels;
using Dynamo.Wpf;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using RevitServices;

using RevElement = Revit.Elements.Element;
using RevParameter = Revit.Elements.Parameter;

using Docman = RevitServices.Persistence.DocumentManager;
using Transman = RevitServices.Transactions.TransactionManager;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph;
using System.Xml;

namespace Celery.UI
{
    [NodeName("Revit Parameter Selector")]
    [NodeCategory("Celery.RevitControls")]
    [NodeDescription("A list to select a parameter from the element.")]
    [InPortNames("Element")]
    [InPortDescriptions("Input element.")]
    [InPortTypes("var")]
    //[OutPortNames("paramName", "paramValue")]
    //[OutPortDescriptions("Name of the parameter selected.", "Value of the parameter selected.")]
    //[OutPortTypes("string", "var")]
    [OutPortNames("paramName")]
    [OutPortDescriptions("Name of the parameter selected.")]
    [OutPortTypes("var")]
    [IsDesignScriptCompatible]
    class ParameterSelector : NodeModel
    {
        internal EngineController PSEngineController { get; set; }

        private ObservableCollection<RevParameter> _itemsCollection;
        public ObservableCollection<RevParameter> ItemsCollection
        {
            get { return _itemsCollection; }
            set
            {
                _itemsCollection = value;
                RaisePropertyChanged("ItemsCollection");
            }
        }

        public RevElement RevitElement = null;

        private bool IsDeserialized = false;

        private RevParameter _selectedItem;
        public RevParameter SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_selectedItem != null && _selectedItem.HasValue)
                    SelectedItemValue = _selectedItem.Value;
                RaisePropertyChanged("SelectedItem");

                OnNodeModified();
            }
        }

        private object _selectedItemValue;
        public object SelectedItemValue
        {
            get { return _selectedItemValue; }
            set { _selectedItemValue = value; }
        }

        public event Action RequestChangeParameterSelector;

        protected virtual void OnRequestChangeParameterSelector()
        {
            if (RequestChangeParameterSelector != null)
            {
                RequestChangeParameterSelector();
            }
        }

        public ParameterSelector()
        {
            RegisterAllPorts();

            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            ItemsCollection = new ObservableCollection<RevParameter>();
        }

        void Connectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnRequestChangeParameterSelector();
        }

        public void PopulateItems(EngineController ec)
        {
            if (HasConnectedInput(0))
            {
                var minnode = InPorts[0].Connectors[0].Start.Owner;
                var minindex = InPorts[0].Connectors[0].Start.Index;

                var minid = minnode.GetAstIdentifierForOutputIndex(minindex).Name;

                var minmirror = this.PSEngineController.GetMirror(minid);

                if (minmirror.GetData().IsCollection)
                {
                    //ItemsCollection = new ObservableCollection<Parameter>(minmirror.GetData().GetElements().Select(x => (Parameter)x.Data));
                }
                else
                {
                    RevitElement = (RevElement)minmirror.GetData().Data;
                    if (RevitElement != null)
                    {
                        foreach (Revit.Elements.Parameter param in RevitElement.Parameters)
                        {
                            ItemsCollection.Add(param);
                        }
                    }
                }

                if (!IsDeserialized)
                    SelectedItem = ItemsCollection.FirstOrDefault();
                //else
                //    IsDeserialized = false;
            }
        }

        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            List<AssociativeNode> outputnodes = new List<AssociativeNode>();

            if (!HasConnectedInput(0))
            {
                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()));
                //outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildNullNode()));
            }
            else if (SelectedItem == null)
            {
                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()));
                //outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildNullNode()));
            }
            else
            {
                AssociativeNode parameternamenode;
                AssociativeNode parametervaluenode;

                parameternamenode = AstFactory.BuildStringNode(SelectedItem.Name);
                //parameternamenode = AstFactory.BuildPrimitiveNodeFromObject(SelectedItemValue);
                try
                {
                    if (SelectedItemValue == null)
                        parametervaluenode = AstFactory.BuildNullNode();
                    else
                        parametervaluenode = AstFactory.BuildPrimitiveNodeFromObject(SelectedItemValue);
                }
                catch
                {
                    parametervaluenode = AstFactory.BuildNullNode();
                }

                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), parameternamenode));
                //outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), parametervaluenode));
            }

            return outputnodes;
        }

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context);

            var xmldoc = element.OwnerDocument;
            var subnode = xmldoc.CreateElement("Celery.RevitParameterSelector");
            if (SelectedItem != null) subnode.SetAttribute("selectedparameter", SelectedItem.ToString());
            if (subnode.Attributes.Count > 0) element.AppendChild(subnode);
        }

        protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.DeserializeCore(nodeElement, context);

            foreach (XmlNode subnode in nodeElement.ChildNodes)
            {
                if (!subnode.Name.Equals("Celery.RevitParameterSelector"))
                {
                    continue;
                }
                if (subnode.Attributes == null || (subnode.Attributes.Count <= 0))
                    continue;

                foreach (XmlAttribute attr in subnode.Attributes)
                {
                    switch (attr.Name)
                    {
                        case "selectedparameter":
                            if (RevitElement != null) SelectedItem = Revit.Elements.Parameter.ParameterByName(RevitElement, attr.Value);
                            break;
                        default:
                            Log(string.Format("{0} attribute could not be deserialized for {1}", attr.Name, GetType()));
                            break;
                    }
                }

                IsDeserialized = true;
            }
        }
    }

    class ParameterSelectorViewCustomization : INodeViewCustomization<ParameterSelector>
    {
        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;

        private ParameterSelector lismodel;
        private CeleryItemSelectorControl liscontrol;

        public void CustomizeView(ParameterSelector model, Dynamo.Controls.NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);

            lismodel = model;

            lismodel.PSEngineController = nodeView.ViewModel.DynamoViewModel.EngineController;

            liscontrol = new CeleryItemSelectorControl();
            nodeView.inputGrid.Children.Add(liscontrol);
            liscontrol.DataContext = model;

            model.RequestChangeParameterSelector += UpdateParameterSelector;

            UpdateParameterSelector();
        }

        private void UpdateParameterSelector()
        {
            var s = dynamoViewmodel.Model.Scheduler;

            var t = new DelegateBasedAsyncTask(s, () =>
            {
                //  use EngineController here
                lismodel.PopulateItems(dynamoModel.EngineController);
            });

            t.ThenSend((_) =>
            {
                lismodel.SelectedItem = lismodel.ItemsCollection.FirstOrDefault();
            }, syncContext);

            s.ScheduleForExecution(t);
        }

        public void Dispose()
        {

        }
    }

}
