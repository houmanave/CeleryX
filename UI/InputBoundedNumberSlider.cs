using Celery.Controls;
using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.UI.Commands;
using Dynamo.ViewModels;
using Dynamo.Wpf;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;
using Dynamo.Graph;
using System.Xml;
using System.Diagnostics;
using Newtonsoft.Json;
using Autodesk.DesignScript.Runtime;
using System.Collections;

namespace Celery.UI
{
    /// <summary>
    /// A number slider with numerical inputs.
    /// </summary>
    [IsDesignScriptCompatible]
    [NodeName("InputBoundedNumberSlider")]
    [NodeCategory("Celery.NumericControls")]
    [NodeDescription("Slider to produce output number based on given input-bounded numbers.")]
    //[InPortNames("leftLimit", "rightLimit")]
    //[InPortDescriptions("Left limit of the domain.", "Right limit of the domain.")]
    [InPortTypes("double", "double")]
    //[OutPortNames("value")]
    //[OutPortDescriptions("Value extracted from the slider.")]
    [OutPortTypes("double")]
    class InputBoundedNumberSlider : NodeModel
    {
        internal EngineController EngineController { get; set; }

        private int _sliderPrecision = -1;
        public int SliderPrecision
        {
            get
            {
                return _sliderPrecision;
            }
            set
            {
                _sliderPrecision = value;
                //InitialPrecision = _sliderPrecision;    //  2018.5.13 cpreviously commented

                RecomputeOutputValues();

                RaisePropertyChanged("SliderPrecision");

                OnNodeModified();
            }
        }

        public bool IsDeserialized = false;

        public int InitialPrecision { get; private set; }
        
        public List<int> PrecisionCollection
        {
            get;
            private set;
        }

        [JsonIgnore]
        public double SliderLargeChange
        {
            get
            {
                return Math.Pow(10.0, SliderPrecision / 2);
            }
        }

        [JsonIgnore]
        public double SliderMaximum
        {
            get
            {
                return Math.Pow(10.0, SliderPrecision);
            }
        }

        private double _sliderValue = 1.0;
        public double SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;

                //  2018.5.19
                RecomputeOutputValues();
                //

                RaisePropertyChanged("SliderValue");

                OnNodeModified();
            }
        }

        private double _minval;

        [JsonIgnore]
        public double Minval
        {
            get { return _minval; }
            set
            {
                _minval = value;

                RaisePropertyChanged("Minval");
            }
        }

        private double _maxval;

        [JsonIgnore]
        public double Maxval
        {
            get { return _maxval; }
            set
            {
                _maxval = value;

                RaisePropertyChanged("Maxval");
            }
        }

        private void RecomputeOutputValues()
        {
            //  assuming minimum is 1 and maximum is 500
            double remaps = (SliderValue - 1) * (Maxval - Minval) / (500 - 1) + Minval;
            OutputValue = remaps;

            //OutputValue = _minval + (_maxval - _minval) * (_sliderValue / Math.Pow(10.0, InitialPrecision)/*SliderMaximum*/);
            //OutputviewValue = _minval + (_maxval - _minval) * (_sliderValue / Math.Pow(10.0, InitialPrecision)/*SliderMaximum*/);
        }

        private double _outputValue;

        [JsonIgnore]
        public double OutputValue
        {
            get
            {
                return _outputValue;
            }
            set
            {
                _outputValue = value;
                if (IsInteger)
                {
                    _outputValue = Math.Round(_outputValue);
                }
                else
                {
                    _outputValue = Math.Round(_outputValue, SliderPrecision);
                }

                //  2018.5.19
                OutputviewValue = _outputValue;

                RaisePropertyChanged("OutputValue");

                //OnNodeModified();
            }
        }

        private double _outputviewValue;

        [JsonIgnore]
        public double OutputviewValue
        {
            get { return _outputviewValue; }
            set
            {
                _outputviewValue = value;

                RaisePropertyChanged("OutputviewValue");
            }
        }

        private bool _isInteger;
        public bool IsInteger
        {
            get { return _isInteger; }
            set
            {
                _isInteger = value;
                if (_isInteger)
                {
                    OutputValue = Math.Round(OutputValue);
                }
                else
                {
                    OutputValue = Math.Round(OutputValue, SliderPrecision);
                }

                RaisePropertyChanged("IsInteger");

                OnNodeModified();
            }
        }

        //private bool _isNumber;
        [JsonIgnore]
        public bool IsNumber
        {
            get
            {
                return !_isInteger;
            }
            set
            {
                IsInteger = !(bool)value;
                RaisePropertyChanged("IsNumber");

                OnNodeModified();
            }
        }

        [JsonIgnore]
        public bool IsUntriggerComputeOutput { get; private set; }

        public event Action RequestChangeVariableNumberSlider;

        protected virtual void OnRequestChangeVariableNumberSlider()
        {
            if (RequestChangeVariableNumberSlider != null)
                RequestChangeVariableNumberSlider();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InputBoundedNumberSlider()
        {
            Console.WriteLine("Passed Constructor");

            InPorts.Add(new PortModel(PortType.Input, this, new PortData("leftLimit", "Left limit of the domain.")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("rightLimit", "Right limit of the domain.")));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("value", "Value extracted from the slider.")));

            RegisterAllPorts();

            foreach(var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }

            this.PropertyChanged += InputBoundedNumberSlider_PropertyChanged;

            if (PrecisionCollection == null)
            {
                PrecisionCollection = new List<int>();
            }

            if (!PrecisionCollection.Contains(1)) PrecisionCollection.Add(1);
            if (!PrecisionCollection.Contains(2)) PrecisionCollection.Add(2);
            if (!PrecisionCollection.Contains(3)) PrecisionCollection.Add(3);
            if (!PrecisionCollection.Contains(4)) PrecisionCollection.Add(4);
            if (!PrecisionCollection.Contains(5)) PrecisionCollection.Add(5);
            if (!PrecisionCollection.Contains(6)) PrecisionCollection.Add(6);
            if (!PrecisionCollection.Contains(7)) PrecisionCollection.Add(7);
            if (!PrecisionCollection.Contains(8)) PrecisionCollection.Add(8);
            if (!PrecisionCollection.Contains(9)) PrecisionCollection.Add(9);
            if (!PrecisionCollection.Contains(10)) PrecisionCollection.Add(10);

            if (SliderPrecision < 0)
                SliderPrecision = 3;
            InitialPrecision = SliderPrecision;

            ArgumentLacing = LacingStrategy.Disabled;
            //ShouldDisplayPreviewCore = false;
        }

        [JsonConstructor]
        InputBoundedNumberSlider(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            //Console.WriteLine("Passed JsonConstructor");
            //  at this stage, no values are listed in the combobox.
            //  and as the precision was not assigned properly,
            //  the rounding off goes odd; multiplying the right limit by 10 to the
            //  slider precision value's power.

            foreach (var port in InPorts)
            {
                port.Connectors.CollectionChanged += Connectors_CollectionChanged;
            }
            //  unfixed

            this.PropertyChanged += InputBoundedNumberSlider_PropertyChanged;
            //  unfixed

            if (PrecisionCollection == null)
            {
                PrecisionCollection = new List<int>();
            }
            //  precision is placed in the combobox, but the rounding off is not fixed yet.

            if (SliderPrecision < 0)
                SliderPrecision = 3;
            InitialPrecision = SliderPrecision;
            //  rounding off is fixed here.

            ArgumentLacing = LacingStrategy.Disabled;

            //this.PortDisconnected += InputBoundedNumberSlider_PortDisconnected;
        }

        private void InputBoundedNumberSlider_PortDisconnected(PortModel obj)
        {
            RaisePropertyChanged("SliderPrecision");
        }

        #region 2.0
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
        }
        #endregion

        private void InputBoundedNumberSlider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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

            //  the function of this is to update the UI whenever there is a
            //  change of tne value of either limits.
            //  this connects to the UpdateVariaNumberSlider from the NodeViewCustomization
            OnRequestChangeVariableNumberSlider();
        }

        void Connectors_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //  the function of this is to update the UI whenever there is a
            //  change of tne value of either limits.
            //  this connects to the UpdateVariaNumberSlider from the NodeViewCustomization
            OnRequestChangeVariableNumberSlider();
        }

        public void ComputeOutput(EngineController engine)
        {
            Console.WriteLine("Passed ComputeOutput");

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

                double parsed;
                Minval = TryConvertToDouble(mino, out parsed) ? parsed : 0.0;
                Maxval = TryConvertToDouble(maxo, out parsed) ? parsed : 0.0;

                RecomputeOutputValues();

                //
                IsUntriggerComputeOutput = true;
            }
        }

        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<ProtoCore.AST.AssociativeAST.AssociativeNode> BuildOutputAst(List<ProtoCore.AST.AssociativeAST.AssociativeNode> inputAstNodes)
        {
            if (!InPorts[0].Connectors.Any() || !InPorts[1].Connectors.Any())
            {
                return new[] {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0),AstFactory.BuildNullNode())
                };
            }

            var doublenode = AstFactory.BuildDoubleNode(OutputValue);

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), doublenode)
            };
        }

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
        
    }

    /// <summary>
    /// Node view customizer for InputBoundedNumberSlider
    /// </summary>
    class InputBoundedNumberSliderViewCustomization : INodeViewCustomization<InputBoundedNumberSlider>
    {
        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;
        private CelerySliderControl celerySliderControl;
        private InputBoundedNumberSlider varianumsliNode;

        //private IViewModelView<NodeViewModel> ui;

        public void CustomizeView(InputBoundedNumberSlider model, Dynamo.Controls.NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);
            varianumsliNode = model;

            //ui = nodeView;

            model.EngineController = nodeView.ViewModel.DynamoViewModel.EngineController;

            celerySliderControl = new CelerySliderControl(model, nodeView);
            nodeView.inputGrid.Children.Add(celerySliderControl);

            celerySliderControl.DataContext = model;
            
            model.RequestChangeVariableNumberSlider += UpdateVariaNumberSlider;

            UpdateVariaNumberSlider();

            celerySliderControl.drawSlider.PreviewMouseUp += drawSlider_PreviewMouseUp;
            celerySliderControl.drawSlider.ValueChanged += drawSlider_ValueChanged;
            celerySliderControl.drawSlider.PreviewMouseDown += drawSlider_PreviewMouseDown;
        }

        void drawSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            varianumsliNode.ComputeOutput(dynamoModel.EngineController);
        }

        void drawSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
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
            }, syncContext);

            s.ScheduleForExecution(t);
        }

        public void Dispose()
        {
            celerySliderControl.drawSlider.PreviewMouseUp -= drawSlider_PreviewMouseUp;
            celerySliderControl.drawSlider.ValueChanged -= drawSlider_ValueChanged;
            celerySliderControl.drawSlider.PreviewMouseDown -= drawSlider_PreviewMouseDown;
        }
    }
}
