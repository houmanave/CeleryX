using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Graph;
using System.Xml;
using Dynamo.Engine;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;

namespace Celery.UI
{
    [IsDesignScriptCompatible]
    [NodeName("ListItemSelector")]
    [NodeCategory("Celery.ListControls")]
    [NodeDescription("Select an item from a list in the input with a drop-down control.")]
    [InPortNames("list")]
    [InPortDescriptions("Input list.")]
    [InPortTypes("var[]..[]")]
    [OutPortNames("item")]
    [OutPortDescriptions("Item selected from the list.")]
    [OutPortTypes("var")]
    public class ListItemSelectorX : NodeModel
    {
        public ListItemSelectorX()
        {
            RegisterAllPorts();

            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            PopulateItems();

            ShouldDisplayPreviewCore = false;
        }

        [JsonConstructor]
        ListItemSelectorX(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts) { }

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

        [JsonIgnore]
        public EngineController EngineControl = null;

        private string AttributeValue = string.Empty;

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

        public override IEnumerable<ProtoCore.AST.AssociativeAST.AssociativeNode> BuildOutputAst(List<ProtoCore.AST.AssociativeAST.AssociativeNode> inputAstNodes)
        {
            if (!InPorts[0].Connectors.Any())
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode())
                };
            }

            if (SelectedIndex == -1 || Items.Count == 0)
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode())
                };
            }
            
            AssociativeNode objectnode;
            object Obj = Items[SelectedIndex];
            if (Obj.GetType() == typeof(int))
            {
                objectnode = AstFactory.BuildIntNode((int)Obj);
            }
            else if (Obj.GetType() == typeof(double))
            {
                objectnode = AstFactory.BuildDoubleNode((double)Obj);
            }
            else if (Obj.GetType() == typeof(bool))
            {
                objectnode = AstFactory.BuildBooleanNode((bool)Obj);
            }
            else
            {
                objectnode = AstFactory.BuildStringNode(Obj.ToString());
            }

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), objectnode)
            };
        }

        //[Obsolete]
        //protected override void SerializeCore(XmlElement element, SaveContext context)
        //{
        //    base.SerializeCore(element, context);

        //    element.SetAttribute("index", SaveSelectedIndex(SelectedIndex, Items));
        //}

        //[Obsolete]
        //protected override void DeserializeCore(XmlElement nodeElement, SaveContext context)
        //{
        //    base.DeserializeCore(nodeElement, context);

        //    var att = nodeElement.Attributes["index"];
        //    if (att == null)
        //        return;

        //    AttributeValue = att.Value;
        //    _selectedIndex = ParseSelectedIndex(AttributeValue, Items);

        //    if (_selectedIndex < 0)
        //    {
        //        Warning(Dynamo.Properties.Resources.NothingIsSelectedWarning);
        //    }
        //}

        protected override bool UpdateValueCore(UpdateValueParams updateValueParams)
        {
            string name = updateValueParams.PropertyName;
            string value = updateValueParams.PropertyValue;

            if (name == "Value" && value != null)
            {
                _selectedIndex = ParseSelectedIndex(value, Items);
                if (_selectedIndex < 0)
                {
                    Warning(Dynamo.Properties.Resources.NothingIsSelectedWarning);
                }
                return true;
            }

            return base.UpdateValueCore(updateValueParams);
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

        public void PopulateItems()
        {
            if (InPorts[0].Connectors.Any())
            {
                var minnode = InPorts[0].Connectors[0].Start.Owner;
                var minindex = InPorts[0].Connectors[0].Start.Index;

                var minid = minnode.GetAstIdentifierForOutputIndex(minindex).Name;

                var minmirror = this.EngineControl.GetMirror(minid);

                if (Items != null)
                {
                    Items.Clear();
                }
                else
                {
                    Items = new ObservableCollection<CbxItem>();
                }
                if (minmirror.GetData().IsCollection)
                {
                    int i = 0;
                    foreach (var dat in minmirror.GetData().GetElements())
                    {
                        CbxItem cbi = new CbxItem(dat.Data.ToString(), dat);
                        Items.Add(cbi);
                        i++;
                    }
                }
                else
                {
                    var dat = minmirror.GetData().Data;
                    Items.Add(new CbxItem(dat.ToString(), dat));
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

        protected enum SelectionState
        {
            Done,
            Restore
        }

        protected SelectionState PopulateItemsCore(string currentSelection)
        {
            if (InPorts[0].Connectors.Any())
            {
                return SelectionState.Restore;
            }

            return SelectionState.Done;
        }
    }

    public class CbxItem : IComparable
    {
        public string Name { get; set; }
        public object Item { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public CbxItem(string name, object item)
        {
            Name = name;
            Item = item;
        }

        public int CompareTo(object obj)
        {
            var x = obj as CbxItem;
            if (x == null)
                return 1;

            return Name.CompareTo(x);
        }
    }
}
