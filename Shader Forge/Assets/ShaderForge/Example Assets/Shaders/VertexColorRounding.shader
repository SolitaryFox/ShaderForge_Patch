// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:0,x:34043,y:32815,varname:node_0,prsc:2|emission-585-OUT;n:type:ShaderForge.SFN_VertexColor,id:4,x:33627,y:32845,varname:node_4,prsc:2;n:type:ShaderForge.SFN_Slider,id:38,x:33095,y:32967,ptovrint:False,ptlb:Divisions,ptin:_Divisions,varname:_Divisions,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5639098,max:1;n:type:ShaderForge.SFN_Power,id:42,x:33443,y:32990,varname:node_42,prsc:2|VAL-38-OUT,EXP-44-OUT;n:type:ShaderForge.SFN_Vector1,id:44,x:33252,y:33034,varname:node_44,prsc:2,v1:2;n:type:ShaderForge.SFN_ConstantLerp,id:137,x:33627,y:32990,varname:node_137,prsc:2,a:0.5,b:12|IN-42-OUT;n:type:ShaderForge.SFN_Posterize,id:585,x:33845,y:32915,varname:node_585,prsc:2|IN-4-RGB,STPS-137-OUT;proporder:38;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Vertex Color Rounding" {
    Properties {
        _Divisions ("Divisions", Range(0, 1)) = 0.5639098
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _Divisions;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(0)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_137 = lerp(0.5,12,pow(_Divisions,2.0));
                float3 emissive = floor(i.vertexColor.rgb * node_137) / (node_137 - 1);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
