using Celery.UI;
using Dynamo.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dynamo.Controls;
using Dynamo.ViewModels;
using System.Windows.Threading;
using Dynamo.Models;
using Celery.Controls;
using System.Windows.Controls;
using Celery.Controls.Subcontrols;
using Celery.Controls.SubcontrolClasses;

namespace Celery.ViewCustomization
{
    public class UVPointerViewCustomization : INodeViewCustomization<UVPointer>
    {
        private DynamoViewModel dynamoViewmodel;
        private DispatcherSynchronizationContext syncContext;
        private DynamoModel dynamoModel;
        private CeleryUVPointer celeryUVPointerControl;
        private UVPointer uvpNode;
        private NodeViewModel nodeviewmodel;

        public void CustomizeView(UVPointer model, NodeView nodeView)
        {
            dynamoModel = nodeView.ViewModel.DynamoViewModel.Model;
            dynamoViewmodel = nodeView.ViewModel.DynamoViewModel;
            nodeviewmodel = nodeView.ViewModel;
            syncContext = new DispatcherSynchronizationContext(nodeView.Dispatcher);
            uvpNode = model;

            model.EngineController = nodeView.ViewModel.DynamoViewModel.EngineController;

            celeryUVPointerControl = new CeleryUVPointer();

            /*if(!model.IsDeserialized) */model.pointMover = new Controls.Subcontrols.cecoPointFree(new System.Windows.Point(uvpNode.U, uvpNode.V));
            model.pointMover.maxwidth = model.CanvasWidth;
            model.pointMover.maxheight = model.CanvasHeight;
            Canvas.SetZIndex(model.pointMover, 75);

            model.CrossHairHorizontal = new Controls.SubcontrolClasses.crosshair(celeryUVPointerControl.thisCanvas, model.pointMover.Point, model.CanvasWidth, model.CanvasHeight);
            model.CrossHairVertical = new Controls.SubcontrolClasses.crosshair(celeryUVPointerControl.thisCanvas, model.pointMover.Point, model.CanvasWidth, model.CanvasHeight, true);
            model.UVCoordinateText = new Controls.SubcontrolClasses.uvcoordtext(model.pointMover.Point, model.CanvasWidth, model.CanvasHeight);

            celeryUVPointerControl.thisCanvas.Children.Add(model.pointMover);
            celeryUVPointerControl.thisCanvas.Children.Add(model.CrossHairHorizontal.PathCurve);
            celeryUVPointerControl.thisCanvas.Children.Add(model.CrossHairVertical.PathCurve);
            celeryUVPointerControl.thisCanvas.Children.Add(model.UVCoordinateText);

            foreach (gridline gl in model.GridLines)
            {
                celeryUVPointerControl.thisCanvas.Children.Add(gl.PathCurve);
                Canvas.SetZIndex(gl.PathCurve, 10);
            }

            model.pointMover.chairhor = model.CrossHairHorizontal;
            model.pointMover.chairver = model.CrossHairVertical;
            model.pointMover.uvtext = model.UVCoordinateText;

            //
            nodeView.inputGrid.Children.Add(celeryUVPointerControl);
            //

            if (model.CanvasWidth == 0.0)
                model.CanvasWidth = 250.0;
            if (model.CanvasHeight == 0.0)
                model.CanvasHeight = 250.0;

            //
            uvpNode.pointMover.PreviewMouseLeftButtonUp += CanvasPreviewMouseLeftUp;

            UpdateUVPointer(model.pointMover);
        }

        private void CanvasPreviewMouseLeftUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            cecoPointFree cecf = sender as cecoPointFree;

            UpdateUVPointer(cecf);
        }

        private void UpdateUVPointer(cecoPointFree cecf)
        {
            if (cecf != null)
            {
                if (cecf.chairhor != null)
                    cecf.chairhor.Regenerate(cecf);
                if (cecf.chairver != null)
                    cecf.chairver.Regenerate(cecf);
                if (cecf.uvtext != null)
                {
                    cecf.uvtext.Regenerate(cecf.Point);
                }

                uvpNode.U = cecf.Point.X;
                uvpNode.V = cecf.Point.Y;
            }

            uvpNode.OnNodeModified();
        }

        public void Dispose()
        {
            uvpNode.pointMover.PreviewMouseLeftButtonUp -= CanvasPreviewMouseLeftUp;
        }
    }
}
