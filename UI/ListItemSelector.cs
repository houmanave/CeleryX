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
using Dynamo.Graph;
using System.Xml;

namespace Celery.UI
{
    /// <summary>
    /// 
    /// </summary>
    [IsDesignScriptCompatible]
    [NodeName("List Item Selector")]
    [NodeCategory("Celery.ListControls")]
    [NodeDescription("Select an item from a list in the input with a drop-down control.")]
    [InPortNames("list")]
    [InPortDescriptions("Input list.")]
    [InPortTypes("var[]..[]")]
    [OutPortNames("item")]
    [OutPortDescriptions("Item selected from the list.")]
    [OutPortTypes("var")]
    class ListItemSelector : NodeModel
    {
        internal EngineController EngineController { get; set; }

        private ObservableCollection<object> _itemsCollection;
        public ObservableCollection<object> ItemsCollection
        {
            get { return _itemsCollection; }
            set
            {
                _itemsCollection = value;
                RaisePropertyChanged("ItemsCollection");
            }
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");

                OnNodeModified();
            }
        }

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (value > ItemsCollection.Count - 1)
                {
                    _selectedIndex = -1;
                }
                else
                {
                    _selectedIndex = value;
                }
                RaisePropertyChanged("SelectedIndex");
            }
        }

        private bool IsDeserialized = false;

        public event Action RequestChangeListItemSelector;

        protected virtual void OnRequestChangeListItemSelector()
        {
            if (RequestChangeListItemSelector != null)
            {
                RequestChangeListItemSelector();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ListItemSelector()
        {
            RegisterAllPorts();

            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            ItemsCollection = new ObservableCollection<object>();

            //ShouldDisplayPreviewCore = false;
        }

        void Connectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnRequestChangeListItemSelector();
        }

        public void PopulateItems(EngineController ec)
        {
            if (HasConnectedInput(0))
            {
                var minnode = InPorts[0].Connectors[0].Start.Owner;
                var minindex = InPorts[0].Connectors[0].Start.Index;

                var minid = minnode.GetAstIdentifierForOutputIndex(minindex).Name;

                var minmirror = this.EngineController.GetMirror(minid);

                if (minmirror.GetData().IsCollection)
                {
                    ItemsCollection = new ObservableCollection<object>(minmirror.GetData().GetElements().Select(x => x.Data));
                }
                else
                {
                    ItemsCollection.Add(minmirror.GetData().Data);
                }

                if (!IsDeserialized)
                    SelectedItem = ItemsCollection.FirstOrDefault();
                //else
                //    IsDeserialized = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        public override IEnumerable<ProtoCore.AST.AssociativeAST.AssociativeNode> BuildOutputAst(List<ProtoCore.AST.AssociativeAST.AssociativeNode> inputAstNodes)
        {
            if (!HasConnectedInput(0))
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode())
                };
            }

            if (SelectedItem == null)
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode())
                };
            }

            //var objectnode = AstFactory.BuildPrimitiveNodeFromObject(SelectedItem);
            AssociativeNode objectnode;
            if (SelectedItem.GetType() == typeof(int))
            {
                objectnode = AstFactory.BuildIntNode((int)SelectedItem);
            }
            else if (SelectedItem.GetType() == typeof(double))
            {
                objectnode = AstFactory.BuildDoubleNode((double)SelectedItem);
            }
            else if (SelectedItem.GetType() == typeof(bool))
            {
                objectnode = AstFactory.BuildBooleanNode((bool)SelectedItem);
            }
            else
            {
                objectnode = AstFactory.BuildStringNode(SelectedItem.ToString());
            }

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), objectnode)
            };
        }

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context);

            var xmldoc = element.OwnerDocument;
            var subnode = xmldoc.CreateElement("Celery.ListItemSelector");
            if (SelectedItem != null) subnode.SetAttribute("selectedindex", SaveSelectedIndex(SelectedIndex, ItemsCollection));
            if (subnode.Attributes.Count > 0) element.AppendChild(subnode);
        }

        protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.DeserializeCore(nodeElement, context);

            var attrib = nodeElement.Attributes["selectedindex"];
            if (attrib == null)
                return;

            _selectedIndex = ParseSelectedIndex(attrib.Value, ItemsCollection);

            if (_selectedIndex < 0)
            {
            }

            //foreach (XmlNode subnode in nodeElement.ChildNodes)
            //{
            //    if (!subnode.Name.Equals("Celery.ListItemSelector"))
            //    {
            //        continue;
            //    }
            //    if (subnode.Attributes == null || (subnode.Attributes.Count <= 0))
            //        continue;

            //    foreach (XmlAttribute attr in subnode.Attributes)
            //    {
            //        switch (attr.Name)
            //        {
            //            case "selecteditem":
            //                SelectedItem = Convert.ChangeType(attr.Value, typeof(object));
            //                break;
            //            default:
            //                Log(string.Format("{0} attribute could not be deserialized for {1}", attr.Name, GetType()));
            //                break;
            //        }
            //    }

            //    IsDeserialized = true;
            //}
        }

        protected override bool UpdateValueCore(UpdateValueParams updateValueParams)
        {
            string name = updateValueParams.PropertyName;
            string value = updateValueParams.PropertyValue;

            if (name == "Value" && value != null)
            {
                _selectedIndex = ParseSelectedIndex(value, ItemsCollection);
                if (_selectedIndex < 0)
                {
                }
                return true;
            }

            return base.UpdateValueCore(updateValueParams);
        }

        private int ParseSelectedIndex(string index, IList<object> items)
        {
            return ParseSelectedIndexImplement(index, items);
        }

        public static int ParseSelectedIndexImplement(string index, IList<object> items)
        {
            int selectedIndex = -1;

            var splits = index.Split(':');
            if (splits.Count() > 1)
            {
                var name = XmlUnescape(index.Substring(index.IndexOf(':') + 1));
                var item = items.FirstOrDefault(i => i.ToString() == name);
                selectedIndex = item != null ?
                    items.IndexOf(item) :
                    -1;
            }
            else
            {
                var tempIndex = Convert.ToInt32(index);
                selectedIndex = tempIndex > (items.Count - 1) ?
                    -1 :
                    tempIndex;
            }

            return selectedIndex;
        }

        private string SaveSelectedIndex(int index, IList<object> items)
        {
            return SaveSelectedIndexImplement(index, items);
        }

        public static string SaveSelectedIndexImplement(int index, IList<object> items)
        {
            if (index == -1 || items.Count == 0)
            {
                return "-1";
            }

            var item = items[index];
            return string.Format("{0}:{1}", index, XmlEscape(item.ToString()));
        }

        protected static string XmlEscape(string unescaped)
        {
            var doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }

        protected static string XmlUnescape(string escaped)
        {
            var doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }
    }

    class ListItemSelectorViewCustomization : INodeViewCustomization<ListItemSelector>
    {
        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;

        private ListItemSelector lismodel;
        private CeleryItemSelectorControl liscontrol;

        public void CustomizeView(ListItemSelector model, Dynamo.Controls.NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);

            lismodel = model;

            lismodel.EngineController = nodeView.ViewModel.DynamoViewModel.EngineController;

            liscontrol = new CeleryItemSelectorControl();
            nodeView.inputGrid.Children.Add(liscontrol);
            liscontrol.DataContext = model;

            model.RequestChangeListItemSelector += UpdateListItemSelector;

            UpdateListItemSelector();
        }

        private void UpdateListItemSelector()
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
