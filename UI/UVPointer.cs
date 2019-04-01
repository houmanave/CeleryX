using Dynamo.Graph.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoCore.AST.AssociativeAST;
using Dynamo.Graph;
using System.Xml;
using Celery.Controls.Subcontrols;
using Dynamo.Engine;
using Celery.Controls.SubcontrolClasses;
using System.Windows.Media;
using Newtonsoft.Json;
using Autodesk.DesignScript.Runtime;
using System.Collections;

namespace Celery.UI
{
    [IsDesignScriptCompatible]
    [NodeName("UVPointer")]
    [NodeCategory("Celery.NumericControls")]
    [NodeDescription("A control for specifying UV coordinates by a movable 2D point.")]
    [OutPortNames("u", "v")]
    [OutPortDescriptions("U parameter.", "V pareameter.")]
    [OutPortTypes("double","double")]
    public class UVPointer : NodeModel
    {
        private double _u;
        private double _v;

        [JsonProperty("U")]
        public double U
        {
            get
            {
                return _u;
            }

            set
            {
                _u = value;
                RaisePropertyChanged("U");

                //OnNodeModified();
            }
        }

        [JsonProperty("V")]
        public double V
        {
            get
            {
                return _v;
            }

            set
            {
                _v = value;
                RaisePropertyChanged("V");

                //OnNodeModified();
            }
        }

        [JsonIgnore]
        public double CanvasWidth = 250;
        [JsonIgnore]
        public double CanvasHeight = 250;

        [JsonIgnore]
        internal EngineController EngineController { get; set; }

        [JsonIgnore]
        public cecoPointFree pointMover { get; set; }

        [JsonIgnore]
        public crosshair CrossHairHorizontal { get; set; }
        [JsonIgnore]
        public crosshair CrossHairVertical { get; set; }
        [JsonIgnore]
        public uvcoordtext UVCoordinateText { get; set; }
        [JsonIgnore]
        public List<gridline> GridLines { get; set; }

        [JsonIgnore]
        public bool IsDeserialized = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public UVPointer()
        {
            RegisterAllPorts();

            /*if (U < 0) */U = 125;
            /*if (V < 0) */V = 125;

            pointMover = new cecoPointFree(new System.Windows.Point(U, V));
            pointMover.maxwidth = CanvasWidth;
            pointMover.maxheight = CanvasHeight;

            GridLines = new List<gridline>();
            for (double i = 0.1; i < 1.0; i += 0.1)
            {
                gridline glx = new gridline(i, CanvasWidth, CanvasHeight);
                glx.PathCurve.Stroke = Brushes.DarkGray;
                gridline gly = new gridline(i, CanvasWidth, CanvasHeight, false);
                gly.PathCurve.Stroke = Brushes.DarkGray;

                GridLines.Add(glx);
                GridLines.Add(gly);
            }

            ArgumentLacing = LacingStrategy.Disabled;
        }

        [JsonConstructor]
        UVPointer(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            pointMover = new cecoPointFree(new System.Windows.Point(U, V));
            pointMover.maxwidth = CanvasWidth;
            pointMover.maxheight = CanvasHeight;

            GridLines = new List<gridline>();
            for (double i = 0.1; i < 1.0; i += 0.1)
            {
                gridline glx = new gridline(i, CanvasWidth, CanvasHeight);
                glx.PathCurve.Stroke = Brushes.DarkGray;
                gridline gly = new gridline(i, CanvasWidth, CanvasHeight, false);
                gly.PathCurve.Stroke = Brushes.DarkGray;

                GridLines.Add(glx);
                GridLines.Add(gly);
            }

            ArgumentLacing = LacingStrategy.Disabled;
        }

        #region 2.0
        //  use the VMDataBridge to safely retrieve the input values
        //  (although it is not applicable here.)

        //protected override void OnBuilt()
        //{
        //    base.OnBuilt();
        //    VMDataBridge.DataBridge.Instance.RegisterCallback(GUID.ToString(), DataBridgeCallback);
        //}

        //public override void Dispose()
        //{
        //    base.Dispose();
        //    VMDataBridge.DataBridge.Instance.UnregisterCallback(GUID.ToString());
        //}

        //private void DataBridgeCallback(object data)
        //{
        //    ArrayList inputs = data as ArrayList;
        //}
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        [IsVisibleInDynamoLibrary(false)]
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            double uu = U / CanvasWidth;
            double vv = (CanvasHeight - V) / CanvasHeight;

            uu = Math.Round(uu, 3);
            vv = Math.Round(vv, 3);

            AssociativeNode unode = AstFactory.BuildDoubleNode(uu);
            AssociativeNode vnode = AstFactory.BuildDoubleNode(vv);

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), unode),
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(1), vnode)
            };
        }
    }
}
