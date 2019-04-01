using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using RevElement = Revit.Elements.Element;

using Docman = RevitServices.Persistence.DocumentManager;
using Transman = RevitServices.Transactions.TransactionManager;
using System.Xml;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Graph;
using Autodesk.Revit.DB;

namespace Celery.UI
{
    [NodeName("Parameter Selector X")]
    [NodeCategory("Celery.RevitControls")]
    [NodeDescription("A list to select a parameter from the element.")]
    [InPortNames("Element")]
    [InPortDescriptions("Input element.")]
    [InPortTypes("var")]
    [OutPortNames("paramName", "paramValue")]
    [OutPortDescriptions("Name of the parameter selected.", "Value of the parameter selected.")]
    [OutPortTypes("string", "var")]
    [IsDesignScriptCompatible]
    public class ParameterSelectorX : NodeModel
    {
        private ObservableCollection<CbxItem> _items = new ObservableCollection<CbxItem>();
        public ObservableCollection<CbxItem> Items
        {
            get
            {
                return _items;
            }

            set
            {
                _items = value;
                RaisePropertyChanged("Items");
            }
        }

        private int _selectedIndex = 0;
        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }

            set
            {
                if (value > Items.Count - 1)
                {
                    _selectedIndex = -1;
                }
                else
                {
                    _selectedIndex = value;
                    if (Items.Count > 0)
                        AttributeValue = string.Format("{0}:{1}", _selectedIndex, XmlEscape((_items.ElementAt(_selectedIndex)).Name));
                }
                RaisePropertyChanged("SelectedIndex");
            }
        }

        public EngineController EngineControl
        {
            get; set;
        }

        private string AttributeValue = string.Empty;
        private RevElement Element;

        public ParameterSelectorX()
        {
            RegisterAllPorts();

            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            PopulateItems();

            ShouldDisplayPreviewCore = false;
        }

        void Connectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnRequestChangeListItemSelector();
        }

        public event Action RequestChangeListItemSelector;

        protected virtual void OnRequestChangeListItemSelector()
        {
            if (RequestChangeListItemSelector != null)
            {
                RequestChangeListItemSelector();
            }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            List<AssociativeNode> outputnodes = new List<AssociativeNode>();

            if (!HasConnectedInput(0))
            {
                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()));
                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildNullNode()));
            }
            else if (SelectedIndex == -1 || Items.Count == 0)
            {
                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode()));
                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), AstFactory.BuildNullNode()));
            }
            else
            {
                AssociativeNode parameternamenode;
                AssociativeNode parametervaluenode;
                
                var Obj = Items[SelectedIndex];

                parameternamenode = AstFactory.BuildStringNode(Obj.Name);
                if (Obj.Item != null)
                {
                    try
                    {
                        parametervaluenode = AstFactory.BuildPrimitiveNodeFromObject(Obj.Item);
                    }
                    catch
                    {
                        parametervaluenode = AstFactory.BuildStringNode(Obj.Item.ToString());
                    }
                }
                else
                {
                    parametervaluenode = AstFactory.BuildNullNode();
                }

                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), parameternamenode));
                outputnodes.Add(AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), parametervaluenode));
            }

            return outputnodes;
        }

        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context);

            element.SetAttribute("index", SaveSelectedIndex(SelectedIndex, Items));
        }

        protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        {
            base.DeserializeCore(nodeElement, context);

            var att = nodeElement.Attributes["index"];
            if (att == null)
                return;

            AttributeValue = att.Value;
            _selectedIndex = ParseSelectedIndex(AttributeValue, Items);

            if (_selectedIndex < 0)
            {
                Warning(Dynamo.Properties.Resources.NothingIsSelectedWarning);
            }
        }

        public void PopulateItems()
        {
            if (HasConnectedInput(0))
            {
                var minnode = InPorts[0].Connectors[0].Start.Owner;
                var minindex = InPorts[0].Connectors[0].Start.Index;

                var minid = minnode.GetAstIdentifierForOutputIndex(minindex).Name;

                var minmirror = this.EngineControl.GetMirror(minid);

                if (minmirror.GetData().IsCollection)
                {
                    //int i = 0;
                    //Items = new ObservableCollection<CbxItem>();
                    //foreach (var dat in minmirror.GetData().GetElements())
                    //{
                    //    CbxItem cbi = new CbxItem(dat.Data.ToString(), dat);
                    //    Items.Add(cbi);
                    //    i++;
                    //}
                }
                else
                {
                    var dat = minmirror.GetData().Data;

                    RevElement RevitElement = (RevElement)minmirror.GetData().Data;
                    if (RevitElement != null)
                    {
                        if (Items != null)
                        {
                            Items.Clear();
                        }
                        else
                        {
                            Items = new ObservableCollection<CbxItem>();
                        }

                        int i = 0;

                        Document doc = Docman.Instance.CurrentDBDocument;   //
                        //doc = Revit.Application.Document.Current
                        Transman.Instance.EnsureInTransaction(doc); //
                        foreach (Revit.Elements.Parameter param in RevitElement.Parameters)
                        {
                            Items.Add(new CbxItem(param.Name, param.Value));
                            i++;
                        }
                        Transman.Instance.TransactionTaskDone();    //
                    }
                }
            }

            if (!string.IsNullOrEmpty(AttributeValue))
                _selectedIndex = ParseSelectedIndex(AttributeValue, Items);

            var cursel = string.Empty;
            if (SelectedIndex >= 0 && (SelectedIndex < _items.Count))
            {
                cursel = _items.ElementAt(SelectedIndex).Name;
            }
            var selstate = PopulateItemsCore(cursel);
            if (selstate == SelectionState.Restore)
            {
                _selectedIndex = -1;
                for (int i = 0; i < _items.Count; i++)
                {
                    if ((_items.ElementAt(i)).Name.Equals(cursel))
                    {
                        SelectedIndex = i;
                    }
                }
            }
        }

        protected virtual int ParseSelectedIndex(string index, IList<CbxItem> items)
        {
            return ParseSelectedIndexImpl(index, items);
        }

        public static int ParseSelectedIndexImpl(string index, IList<CbxItem> items)
        {
            int selectedIndex = -1;

            var splits = index.Split(':');
            if (splits.Count() > 1)
            {
                var name = XmlUnescape(index.Substring(index.IndexOf(':') + 1));
                var item = items.FirstOrDefault(i => i.Name == name);
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

        protected virtual string SaveSelectedIndex(int index, IList<CbxItem> items)
        {
            return SaveSelectedIndexImpl(index, items);
        }

        public static string SaveSelectedIndexImpl(int index, IList<CbxItem> items)
        {
            if (index == -1 || items.Count == 0)
            {
                return "-1";
            }

            var item = items[index];
            return string.Format("{0}:{1}", index, XmlEscape(item.Name));
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

        protected enum SelectionState
        {
            Done,
            Restore
        }

        protected SelectionState PopulateItemsCore(string currentSelection)
        {
            if (HasConnectedInput(0))
            {
                return SelectionState.Restore;
            }

            return SelectionState.Done;
        }
    }
}
