using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Graph;
using System.Xml;
using System.Collections.ObjectModel;
using Dynamo.Wpf;
using Dynamo.Controls;
using Dynamo.ViewModels;
using System.Windows.Threading;
using Dynamo.Models;
using Celery.Controls;
using System.Windows;
using Celery.Controls.Subcontrols;
using System.Windows.Controls;
using Dynamo.Scheduler;
using System.ComponentModel;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Documents;
using Celery.Controls.SubcontrolClasses;
using Newtonsoft.Json;
using System.Collections;

namespace Celery.UI
{
    [IsDesignScriptCompatible]
    [NodeName("2DGraphItemSelector")]
    [NodeCategory("Celery.NumericControls")]
    [NodeDescription("An item selector node in 2D graph view for selecting a pair of items from the input list of lists.")]
    [InPortNames("number[][]", "header[]")]
    [InPortDescriptions("Two-dimensional matrix of numbers.", "List of labels.")]
    [InPortTypes("List<List<double>>", "List<string>")]
    [OutPortNames("outX", "outY", "outList")]
    [OutPortDescriptions("First item.", "Second item.", "Items in this index.")]
    [OutPortTypes("double", "double", "var[]")]
    //[OutPortNames("outX", "outY")]
    //[OutPortDescriptions("First item.", "Second item.")]
    //[OutPortTypes("double", "double", "List<double>")]
    class TwoDGraphItemSelector : NodeModel
    {
        internal EngineController EngineController { get; set; }

        private double OFFSETGAP = 5.0;

        private List<List<double>> _lldoubs;
        private Dictionary<string, int> _lilabels;
        private double _outDouble0;
        private double _outDouble1;
        private double _maxValueAtX;
        private double _minValueAtX;
        private double _maxValueAtY;
        private double _minValueAtY;
        private string _labelAtX;
        private string _labelAtY;
        private int _indexAtX;
        private int _indexAtY;
        private ObservableCollection<double> _listAtX;
        private ObservableCollection<double> _listAtY;
        private int _itemIndex;
        private List<double> _ldoubsAtIndex;

        [JsonIgnore]
        public List<List<double>> Lldoubs
        {
            get
            {
                return _lldoubs;
            }

            set
            {
                _lldoubs = value;

                //OnNodeModified();
            }
        }

        [JsonIgnore]
        public Dictionary<string, int> Lilabels
        {
            get
            {
                return _lilabels;
            }

            set
            {
                _lilabels = value;

                RaisePropertyChanged("Lilabels");

                //OnNodeModified();
            }
        }

        [JsonIgnore]
        public double OutDouble0
        {
            get
            {
                return _outDouble0;
            }
            set
            {
                _outDouble0 = value;

                //
                Debug.WriteLine("DOUBLE 0 - " + value.ToString());
                //
                //OnNodeModified();
            }
        }

        [JsonIgnore]
        public double OutDouble1
        {
            get
            {
                return _outDouble1;
            }
            set
            {
                _outDouble1 = value;

                //
                Debug.WriteLine("DOUBLE 1 - " + value.ToString());
                //
                //OnNodeModified();
            }
        }

        [JsonIgnore]
        public double MaxValueAtX
        {
            get
            {
                return _maxValueAtX;
            }

            set
            {
                _maxValueAtX = value;

                RaisePropertyChanged("MaxValueAtX");
                //
                OnNodeModified();
            }
        }

        [JsonIgnore]
        public double MinValueAtX
        {
            get
            {
                return _minValueAtX;
            }

            set
            {
                _minValueAtX = value;

                RaisePropertyChanged("MinValueAtX");
                //
                OnNodeModified();
            }
        }

        [JsonIgnore]
        public double MaxValueAtY
        {
            get
            {
                return _maxValueAtY;
            }

            set
            {
                _maxValueAtY = value;

                RaisePropertyChanged("MaxValueAtY");
                //
                OnNodeModified();
            }
        }

        [JsonIgnore]
        public double MinValueAtY
        {
            get
            {
                return _minValueAtY;
            }

            set
            {
                _minValueAtY = value;

                RaisePropertyChanged("MinValueAtY");
                //
                OnNodeModified();
            }
        }
        
        public string LabelAtX
        {
            get
            {
                return _labelAtX;
            }

            set
            {
                if (_labelAtX != value)
                {
                    _labelAtX = value;

                    PopulatePointsToGraph();

                    RaisePropertyChanged("LabelAtX");
                    //
                    OnNodeModified();
                }
            }
        }
        
        public string LabelAtY
        {
            get
            {
                return _labelAtY;
            }

            set
            {
                if (_labelAtY != value)
                {
                    _labelAtY = value;

                    PopulatePointsToGraph();

                    RaisePropertyChanged("LabelAtY");
                    //
                    OnNodeModified();
                }
            }
        }

        [JsonIgnore]
        public int IndexAtX
        {
            get
            {
                return _indexAtX;
            }

            set
            {
                _indexAtX = value;
            }
        }

        [JsonIgnore]
        public int IndexAtY
        {
            get
            {
                return _indexAtY;
            }

            set
            {
                _indexAtY = value;
            }
        }

        [JsonIgnore]
        public ObservableCollection<double> ListAtX
        {
            get
            {
                return _listAtX;
            }

            set
            {
                _listAtX = value;
            }
        }

        [JsonIgnore]
        public ObservableCollection<double> ListAtY
        {
            get
            {
                return _listAtY;
            }

            set
            {
                _listAtY = value;
            }
        }

        public int ItemIndex
        {
            get
            {
                return _itemIndex;
            }

            set
            {
                _itemIndex = value;
            }
        }

        [JsonIgnore]
        public List<double> LdoubsAtIndex
        {
            get
            {
                return _ldoubsAtIndex;
            }

            set
            {
                _ldoubsAtIndex = value;
            }
        }

        [JsonIgnore]
        public bool IsUntriggerComputeOutput { get; private set; }

        [JsonIgnore]
        public ObservableCollection<cegrPointItemThumb> ItemPoints { get; private set; }
        [JsonIgnore]
        public double CanvasSizeX { get; set; }
        [JsonIgnore]
        public double CanvasSizeY { get; set; }

        [JsonIgnore]
        public Canvas graphCanvas { get; set; }

        [JsonIgnore]
        public cegrPointItemThumb SelectedPointItem { get; private set; }

        [JsonIgnore]
        public List<gridline> GridLines { get; private set; }

        public event Action RequestChangeTwoDGraphItemSelectorValues;

        protected virtual void OnRequestChangeTwoDGraphItemSelectorValues()
        {
            //
            Debug.WriteLine("_onrequestchangetwographitemselectorvalues");
            //

            if (RequestChangeTwoDGraphItemSelectorValues != null)
                RequestChangeTwoDGraphItemSelectorValues();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TwoDGraphItemSelector()
        {
            RegisterAllPorts();

            this.PropertyChanged += TwoDGraphItemSelector_PropertyChanged;
            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            this.PortDisconnected += TwoDGraphItemSelector_PortDisconnected;

            ListAtX = new ObservableCollection<double>();
            ListAtY = new ObservableCollection<double>();

            ItemPoints = new ObservableCollection<cegrPointItemThumb>();

            Lldoubs = new List<List<double>>();
            Lilabels = new Dictionary<string, int>();
            LdoubsAtIndex = new List<double>();

            ItemIndex = 0;
        }

        [JsonConstructor]
        TwoDGraphItemSelector(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            this.PropertyChanged += TwoDGraphItemSelector_PropertyChanged;
            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            this.PortDisconnected += TwoDGraphItemSelector_PortDisconnected;

            //    ListAtX = new ObservableCollection<double>();
            //    ListAtY = new ObservableCollection<double>();

            //    ItemPoints = new ObservableCollection<cegrPointItemThumb>();

            //    Lldoubs = new List<List<double>>();
            //    Lilabels = new Dictionary<string, int>();
            //    LdoubsAtIndex = new List<double>();
        }

        private void TwoDGraphItemSelector_PortDisconnected(PortModel obj)
        {

        }

        //  2.0
        protected override void OnBuilt()
        {
            base.OnBuilt();
            VMDataBridge.DataBridge.Instance.RegisterCallback(GUID.ToString(), DataBridgeCallback);
        }

        public override void Dispose()
        {
            base.Dispose();
            VMDataBridge.DataBridge.Instance.UnregisterCallback(GUID.ToString());
        }

        private void DataBridgeCallback(object data)
        {
            ArrayList inputs = data as ArrayList;
            string inputText = "";
            foreach (var input in inputs)
            {
                inputText += input.ToString() + " ";
            }

            Debug.WriteLine(inputText);
        }
        //

        public void SetGridLines()
        {
            //  add gridlines
            GridLines = new List<gridline>();
            for (double i = 0.1; i < 1.0; i += 0.1)
            {
                gridline glx = new gridline(i, CanvasSizeX + OFFSETGAP * 2.0, CanvasSizeY + OFFSETGAP * 2.0);
                glx.PathCurve.Stroke = Brushes.DarkGray;
                gridline gly = new gridline(i, CanvasSizeX + OFFSETGAP * 2.0, CanvasSizeY + OFFSETGAP * 2.0, false);
                gly.PathCurve.Stroke = Brushes.DarkGray;

                GridLines.Add(glx);
                GridLines.Add(gly);
            }
        }

        private void Connectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //
            Debug.WriteLine("_COLLECTIONCHANGED");
            //

            OnRequestChangeTwoDGraphItemSelectorValues();
        }

        private void TwoDGraphItemSelector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsUntriggerComputeOutput)
            {
                //
                Debug.WriteLine("_propertychanged");
                //

                IsUntriggerComputeOutput = false;
                return;
            }

            //
            Debug.WriteLine("_PROPERTYCHANGED");
            //

            if (e.PropertyName != "CachedValue")
            {
                return;
            }
            if (InPorts.Any(x => x.Connectors.Count == 0))
            {
                return;
            }

            OnRequestChangeTwoDGraphItemSelectorValues();
        }

        public void ComputeOutput(EngineController controller)
        {
            //
            Debug.WriteLine("ComputeOutput");
            //

            //
            if (IsUntriggerComputeOutput)
            {
                IsUntriggerComputeOutput = false;
                return;
            }
            //

            if (InPorts[0].Connectors.Any() && InPorts[1].Connectors.Any())
            {
                var lilinode = InPorts[0].Connectors[0].Start.Owner;
                var liliidx = InPorts[0].Connectors[0].Start.Index;

                var liliid = lilinode.GetAstIdentifierForOutputIndex(liliidx).Name;

                List<object> lilio = null;

                var lilimirror = this.EngineController.GetMirror(liliid);
                if (lilimirror == null)
                {
                    return;
                }

                if (lilimirror.GetData().IsCollection)
                {
                    lilio = new List<object>();
                    var mirrorelems0 = lilimirror.GetData().GetElements().ToList();

                    if (mirrorelems0 == null)
                    {
                        return;
                    }

                    foreach (var inmirdata in mirrorelems0)
                    {
                        if (inmirdata.IsCollection)
                        {
                            List<object> datalist = inmirdata.GetElements().Select(x => x.Data).ToList();
                            lilio.Add(datalist);
                        }
                        else
                        {
                            //  data other than list not accepted
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }

                //  Finalize value here
                if (lilio != null)
                {
                    Lldoubs = new List<List<double>>();

                    //  Populate values
                    List<object> lst = lilio as List<object>;
                    if (lst != null)
                    {
                        foreach (object liobj in lst)
                        {
                            List<double> ldu = new List<double>();
                            List<object> lids = liobj as List<object>;
                            if (lids != null)
                            {
                                foreach (object obj in lids)
                                {
                                    double d = double.NaN;
                                    if (double.TryParse(obj.ToString(), out d))
                                    {
                                        ldu.Add(d);
                                    }
                                }
                            }
                            Lldoubs.Add(new List<double>(ldu));
                        }
                    }
                }
                else
                {
                    return;
                }

                //////////////////////////////////////////////////////////////
                var labelsnode = InPorts[1].Connectors[0].Start.Owner;
                var labelsidx = InPorts[1].Connectors[0].Start.Index;
                var labelsid = labelsnode.GetAstIdentifierForOutputIndex(labelsidx).Name;
                List<object> labelso = null;
                var labelsmirror = this.EngineController.GetMirror(labelsid);

                if (labelsmirror.GetData().IsCollection)
                {
                    labelso = new List<object>();
                    labelso = labelsmirror.GetData().GetElements().Select(x => x.Data).ToList();
                }
                else
                {
                    return;
                }

                if (labelso != null)
                {
                    Dictionary<string, int> dicdummy = new Dictionary<string, int>();

                    int i = 0;
                    foreach (object obj in labelso)
                    {
                        dicdummy.Add(obj.ToString(), i);
                        i++;
                    }

                    if (dicdummy != Lilabels)
                    {
                        Lilabels = new Dictionary<string, int>(dicdummy);

                        if (string.IsNullOrEmpty(LabelAtX) || !Lilabels.ContainsKey(LabelAtX))
                        {
                            LabelAtX = Lilabels.Keys.First();
                        }
                        if (string.IsNullOrEmpty(LabelAtY) || !Lilabels.ContainsKey(LabelAtY))
                        {
                            LabelAtY = Lilabels.Keys.Last();
                        }
                    }
                }

                //PopulatePointsToGraph();
            }
            else
            {
                //  clear anything from the canvas
                Lldoubs.Clear();

                Lilabels.Clear();

                ItemIndex = 0;
            }

            if (InPorts[1].Connectors.Any())
            {
                //var labelsnode = InPorts[1].Connectors[0].Start.Owner;
                //var labelsidx = InPorts[1].Connectors[0].Start.Index;
                //var labelsid = labelsnode.GetAstIdentifierForOutputIndex(labelsidx).Name;
                //List<object> labelso = null;
                //var labelsmirror = this.EngineController.GetMirror(labelsid);

                //if (labelsmirror.GetData().IsCollection)
                //{
                //    labelso = new List<object>();
                //    labelso = labelsmirror.GetData().GetElements().Select(x => x.Data).ToList();
                //}
                //else
                //{
                //    return;
                //}

                //if (labelso != null)
                //{
                //    Dictionary<string, int> dicdummy = new Dictionary<string, int>();

                //    int i = 0;
                //    foreach (object obj in labelso)
                //    {
                //        dicdummy.Add(obj.ToString(), i);
                //        i++;
                //    }

                //    if (dicdummy != Lilabels)
                //    {
                //        Lilabels = new Dictionary<string, int>(dicdummy);

                //        if (string.IsNullOrEmpty(LabelAtX) || !Lilabels.ContainsKey(LabelAtX))
                //        {
                //            LabelAtX = Lilabels.Keys.First();
                //        }
                //        if (string.IsNullOrEmpty(LabelAtY) || !Lilabels.ContainsKey(LabelAtY))
                //        {
                //            LabelAtY = Lilabels.Keys.Last();
                //        }
                //    }
                //}

                //PopulatePointsToGraph();
            }
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (!InPorts[0].Connectors.Any() || !InPorts[1].Connectors.Any())
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode()),
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1),AstFactory.BuildNullNode()),
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(2),AstFactory.BuildNullNode())
                };
            }

            if (LdoubsAtIndex == null)
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode()),
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1),AstFactory.BuildNullNode()),
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(2),AstFactory.BuildNullNode())
                };
            }

            AssociativeNode doublenode0 = AstFactory.BuildDoubleNode(OutDouble0);
            AssociativeNode doublenode1 = AstFactory.BuildDoubleNode(OutDouble1);

            List<AssociativeNode> listdoub = new List<AssociativeNode>();
            foreach (double d in LdoubsAtIndex)
            {
                AssociativeNode ad = AstFactory.BuildDoubleNode(d);
                listdoub.Add(ad);
            }
            AssociativeNode alistdoub = AstFactory.BuildExprList(listdoub);

            //
            Debug.WriteLine("BuildOutputAst");
            //

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), doublenode0),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), doublenode1),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(2), alistdoub)
            };
        }

        //  PRIVATE FUNCTIONS
        public void PopulatePointsToGraph()
        {
            //
            Debug.WriteLine("PopulatePointsToGraph");
            //

            if (graphCanvas == null)
            {
                return;
            }

            //  clear anything from the canvas
            graphCanvas.Children.Clear();

            foreach (gridline gl in GridLines)
            {
                Canvas.SetZIndex(gl.PathCurve, 1);
                graphCanvas.Children.Add(gl.PathCurve);
            }

            if (ItemPoints != null)
            {
                foreach (cegrPointItemThumb pt in ItemPoints)
                {
                    pt.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
                }
                ItemPoints.Clear();
            }
            else
            {
                ItemPoints = new ObservableCollection<cegrPointItemThumb>();
                //return;
            }

            if (string.IsNullOrEmpty(LabelAtX) || string.IsNullOrEmpty(LabelAtY) || Lilabels == null)
            {
                return;
            }
            if (Lilabels.ContainsKey(LabelAtX) && Lilabels.ContainsKey(LabelAtY))
            {
                IndexAtX = Lilabels[LabelAtX];
                IndexAtY = Lilabels[LabelAtY];

                ListAtX = new ObservableCollection<double>(Lldoubs[IndexAtX]);
                ListAtY = new ObservableCollection<double>(Lldoubs[IndexAtY]);

                List<double> orderedX = ListAtX.OrderBy(x => x).ToList();
                List<double> orderedY = ListAtY.OrderBy(x => x).ToList();

                MinValueAtX = orderedX.First();
                MinValueAtY = orderedY.First();
                MaxValueAtX = orderedX.Last();
                MaxValueAtY = orderedY.Last();

                if (ListAtX != null && ListAtX.Count > 0 &&
                    ListAtY != null && ListAtY.Count > 0)
                {
                    double propx = MaxValueAtX - MinValueAtX;
                    double propy = MaxValueAtY - MinValueAtY;

                    int maxnumitems = (ListAtX.Count > ListAtY.Count) ? ListAtX.Count : ListAtY.Count;
                    for (int i = 0; i < maxnumitems; i++)
                    {
                        double itemx = (i >= ListAtX.Count) ? ListAtX.Last() : ListAtX[i];
                        double itemy = (i >= ListAtY.Count) ? ListAtY.Last() : ListAtY[i];

                        double px = (itemx - MinValueAtX) * (CanvasSizeX - 0.0) / (MaxValueAtX - MinValueAtX);
                        double py = (itemy - MinValueAtY) * (CanvasSizeY - 0.0) / (MaxValueAtY - MinValueAtY);
                        px += 5.0;
                        py += 5.0;

                        //  10.0 is point size
                        py = CanvasSizeY - py + 10.0;

                        cegrPointItemThumb point = new cegrPointItemThumb(new Point(px, py), i, itemx, itemy, LabelAtX, LabelAtY);
                        Canvas.SetZIndex(point, 70);
                        graphCanvas.Children.Add(point);

                        //  add text
                        Canvas.SetZIndex(point.CoordText, 2000);
                        graphCanvas.Children.Add(point.CoordText);
                        point.CoordText.Visibility = Visibility.Hidden;
                        
                        point.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
                        point.MouseEnter += cegrPoint_MouseEnter;
                        point.MouseLeave += cegrPoint_MouseLeave;

                        ItemPoints.Add(point);
                    }

                    if (ItemPoints.Count > 0 && ItemPoints.Count > ItemIndex)
                    {
                        cegrPointItemThumb ctm = ItemPoints.Where(x => x.Index == ItemIndex).FirstOrDefault();
                        SelectPointItemFunction(ctm);
                        //OutDouble0 = ListAtX[ItemIndex];
                        //OutDouble1 = ListAtY[ItemIndex];
                    }
                }
            }
        }

        private void cegrPoint_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            cegrPointItemThumb el = sender as cegrPointItemThumb;
            if (el != null)
            {
                if (el.CoordText != null)
                {
                    el.CoordText.Visibility = Visibility.Hidden;
                }
            }
        }

        private void cegrPoint_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            cegrPointItemThumb el = sender as cegrPointItemThumb;
            if (el != null)
            {
                if (el.CoordText != null)
                {
                    el.CoordText.Visibility = Visibility.Visible;
                }
            }
        }

        public void CanvasPreviewMouseLeftUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //
            Debug.WriteLine("CanvasPreviewMouseUp");
            //

            cegrPointItemThumb pointitem = sender as cegrPointItemThumb;
            if (pointitem != null)
            {
                SelectPointItemFunction(pointitem);
            }
        }

        private void SelectPointItemFunction(cegrPointItemThumb pointItem)
        {
            if (SelectedPointItem != null)
            {
                SelectedPointItem.Background = SelectedPointItem.ColorDeselected;
            }

            int index = pointItem.Index;

            OutDouble0 = ListAtX[index];
            OutDouble1 = ListAtY[index];

            if (LdoubsAtIndex != null)
            {
                LdoubsAtIndex.Clear();
            }
            else
            {
                LdoubsAtIndex = new List<double>();
            }

            foreach (List<double> ld in Lldoubs)
            {
                double ldvalatindex = (ld.Count < index) ? ld.Last() : ld[index];
                LdoubsAtIndex.Add(ldvalatindex);
            }

            IsUntriggerComputeOutput = true;

            SelectedPointItem = pointItem;
            SelectedPointItem.Background = SelectedPointItem.ColorSelected;

            ItemIndex = SelectedPointItem.Index;

            OnNodeModified();
        }
    }

    class TwoDGraphItemSelectorViewCustomization : INodeViewCustomization<TwoDGraphItemSelector>
    {
        private double CANVAS_X = 488.0;
        private double CANVAS_Y = 488.0;

        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;
        private Celery2DGraphSelectorControl celeryItemSelectorControl;
        private TwoDGraphItemSelector twodgraphitemNode;
        private NodeViewModel nodeviewmodel;

        List<DependencyObject> hitResultsList = new List<DependencyObject>();

        public void CustomizeView(TwoDGraphItemSelector model, NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            nodeviewmodel = nodeView.ViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);
            twodgraphitemNode = model;

            model.EngineController = nodeView.ViewModel.DynamoViewModel.EngineController;

            celeryItemSelectorControl = new Celery2DGraphSelectorControl();

            nodeView.inputGrid.Children.Add(celeryItemSelectorControl);

            model.graphCanvas = celeryItemSelectorControl.thisCanvas;
            model.CanvasSizeX = CANVAS_X;
            model.CanvasSizeY = CANVAS_Y;

            model.SetGridLines();

            celeryItemSelectorControl.DataContext = model;

            //twodgraphitemNode.PopulatePointsToGraph();
            //celeryItemSelectorControl.thisCanvas.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;

            twodgraphitemNode.RequestChangeTwoDGraphItemSelectorValues += UpdateTwoDGraphReturnValues;

            UpdateTwoDGraphReturnValues();
        }

        private void ThisCanvasMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //
            Debug.WriteLine("xThisCanvasMouseLeftButtonUp");
            //

            hitresults.Clear();

            UIElement uiel = (UIElement)sender;
            Point position = e.GetPosition(uiel);
            VisualTreeHelper.HitTest(uiel,
                null/*new HitTestFilterCallback(OnHitTestFilterCallback)*/,
                new HitTestResultCallback(OnHitTestResultCallback),
                new PointHitTestParameters(position));

            if (hitresults.Count > 0)
            {
                foreach (var hititem in hitresults)
                {
                    cegrPointItemThumb g = hititem as cegrPointItemThumb;
                    if (g != null)
                    {
                        int index = g.Index;

                        //UpdateTwoDGraphReturnValues();

                        twodgraphitemNode.OutDouble0 = twodgraphitemNode.ListAtX[index];
                        twodgraphitemNode.OutDouble1 = twodgraphitemNode.ListAtY[index];

                        twodgraphitemNode.OnNodeModified();
                    }
                }
            }
        }

        private void CanvasPreviewMouseLeftUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //
            Debug.WriteLine("xCanvasPreviewMouseLeftUp");
            //

            //cegrPointItem pointitem = sender as cegrPointItem;
            //if (pointitem != null)
            //{
            //    int index = pointitem.Index;

            //    UpdateTwoDGraphReturnValues();

            //    twodgraphitemNode.OnNodeModified();
            //}

            hitresults.Clear();

            UIElement uiel = (UIElement)sender;
            Point position = e.GetPosition(uiel);
            VisualTreeHelper.HitTest(uiel, 
                null/*new HitTestFilterCallback(OnHitTestFilterCallback)*/,
                new HitTestResultCallback(OnHitTestResultCallback),
                new PointHitTestParameters(position));

            if (hitresults.Count > 0)
            {
                foreach (var hititem in hitresults)
                {
                    cegrPointItemThumb g = hititem as cegrPointItemThumb;
                    if (g != null)
                    {
                        int index = g.Index;

                        //UpdateTwoDGraphReturnValues();
                        twodgraphitemNode.OutDouble0 = twodgraphitemNode.ListAtX[index];
                        twodgraphitemNode.OutDouble1 = twodgraphitemNode.ListAtY[index];

                        twodgraphitemNode.OnNodeModified();
                    }
                }
            }
        }

        private void UpdateTwoDGraphReturnValues()
        {
            //
            Debug.WriteLine("UpdateTwoGraphReturnValues");
            //

            var s = dynamoViewmodel.Model.Scheduler;

            var t = new DelegateBasedAsyncTask(s, () =>
            {
                twodgraphitemNode.ComputeOutput(dynamoModel.EngineController);
            });

            t.ThenSend((_) =>
            {
                //  add the collected grid lines from the ComputeOutput method
                twodgraphitemNode.PopulatePointsToGraph();
            }, syncContext);

            s.ScheduleForExecution(t);
        }

        public void Dispose()
        {
            //
            Debug.WriteLine("Dispose");
            //
            
            celeryItemSelectorControl.thisCanvas.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
        }

        //  Hit tests
        private readonly List<DependencyObject> hitresults = new List<DependencyObject>();

        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            hitresults.Add(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }

        private HitTestFilterBehavior OnHitTestFilterCallback(DependencyObject target)
        {
            UIElement element = target as UIElement;
            if (element != null)
            {
                if (element.Visibility != Visibility.Visible || element.Opacity <= 0)
                {
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                }
            }

            return HitTestFilterBehavior.Continue;
        }
    }
}
