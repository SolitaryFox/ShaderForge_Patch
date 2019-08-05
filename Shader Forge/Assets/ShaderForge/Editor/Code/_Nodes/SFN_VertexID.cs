using UnityEngine;
using UnityEditor;
using System.Collections;
using ShaderForge;
namespace ShaderForge
{
    public class SFN_VertexID : SF_Node
    {
        public SFN_VertexID()
        {

        }

        public override void Initialize()
        {
            base.Initialize("VertexID.", InitialPreviewRenderMode.Off);
            base.showColor = true;
            base.UseLowerPropertyBox(false);
            base.texture.CompCount = 3;
            base.neverDefineVariable = true;
            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv1,false)
            };
        }

        public override Vector4 EvalCPU()
        {
            return new Color(1f, 0f, 0f, 0f);
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            return SF_Evaluator.WithProgramPrefix("vid");
        }
    }
}