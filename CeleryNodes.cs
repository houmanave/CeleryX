using Dynamo.Engine;
using Dynamo.Graph.Nodes;
//using MathNet.Numerics;
using ProtoCore.AST.AssociativeAST;
using System;
using System.Collections.Generic;

namespace Celery
{
    /// <summary>
    /// 
    /// </summary>
    public class CeleryNodes
    {
    }

    /*[NodeName("Mathematics.Tangent")]
    [NodeDescription("Evaluates the tangent of the number argument.")]
    [NodeCategory("Celery.DeepMath")]
    [InPortNames("arg")]
    [InPortDescriptions("argument value")]
    [InPortTypes("double")]
    [OutPortNames("result")]
    [OutPortDescriptions("Result from the tangent")]
    [OutPortTypes("double")]
    [IsDesignScriptCompatible]
    public class Tangent : NodeModel
    {
        public Tangent()
        {
            RegisterAllPorts();

            //ArgumentLacing = LacingStrategy.Disabled;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (!HasConnectedInput(0))
            {
                return new[]
                {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode())
                };
            }

            //Func<double, double> tangentmethod = Trig.Tan;

            var functionnode = AstFactory.BuildFunctionCall(
                new Func<double, double>(Trig.Tan),
                new List<AssociativeNode>
                {
                    inputAstNodes[0]
                });

            return new[]
                {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functionnode)
                };
        }
    }

    [NodeName("Mathematics.FirstDerivative")]
    [NodeDescription("Evaluates the first derivation of the function or equation.")]
    [NodeCategory("Celery.DeepMath")]
    [InPortNames("f(x)", "arg")]
    [InPortDescriptions("Function or equation", "argument value")]
    [InPortTypes("function", "double")]
    [OutPortNames("result")]
    [OutPortDescriptions("Result from the first derivative")]
    [OutPortTypes("double")]
    [IsDesignScriptCompatible]
    public class FirstDerivative : NodeModel
    {
        internal EngineController EngineController { get; set; }

        public FirstDerivative()
        {
            RegisterAllPorts();

            ArgumentLacing = LacingStrategy.Disabled;
            //MathNet.Numerics.Differentiate.FirstDerivative(f, x);
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            if (!HasConnectedInput(0) || !HasConnectedInput(1))
            {
                return new[]
                {
                    AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildNullNode())
                };
            }

            var functioncall = AstFactory.BuildFunctionCall(
                new Func<Func<double, double>, double, double>(MathNet.Numerics.Differentiate.FirstDerivative),
                new List<AssociativeNode>
                {
                    inputAstNodes[0],
                    inputAstNodes[1]
                });

            return new[]
            {
                AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), functioncall)
            };
        }
    }*/
}
