Shader "Hidden/Shader Forge/SFN_VertexID" {
    Properties {
        _OutputMask ("Output Mask", Vector) = (1,1,1,1)

    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma target 3.0
            uniform float4 _OutputMask;


            struct VertexInput {
                float4 vertex : POSITION;
               uint vid : SV_VertexID;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 vid : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vid =v.vid;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                // Operator
                float4 outputColor = float4( i.vid, 0 );
                // Return
                return outputColor * _OutputMask;
            }
            ENDCG
        }
    }
}
