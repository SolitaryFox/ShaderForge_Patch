using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderForge
{

    public class SFN_TexSize : SF_Node
    {

        public bool TexAssetConnected()
        {
            if (property == null)
                if (GetInputIsConnected("TEX"))
                    return true;
            return false;
        }

        public Texture textureAsset;

        public Texture TextureAsset
        {
            get
            {
                if (TexAssetConnected())
                {
                    textureAsset = null;
                    return (GetInputCon("TEX").node as SFN_Tex2dAsset).textureAsset;
                }
                return textureAsset;
            }
            set
            {
                textureAsset = value;
            }
        }

        public override void Initialize()
        {
            base.Initialize("Tex Size", InitialPreviewRenderMode.BlitQuad);
            base.showColor = true;
            base.UseLowerPropertyBox(false);
            base.texture.uniform = false;
            base.texture.CompCount = 4;
            base.alwaysDefineVariable = true;
            base.shaderGenMode = ShaderGenerationMode.OffUniform;

            connectors = new SF_NodeConnector[]{
                    SF_NodeConnector.Create(this,"TEX","Tex",ConType.cInput,ValueType.TexAsset).WithColor(SF_Node.colorExposed),
                SF_NodeConnector.Create(this,"1w","1/w",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.R),
                SF_NodeConnector.Create(this,"1h","1/h",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.G),
                SF_NodeConnector.Create(this,"w","w",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.B),
                SF_NodeConnector.Create(this,"h","h",ConType.cOutput,ValueType.VTv1,false).Outputting(OutChannel.A)
            };

        }

        public override bool UpdatesOverTime()
        {
            return true;
        }

        public override bool IsUniformOutput()
        {
            return true;
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {
            var connect = GetInputCon("TEX");
            return connect.node.property.nameInternal + "_TexelSize";
        }

        public override float EvalCPU(int c)
        {
            return 1f;
        }
    }
}
