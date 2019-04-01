using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Wpf;
using Celery.Controls;
using Dynamo.Controls;
using Dynamo.ViewModels;
using System.Windows.Threading;
using Dynamo.Models;
using Dynamo.Scheduler;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Markup;
using System.Reflection;
using Celery.Controls.Subcontrols;
using Celery.Controls.SubcontrolClasses;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows;
using Dynamo.Graph;
using System.Xml;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using Autodesk.DesignScript.Runtime;
using Newtonsoft.Json.Converters;
using Celery.Converters;

namespace Celery.UI
{
    [IsDesignScriptCompatible]
    [NodeName("GraphMap")]
    [NodeCategory("Celery.NumericControls")]
    [NodeDescription("A graph for mapping numerical values based on curves.")]
    [InPortNames("minLimit", "maxLimit", "count"/*, "<<"*/)]
    [InPortDescriptions("Minimum limit of the domain.", "Maximum limit of the domain.", "Number of values to be generated to the list."/*, "An input to connect from the output of the left-side GraphMap."*/)]
    [InPortTypes("double", "double", "int"/*, "string"*/)]
    [OutPortNames("numbers"/*, ">>"*/)]
    [OutPortDescriptions("Numerical values generated from the map."/*, "An output to connect to the input of the right-side GraphMap."*/)]
    [OutPortTypes("List<double>"/*,"string"*/)]
    class GraphMap : NodeModel
    {
        [JsonIgnore]
        internal EngineController EngineController { get; set; }

        private double _minLimit;
        private double _maxLimit;
        private int _numberOfValues;
        private double _passedValue;
        private CurveTypes _curveType;
        private List<double> _listOutputValues;
        private double _valueToPass;
        private List<gridline> _gridLines;
        private string _passedOutputString;

        private bool IsInitialized = false;
        private double GRID_LIMIT = 40;

        [JsonIgnore]
        public bool IsDeserialized = false;
        [JsonIgnore]
        public bool AreAllInputsAvailable = false;
        [JsonIgnore]
        public bool AreLimitInputsAvailable = false;
        [JsonIgnore]
        public bool IsCountInputAvailable = false;

        [JsonIgnore]
        public double MinLimit
        {
            get
            {
                return _minLimit;
            }

            set
            {
                _minLimit = value;
                RaisePropertyChanged("MinLimit");

                OnNodeModified();
            }
        }

        [JsonIgnore]
        public double MaxLimit
        {
            get
            {
                return _maxLimit;
            }

            set
            {
                _maxLimit = value;
                RaisePropertyChanged("MaxLimit");

                OnNodeModified();
            }
        }

        [JsonIgnore]
        public int NumberOfValues
        {
            get
            {
                return _numberOfValues;
            }

            set
            {
                _numberOfValues = value;

                GenerateOutputValues();

                RaisePropertyChanged("NumberOfValues");
            }
        }

        [JsonIgnore]
        public double PassedValue
        {
            get
            {
                return _passedValue;
            }

            set
            {
                _passedValue = value;
                RaisePropertyChanged("PassedValue");
            }
        }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public CurveTypes CurveType
        {
            get
            {
                return _curveType;
            }

            set
            {
                _curveType = value;

                if (!IsInitialized)
                {
                }

                GenerateOutputValues();

                RaisePropertyChanged("CurveType");

                OnNodeModified();
            }
        }

        [JsonIgnore]
        public List<double> ListOutputValues
        {
            get
            {
                return _listOutputValues;
            }

            set
            {
                _listOutputValues = value;

                OnNodeModified();
            }
        }

        [JsonIgnore]
        public double ValueToPass
        {
            get
            {
                return _valueToPass;
            }

            set
            {
                _valueToPass = value;
                RaisePropertyChanged("ValueToPass");
            }
        }

        [JsonIgnore]
        public List<gridline> GridLines
        {
            get
            {
                return _gridLines;
            }

            set
            {
                _gridLines = value;
            }
        }

        [JsonIgnore]
        public Canvas canvas { get; set; }

        [JsonIgnore]
        public string PassedOutputString
        {
            get
            {
                return _passedOutputString;
            }

            set
            {
                _passedOutputString = value;
                RaisePropertyChanged("PassedOutputString");
            }
        }

        //  Internal Variables
        //  sine curve
        private cecoPointFree _pointFreeSine1;
        private cecoPointFree _pointFreeSine2;

        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointFree PointFreeSine1
        {
            get
            {
                return _pointFreeSine1;
            }
            set
            {
                _pointFreeSine1 = value;

                OnNodeModified();
            }
        }

        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointFree PointFreeSine2
        {
            get
            {
                return _pointFreeSine2;
            }
            set
            {
                _pointFreeSine2 = value;

                OnNodeModified();
            }
        }
        [JsonIgnore]
        public sinecurve CurveSine { get; set; }

        //  bezier curve
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointEnd PointBezier1 { get; set; }
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointEnd PointBezier2 { get; set; }
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointControl PointBezCont1 { get; set; }
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointControl PointBezCont2 { get; set; }
        [JsonIgnore]
        public beziercurve CurveBezier { get; set; }
        [JsonIgnore]
        public controlline CurveBezControlLine1 { get; set; }
        [JsonIgnore]
        public controlline CurveBezControlLine2 { get; set; }

        //  linear curve
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointFree PointLinear1 { get; set; }
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointFree PointLinear2 { get; set; }
        [JsonIgnore]
        public linearcurve CurveLinear { get; set; }

        //  parabolic curve
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointFree PointParabolic1 { get; set; }
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointFree PointParabolic2 { get; set; }
        [JsonIgnore]
        public paraboliccurve CurveParabolic { get; set; }

        //  perlin noise curve
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointOrtho PointPerlin01 { get; set; }
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointOrtho PointPerlin02 { get; set; }
        [JsonConverter(typeof(StringToPointThumbConverter))]
        public cecoPointFree PointPerlin11 { get; set; }
        [JsonIgnore]
        public perlinnoisecurve CurvePerlinNoise { get; set; }

        //  CANVAS SIZE
        [JsonIgnore]
        public double CanvasWidth = 0.0;
        [JsonIgnore]
        public double CanvasHeight = 0.0;

        [JsonIgnore]
        public bool IsUntriggerComputeOutput { get; private set; }

        public event Action RequestChangeGraphMapValues;

        protected virtual void OnRequestChangeMapValues()
        {
            if (RequestChangeGraphMapValues != null)
                RequestChangeGraphMapValues();
        }

        private void InitializeGraph()
        {
        }

        private void GraphMap_PortDisconnected(PortModel obj)
        {
            
        }

        public GraphMap()
        {
            RegisterAllPorts();

            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            this.PortDisconnected += GraphMap_PortDisconnected;

            this.PropertyChanged += GraphMap_PropertyChanged;

            ArgumentLacing = LacingStrategy.Disabled;

            CurveType = CurveTypes.Linear;
            GridLines = new List<gridline>();

            //
            if (this.CanvasWidth == 0.0)
                this.CanvasWidth = 294.0;
            if (this.CanvasHeight == 0.0)
                this.CanvasHeight = 258.0;

            this.PointBezCont1 = new cecoPointControl(new System.Windows.Point(50, 100));
            this.PointBezCont2 = new cecoPointControl(new System.Windows.Point(this.CanvasWidth - 50, 100));
            this.PointBezier1 = new cecoPointEnd(new System.Windows.Point(0, CanvasHeight));
            this.PointBezier2 = new cecoPointEnd(new System.Windows.Point(this.CanvasWidth, this.CanvasHeight), false);

            this.PointFreeSine1 = new cecoPointFree(new System.Windows.Point(0, 0));
            this.PointFreeSine2 = new cecoPointFree(new System.Windows.Point(this.CanvasWidth, this.CanvasHeight));

            this.PointLinear1 = new cecoPointFree(new System.Windows.Point(0, this.CanvasHeight));
            this.PointLinear2 = new cecoPointFree(new System.Windows.Point(this.CanvasWidth, 0));

            this.PointParabolic1 = new cecoPointFree(new Point(this.CanvasWidth / 2.0, 0));
            this.PointParabolic2 = new cecoPointFree(new Point(this.CanvasWidth, this.CanvasHeight));

            this.PointPerlin01 = new cecoPointOrtho(new Point(this.CanvasWidth / 2.0, 0.0), 0.0, 0.0, false);
            this.PointPerlin02 = new cecoPointOrtho(new Point(1.0, this.CanvasHeight), 0.0, 0.0, true);
            this.PointPerlin11 = new cecoPointFree(new Point(this.CanvasWidth / 2.0, this.CanvasHeight / 2.0));
            //
        }

        [JsonConstructor]
        GraphMap(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            this.PortDisconnected += GraphMap_PortDisconnected;

            this.PropertyChanged += GraphMap_PropertyChanged;

            ArgumentLacing = LacingStrategy.Disabled;

            CurveType = CurveTypes.Linear;
            GridLines = new List<gridline>();
        }

        private void GraphMap_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsUntriggerComputeOutput)
            {
                //
                Debug.WriteLine("_propertychanged");
                //

                IsUntriggerComputeOutput = false;
                return;
            }

            if (e.PropertyName != "CachedValue")
            {
                return;
            }
            if (InPorts.Any(x => x.Connectors.Count == 0))
            {
                return;
            }

            RequestChangeGraphMapValues();
        }

        private void Connectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnRequestChangeMapValues();
        }

        #region new for 2.0 compliance
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
            //string inputtext = "";
            //foreach (var input in inputs)
            //{
            //    inputtext += input.ToString() + " ";
            //}
        }
        #endregion

        /// <summary>
        /// Function to compute outputs from node inputs.
        /// </summary>
        /// <param name="engine">The engine controller to be used to process inputs and outputs on the screen.</param>
        public void ComputeOutput(EngineController engine)
        {
            //
            if (IsUntriggerComputeOutput)
            {
                IsUntriggerComputeOutput = false;
                return;
            }
            //

            if (InPorts[0].Connectors.Any() && InPorts[1].Connectors.Any())
            {
                var minnode = InPorts[0].Connectors[0].Start.Owner;
                var minindex = InPorts[0].Connectors[0].Start.Index;
                var maxnode = InPorts[1].Connectors[0].Start.Owner;
                var maxindex = InPorts[1].Connectors[0].Start.Index;

                var minid = minnode.GetAstIdentifierForOutputIndex(minindex).Name;
                var maxid = maxnode.GetAstIdentifierForOutputIndex(maxindex).Name;

                var minmirror = this.EngineController.GetMirror(minid);
                var maxmirror = this.EngineController.GetMirror(maxid);

                object mino = null;
                object maxo = null;

                if (minmirror.GetData().IsCollection)
                {
                    mino = minmirror.GetData().GetElements().Select(x => x.Data).FirstOrDefault();
                }
                else
                {
                    mino = minmirror.GetData().Data;
                }

                if (maxmirror.GetData().IsCollection)
                {
                    maxo = maxmirror.GetData().GetElements().Select(x => x.Data).FirstOrDefault();
                }
                else
                {
                    maxo = maxmirror.GetData().Data;
                }

                //  this is to prevent any calculation out of null values.
                if (mino == null || maxo == null)
                {
                    IsUntriggerComputeOutput = true;
                    return;
                }

                double parsed;
                MinLimit = TryConvertToDouble(mino, out parsed) ? parsed : 0.0;
                MaxLimit = TryConvertToDouble(maxo, out parsed) ? parsed : 0.0;

                GenerateOutputValues();

                AreLimitInputsAvailable = true;
            }
            else
            {
                AreLimitInputsAvailable = false;
            }

            if (InPorts[2].Connectors.Any())
            {
                var nnode = InPorts[2].Connectors[0].Start.Owner;
                var nindex = InPorts[2].Connectors[0].Start.Index;

                var nid = nnode.GetAstIdentifierForOutputIndex(nindex).Name;

                var nmirror = this.EngineController.GetMirror(nid);

                object no = null;

                if (nmirror.GetData().IsCollection)
                {
                    no = nmirror.GetData().GetElements().Select(x => x.Data).FirstOrDefault();
                }
                else
                {
                    no = nmirror.GetData().Data;
                }

                double parsed;
                PassedValue = TryConvertToDouble(no, out parsed) ? parsed : 0.0;
                NumberOfValues = (int)PassedValue;

                GenerateOutputValues();

                if ((NumberOfValues - 1) * 2 != GridLines.Count)
                {
                    //  remove grid lines
                    RemoveGridLines();

                    //  draw grid lines
                    if (parsed > GRID_LIMIT) parsed = GRID_LIMIT;
                    double heightdiff = 1.0 / parsed;
                    double widthdiff = 1.0 / parsed;
                    for (double d = heightdiff; d < 1.0; d += heightdiff)
                    {
                        gridline gl = new gridline(d, CanvasWidth, CanvasHeight);
                        GridLines.Add(gl);
                    }
                    for (double d = widthdiff; d < 1.0; d += heightdiff)
                    {
                        gridline gl = new gridline(d, CanvasWidth, CanvasHeight, false);
                        GridLines.Add(gl);
                    }
                }

                IsCountInputAvailable = true;
            }
            else
            {
                RemoveGridLines();
                IsCountInputAvailable = false;
            }

            AreAllInputsAvailable = (IsCountInputAvailable && AreLimitInputsAvailable);
            
            //
            IsUntriggerComputeOutput = true;

            //if (HasConnectedInput(3))
            //{
            //    var vnode = InPorts[3].Connectors[0].Start.Owner;
            //    var vindex = InPorts[3].Connectors[0].Start.Index;

            //    var vid = vnode.GetAstIdentifierForOutputIndex(vindex).Name;

            //    var vmirror = this.EngineController.GetMirror(vid);

            //    object vo = null;

            //    if (vmirror.GetData().IsCollection)
            //    {
            //        vo = vmirror.GetData().GetElements().Select(x => x.Data).FirstOrDefault();
            //    }
            //    else
            //    {
            //        vo = vmirror.GetData().Data;
            //    }

            //    //  overwrite the inputs processed from the first set of inputs


            //    ProcessPassingStringFromInputs();
            //}
            //else
            //{
            //    ProcessPassingStringFromInputs();
            //}
        }

        #region new serpalization for 2.0
        //protected override bool UpdateValueCore(UpdateValueParams updateValueParams)
        //{
        //    string name = updateValueParams.PropertyName;
        //    string value = updateValueParams.PropertyValue;

        //    switch (name)
        //    {
        //        case "PointFreeSine1":
        //            this.PointFreeSine1 = this.DeserializeValue(value);
        //            return true;
        //        case "PointFreeSine2":
        //            this.PointFreeSine2 = this.DeserializeValue(value);
        //            return true;
        //        case "PointBezier1":
        //            this.PointBezier1 = this.DeserializeValue(value);
        //            return true;
        //        case "PointBezier2":
        //            this.PointBezier2 = this.DeserializeValue(value);
        //            return true;
        //        case "PointBezCont1":
        //            this.PointBezCont1 = this.DeserializeValue(value);
        //            return true;
        //        case "PointBezCont2":
        //            this.PointBezCont2 = this.DeserializeValue(value);
        //            return true;
        //        case "PointLinear1":
        //            this.PointLinear1 = this.DeserializeValue(value);
        //            return true;
        //        case "PointLinear2":
        //            this.PointLinear2 = this.DeserializeValue(value);
        //            return true;
        //        case "PointParabolic1":
        //            this.PointParabolic1 = this.DeserializeValue(value);
        //            return true;
        //        case "PointParabolic2":
        //            this.PointParabolic2 = this.DeserializeValue(value);
        //            return true;
        //        case "PointPerlin01":
        //            this.PointPerlin01 = this.DeserializeValue(value);
        //            return true;
        //        case "PointPerlin02":
        //            this.PointPerlin02 = this.DeserializeValue(value);
        //            return true;
        //        case "PointPerlin11":
        //            this.PointPerlin11 = this.DeserializeValue(value);
        //            return true;
        //        default:
        //            break;
        //    }

        //    return base.UpdateValueCore(updateValueParams); 
        //}

        //private cecoPointFree DeserializeValuePointFree(string value)
        //{
        //    try
        //    {
        //        string[] pointstr = value.Split(',');
        //        return new cecoPointFree(new Point(
        //            double.Parse(pointstr[0]),
        //            double.Parse(pointstr[1])
        //            ));
        //    }
        //    catch
        //    {
        //        return new cecoPointFree(new Point(0.0, 0.0));
        //    }
        //}

        //private cecoPointEnd DeserializeValuePointEnd(string value)
        //{
        //    try
        //    {
        //        string[] pointstr = value.Split(',');
        //        return new cecoPointEnd(new Point(
        //            double.Parse(pointstr[0]),
        //            double.Parse(pointstr[1])
        //            ),
        //            bool.Parse(pointstr[2])
        //            );
        //    }
        //    catch
        //    {
        //        return new cecoPointEnd(new Point(0.0, 0.0));
        //    }
        //}

        //private cecoPointControl DeserializeValuePointControl(string value)
        //{
        //    try
        //    {
        //        string[] pointstr = value.Split(',');
        //        return new cecoPointControl(new Point(
        //            double.Parse(pointstr[0]),
        //            double.Parse(pointstr[1])
        //            ));
        //    }
        //    catch
        //    {
        //        return new cecoPointControl(new Point(0.0, 0.0));
        //    }
        //}

        //private cecoPointOrtho DeserializeValuePointOrtho(string value)
        //{
        //    try
        //    {
        //        string[] pointstr = value.Split(',');
        //        return new cecoPointOrtho(new Point(
        //            double.Parse(pointstr[0]),
        //            double.Parse(pointstr[1])
        //            ),
        //            double.Parse(pointstr[2]),
        //            double.Parse(pointstr[3]),
        //            bool.Parse(pointstr[4])
        //            );
        //    }
        //    catch
        //    {
        //        return new cecoPointOrtho(new Point(0.0, 0.0), 0.0, 0.0, true);
        //    }
        //}

        //private string SerializeValue(cecoPointFree p)
        //{
        //    return p.ToString();
        //}

        //private string SerializeValue(cecoPointEnd p)
        //{
        //    return p.ToString();
        //}

        //private string SerializeValue(cecoPointControl p)
        //{
        //    return p.ToString();
        //}

        //private string SerializeValue(cecoPointOrtho p)
        //{
        //    return p.ToString();
        //}
        #endregion

        private void GenerateOutputValues()
        {
            switch (CurveType)
            {
                case CurveTypes.Bezier:
                    if (CurveBezier != null)
                    {
                        ListOutputValues = CurveBezier.GetValuesFromAssignedParameters(MinLimit, MaxLimit, NumberOfValues);
                    }
                    break;
                case CurveTypes.Linear:
                    if (CurveLinear != null)
                    {
                        ListOutputValues = CurveLinear.GetValuesFromAssignedParameters(MinLimit, MaxLimit, NumberOfValues);
                    }
                    break;
                case CurveTypes.Sine:
                    if (CurveSine != null)
                    {
                        ListOutputValues = CurveSine.GetValuesFromAssignedParameters(MinLimit, MaxLimit, NumberOfValues);
                    }
                    break;
                case CurveTypes.Parabola:
                    if (CurveParabolic != null)
                    {
                        ListOutputValues = CurveParabolic.GetValuesFromAssignedParameters(MinLimit, MaxLimit, NumberOfValues);
                    }
                    break;
                case CurveTypes.PerlinNoise:
                    if (CurvePerlinNoise != null)
                    {
                        ListOutputValues = CurvePerlinNoise.GetValuesFromAssignedParameters(MinLimit, MaxLimit, NumberOfValues);
                    }
                    break;
                default:
                    break;
            }
        }

        private void ProcessPassingStringFromInputs()
        {
            PassedOutputString = "HOMER";
        }

        /// <summary>
        /// Grid Line Cleanup
        /// </summary>
        private void RemoveGridLines()
        {
            if (GridLines.Count > 0 || canvas != null)
            {
                foreach (gridline g in GridLines)
                {
                    if (canvas.Children.Contains(g.PathCurve))
                        canvas.Children.Remove(g.PathCurve);
                }
                GridLines.Clear();
            }
        }

        /// <summary>
        /// Convert an object to a double-precision float.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parsed"></param>
        /// <returns></returns>
        private static bool TryConvertToDouble(object value, out double parsed)
        {
            parsed = default(double);

            try
            {
                parsed = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            //if (IsPartiallyApplied)
            //{
            //}

            if (!InPorts[0].Connectors.Any() || !InPorts[1].Connectors.Any() || !InPorts[2].Connectors.Any())
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode())
                    //, AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1),AstFactory.BuildNullNode())
                };
            }

            if (ListOutputValues == null)
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode())
                    //, AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1),AstFactory.BuildNullNode())
                };
            }

            var pdoublist = new List<AssociativeNode>();
            foreach (double dval in ListOutputValues)
            {
                pdoublist.Add(AstFactory.BuildDoubleNode(dval));
            }

            var listnode = AstFactory.BuildExprList(pdoublist);
            //var stringnode = AstFactory.BuildStringNode(string.IsNullOrWhiteSpace(PassedOutputString) ? "" : PassedOutputString);

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), listnode)
                //, AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), stringnode)
            };
        }
    }

    class GraphMapViewCustomization : INodeViewCustomization<GraphMap>
    {
        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;
        private CeleryGraphControl celeryGraphControl;
        private GraphMap graphmapNode;
        private NodeViewModel nodeviewmodel;

        List<DependencyObject> hitResultsList = new List<DependencyObject>();

        public void CustomizeView(GraphMap model, NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            nodeviewmodel = nodeView.ViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);
            graphmapNode = model;

            model.EngineController = nodeView.ViewModel.DynamoViewModel.EngineController;

            celeryGraphControl = new CeleryGraphControl(model, nodeView);
            if(celeryGraphControl.thisCanvas.IsInitialized)
            {

            }

            if (model.CanvasWidth == 0.0)
                model.CanvasWidth = 294.0;
            if (model.CanvasHeight == 0.0)
                model.CanvasHeight = 258.0;

            if (celeryGraphControl.thisCanvas.ActualWidth > 0.0)
                model.CanvasWidth = celeryGraphControl.thisCanvas.ActualWidth;
            if (celeryGraphControl.thisCanvas.ActualHeight > 0.0)
                model.CanvasHeight = celeryGraphControl.thisCanvas.ActualHeight;

            //  points
            //  bezier
            Canvas.SetZIndex(model.PointBezCont1, 60);
            model.PointBezCont1.maxwidth = model.CanvasWidth;
            model.PointBezCont1.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointBezCont1);

            Canvas.SetZIndex(model.PointBezCont2, 60);
            model.PointBezCont2.maxwidth = model.CanvasWidth;
            model.PointBezCont2.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointBezCont2);

            Canvas.SetZIndex(model.PointBezier1, 75);
            model.PointBezier1.maxwidth = model.CanvasWidth;
            model.PointBezier1.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointBezier1);
            
            Canvas.SetZIndex(model.PointBezier2, 75);
            model.PointBezier2.maxwidth = model.CanvasWidth;
            model.PointBezier2.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointBezier2);

            //  sine wave
            Canvas.SetZIndex(model.PointFreeSine1, 76);
            model.PointFreeSine1.maxwidth = model.CanvasWidth;
            model.PointFreeSine1.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointFreeSine1);

            Canvas.SetZIndex(model.PointFreeSine2, 76);
            model.PointFreeSine2.maxwidth = model.CanvasWidth;
            model.PointFreeSine2.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointFreeSine2);

            //  linear
            Canvas.SetZIndex(model.PointLinear1, 74);
            model.PointLinear1.maxwidth = model.CanvasWidth;
            model.PointLinear1.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointLinear1);

            Canvas.SetZIndex(model.PointLinear2, 74);
            model.PointLinear2.maxwidth = model.CanvasWidth;
            model.PointLinear2.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointLinear2);

            //  parabola
            Canvas.SetZIndex(model.PointParabolic1, 78);
            model.PointParabolic1.maxwidth = model.CanvasWidth;
            model.PointParabolic1.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointParabolic1);

            Canvas.SetZIndex(model.PointParabolic2, 78);
            model.PointParabolic2.maxwidth = model.CanvasWidth;
            model.PointParabolic2.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointParabolic2);

            //  perlin noise
            Canvas.SetZIndex(model.PointPerlin01, 77);
            model.PointPerlin01.maxwidth = model.CanvasWidth;
            model.PointPerlin01.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointPerlin01);

            Canvas.SetZIndex(model.PointPerlin02, 77);
            model.PointPerlin02.maxwidth = model.CanvasWidth;
            model.PointPerlin02.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointPerlin02);

            Canvas.SetZIndex(model.PointPerlin11, 77);
            model.PointPerlin11.maxwidth = model.CanvasWidth;
            model.PointPerlin11.maxheight = model.CanvasHeight;
            celeryGraphControl.thisCanvas.Children.Add(model.PointPerlin11);

            //  create the curves
            model.CurveBezControlLine1 = new controlline(
                model.PointBezier1.Point,
                model.PointBezCont1.Point);
            Canvas.SetZIndex(model.CurveBezControlLine1, 25);
            model.CurveBezControlLine2 = new controlline(
                model.PointBezier2.Point,
                model.PointBezCont2.Point);
            Canvas.SetZIndex(model.CurveBezControlLine2, 25);
            model.CurveBezier = new beziercurve(
                model.PointBezier1,
                model.PointBezier2,
                model.PointBezCont1,
                model.PointBezCont2,
                model.CanvasWidth,
                model.CanvasHeight);
            Canvas.SetZIndex(model.CurveBezier, 50);

            model.CurveLinear = new linearcurve(
                model.PointLinear1,
                model.PointLinear2,
                model.CanvasWidth,
                model.CanvasHeight);
            Canvas.SetZIndex(model.CurveLinear, 50);

            model.CurveSine = new sinecurve(
                celeryGraphControl.thisCanvas,
                model.PointFreeSine1,
                model.PointFreeSine2,
                model.CanvasWidth,
                model.CanvasHeight);
            Canvas.SetZIndex(model.CurveSine, 49);

            model.CurveParabolic = new paraboliccurve(
                celeryGraphControl.thisCanvas,
                model.PointParabolic1,
                model.PointParabolic2,
                model.CanvasWidth,
                model.CanvasHeight);
            Canvas.SetZIndex(model.CurveParabolic, 48);

            model.CurvePerlinNoise = new perlinnoisecurve(
                celeryGraphControl.thisCanvas,
                model.PointPerlin01,
                model.PointPerlin02,
                model.PointPerlin11,
                1,
                model.CanvasWidth,
                model.CanvasHeight);
            Canvas.SetZIndex(model.CurvePerlinNoise, 47);

            //  add path curves to the canvas
            celeryGraphControl.thisCanvas.Children.Add(model.CurveBezier.PathCurve);
            celeryGraphControl.thisCanvas.Children.Add(model.CurveBezControlLine1.PathCurve);
            celeryGraphControl.thisCanvas.Children.Add(model.CurveBezControlLine2.PathCurve);
            celeryGraphControl.thisCanvas.Children.Add(model.CurveLinear.PathCurve);
            celeryGraphControl.thisCanvas.Children.Add(model.CurveSine.PathCurve);
            celeryGraphControl.thisCanvas.Children.Add(model.CurveParabolic.PathCurve);
            celeryGraphControl.thisCanvas.Children.Add(model.CurvePerlinNoise.PathCurve);

            //  assign curves to ceco points
            model.PointBezier1.bcurve = model.CurveBezier;
            model.PointBezier2.bcurve = model.CurveBezier;
            model.PointBezier1.cline = model.CurveBezControlLine1;
            model.PointBezier2.cline = model.CurveBezControlLine2;

            model.PointBezCont1.cline = model.CurveBezControlLine1;
            model.PointBezCont2.cline = model.CurveBezControlLine2;
            model.PointBezCont1.bcurve = model.CurveBezier;
            model.PointBezCont2.bcurve = model.CurveBezier;

            model.PointLinear1.lcurve = model.CurveLinear;
            model.PointLinear2.lcurve = model.CurveLinear;

            model.PointFreeSine1.scurve = model.CurveSine;
            model.PointFreeSine2.scurve = model.CurveSine;

            model.PointParabolic1.parabcurve = model.CurveParabolic;
            model.PointParabolic2.parabcurve = model.CurveParabolic;

            model.PointPerlin01.perlcurve = model.CurvePerlinNoise;
            model.PointPerlin02.perlcurve = model.CurvePerlinNoise;
            model.PointPerlin11.perlcurve = model.CurvePerlinNoise;

            //  attach bindings

            //  perlin noise curve
            Binding bndperl = new Binding("CurveType");
            bndperl.Mode = BindingMode.TwoWay;
            bndperl.Converter = new EnumToVisibilityConverter();
            bndperl.ConverterParameter = CurveTypes.PerlinNoise;
            model.CurvePerlinNoise.PathCurve.SetBinding(Path.VisibilityProperty, bndperl);
            model.PointPerlin01.SetBinding(UserControl.VisibilityProperty, bndperl);
            model.PointPerlin02.SetBinding(UserControl.VisibilityProperty, bndperl);
            model.PointPerlin11.SetBinding(UserControl.VisibilityProperty, bndperl);

            //  parabolic curve
            Binding bndparab = new Binding("CurveType");
            bndparab.Mode = BindingMode.TwoWay;
            bndparab.Converter = new EnumToVisibilityConverter();
            bndparab.ConverterParameter = CurveTypes.Parabola;
            model.CurveParabolic.PathCurve.SetBinding(Path.VisibilityProperty, bndparab);
            model.PointParabolic1.SetBinding(UserControl.VisibilityProperty, bndparab);
            model.PointParabolic2.SetBinding(UserControl.VisibilityProperty, bndparab);

            //  linear curve
            Binding bndlinear = new Binding("CurveType");
            bndlinear.Mode = BindingMode.TwoWay;
            bndlinear.Converter = new EnumToVisibilityConverter();
            bndlinear.ConverterParameter = CurveTypes.Linear;
            model.CurveLinear.PathCurve.SetBinding(Path.VisibilityProperty, bndlinear);
            model.PointLinear1.SetBinding(UserControl.VisibilityProperty, bndlinear);
            model.PointLinear2.SetBinding(UserControl.VisibilityProperty, bndlinear);

            //  redraw the curves
            model.CurveLinear.Regenerate(model.PointLinear1);
            model.CurveLinear.Regenerate(model.PointLinear2);

            //  sine curve
            Binding bndsine = new Binding("CurveType");
            bndsine.Mode = BindingMode.TwoWay;
            bndsine.Converter = new EnumToVisibilityConverter();
            bndsine.ConverterParameter = CurveTypes.Sine;
            model.CurveSine.PathCurve.SetBinding(Path.VisibilityProperty, bndsine);
            model.PointFreeSine1.SetBinding(UserControl.VisibilityProperty, bndsine);
            model.PointFreeSine2.SetBinding(UserControl.VisibilityProperty, bndsine);

            model.CurveSine.Regenerate(model.PointFreeSine1);
            model.CurveSine.Regenerate(model.PointFreeSine2);

            //  bezier curve
            Binding bndbezier = new Binding("CurveType");
            bndbezier.Mode = BindingMode.TwoWay;
            bndbezier.Converter = new EnumToVisibilityConverter();
            bndbezier.ConverterParameter = CurveTypes.Bezier;
            model.CurveBezier.PathCurve.SetBinding(Path.VisibilityProperty, bndbezier);
            model.CurveBezControlLine1.PathCurve.SetBinding(Path.VisibilityProperty, bndbezier);
            model.CurveBezControlLine2.PathCurve.SetBinding(Path.VisibilityProperty, bndbezier);
            model.PointBezCont1.SetBinding(UserControl.VisibilityProperty, bndbezier);
            model.PointBezCont2.SetBinding(UserControl.VisibilityProperty, bndbezier);
            model.PointBezier1.SetBinding(UserControl.VisibilityProperty, bndbezier);
            model.PointBezier2.SetBinding(UserControl.VisibilityProperty, bndbezier);

            nodeView.inputGrid.Children.Add(celeryGraphControl);

            //
            model.canvas = celeryGraphControl.thisCanvas;
            celeryGraphControl.DataContext = model;

            //graphmapNode.OnNodeModified();

            //  initialize events here
            graphmapNode.PointBezCont1.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointBezCont2.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointFreeSine1.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointFreeSine2.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointBezier1.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointBezier2.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointLinear1.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointLinear2.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointParabolic1.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointParabolic2.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointPerlin01.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointPerlin02.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;
            graphmapNode.PointPerlin11.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;

            model.RequestChangeGraphMapValues += UpdateGraphMapValues;

            //HideOrShowOnError(graphmapNode.AreAllInputsAvailable);

            UpdateGraphMapValues();
        }

        private void CanvasPreviewMouseLeftUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cecoPointControl cecp = sender as cecoPointControl;
            cecoPointEnd cece = sender as cecoPointEnd;
            cecoPointFree cecf = sender as cecoPointFree;
            cecoPointOrtho ceco = sender as cecoPointOrtho;
            if (cecp != null)
            {
                if (cecp.bcurve != null)
                    cecp.bcurve.Regenerate(cecp);
            }
            if (cece != null)
            {
                if (cece.bcurve != null)
                    cece.bcurve.Regenerate(cece);
            }
            if (cecf != null)
            {
                if (cecf.lcurve != null)
                    cecf.lcurve.Regenerate(cecf);
                if (cecf.scurve != null)
                    cecf.scurve.Regenerate(cecf);
                if (cecf.parabcurve != null)
                    cecf.parabcurve.Regenerate(cecf);
                if (cecf.perlcurve != null)
                    cecf.perlcurve.Regenerate(cecf);
            }
            if (ceco != null)
            {
                if (ceco.perlcurve != null)
                    ceco.perlcurve.Regenerate(ceco);
            }

            UpdateGraphMapValues();

            graphmapNode.OnNodeModified();
        }

        private void HideOrShowOnError(bool isHide)
        {
            celeryGraphControl.thisCanvasErrorText.Visibility = (isHide) ? Visibility.Hidden : Visibility.Visible;
        }

        private void UpdateGraphMapValues()
        {
            var s = dynamoViewmodel.Model.Scheduler;

            var t = new DelegateBasedAsyncTask(s, () =>
            {
                graphmapNode.ComputeOutput(dynamoModel.EngineController);

                if (graphmapNode.ListOutputValues == null)
                {
                    //  no entries in the list
                    //  that means during the point control operations an error is detected
                    if (!graphmapNode.AreAllInputsAvailable)
                        HideOrShowOnError(true);
                    else
                        HideOrShowOnError(false);
                }
                else
                {
                    HideOrShowOnError(graphmapNode.AreAllInputsAvailable);
                }
            });

            t.ThenSend((_) =>
            {
                //  add the collected grid lines from the ComputeOutput method
                foreach (gridline g in graphmapNode.GridLines)
                {
                    if (!graphmapNode.canvas.Children.Contains(g.PathCurve))
                        graphmapNode.canvas.Children.Add(g.PathCurve);
                }
                graphmapNode.OnNodeModified();
            }, syncContext);

            s.ScheduleForExecution(t);
        }

        public void Dispose()
        {
            graphmapNode.PointBezCont1.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointBezCont2.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointFreeSine1.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointFreeSine2.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointBezier1.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointBezier2.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointLinear1.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointLinear2.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointParabolic1.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointParabolic2.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointPerlin01.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointPerlin02.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
            graphmapNode.PointPerlin11.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
        }
    }

    #region Curve Types Enum
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CurveTypes
    {
        [Description("Linear Curve")]
        Linear = 0,
        [Description("Bezier Curve")]
        Bezier,
        [Description("Sine Wave")]
        Sine,
        [Description("Parabolic Curve")]
        Parabola,
        [Description("Perlin Noise")]
        PerlinNoise,
    }

    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type _enumType;

        public Type EnumType
        {
            get
            {
                return _enumType;
            }

            set
            {
                if (value != this._enumType)
                {
                    if (null != value)
                    {
                        Type enumtype = Nullable.GetUnderlyingType(value) ?? value;
                        if (!enumtype.IsEnum)
                            throw new ArgumentException("Type must be for an Enum");
                    }

                    _enumType = value;
                }
            }
        }

        public EnumBindingSourceExtension()
        {

        }

        public EnumBindingSourceExtension(Type enumType)
        {
            EnumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == _enumType)
                throw new InvalidOperationException("The EnumType must be specified.");

            Type actualenumtype = Nullable.GetUnderlyingType(_enumType) ?? _enumType;
            Array enumvalues = Enum.GetValues(actualenumtype);

            if (actualenumtype == _enumType)
            {
                return enumvalues;
            }

            Array temparray = Array.CreateInstance(actualenumtype, enumvalues.Length + 1);
            enumvalues.CopyTo(temparray, 1);
            return temparray;
        }
    }

    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type) : base(type)
        {

        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fi = value.GetType().GetField(value.ToString());
                    if (fi != null)
                    {
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }

                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CurveTypes ct = (CurveTypes)value;
            CurveTypes pt = (CurveTypes)parameter;

            if (ct.Equals(pt))
                return Visibility.Visible;

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (CurveTypes)parameter;
        }
    }
    #endregion
}
