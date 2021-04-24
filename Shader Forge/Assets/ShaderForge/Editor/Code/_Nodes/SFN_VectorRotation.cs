using ShaderForge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFN_VectorRotation : SF_Node_Arithmetic
{
    public override void Initialize()
    {
        base.Initialize("Vector Rotation");
        base.PrepareArithmetic(2, ValueType.VTv3);
        base.shaderGenMode = ShaderGenerationMode.SimpleFunction;
        connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"OUT","",ConType.cOutput,ValueType.VTv3,false),
                SF_NodeConnector.Create(this,"A","Normal Dir",ConType.cInput,ValueType.VTv3,false).SetRequired(true),
                SF_NodeConnector.Create(this,"B","Angle",ConType.cInput,ValueType.VTv3,false).SetRequired(true),
            };
    }

    public override string GetPrepareUniformsAndFunctions()
    {
        var code = @"            float4x4 vector3_rotation(float4 rotation)
			{
				float radX = radians(rotation.x);
				float radY = radians(rotation.y);
				float radZ = radians(rotation.z);
				float sinX = sin(radX);
				float cosX = cos(radX);
				float sinY = sin(radY);
				float cosY = cos(radY);
				float sinZ = sin(radZ);
				float cosZ = cos(radZ);
				return float4x4(cosY * cosZ, sinX * sinY * cosZ - cosX * sinZ, cosX * sinY * cosZ + sinX * sinY, 0,
                                cosY * sinZ, sinX * sinY * sinZ + cosX * cosZ, cosX * sinY * sinZ - sinX * cosZ, 0,
                                -sinY, sinX * cosY, cosX * cosY, 0,
                                0, 0, 0, 1);
              
			}";
        return code;
    }
    public string GetFunctionName()
    {
        return "vector3_rotation";
        //return "CustomCode_" + id;
    }
    public override string Evaluate(OutChannel channel = OutChannel.All)
    {
        var normal_dir = "float4(" + GetConnectorByStringID("A").TryEvaluate() + ",1)";
        var inputB = "float4(" + GetConnectorByStringID("B").TryEvaluate() + ",0)";
        string s = GetFunctionName();
        s += "(" + inputB + ")";
        return $"mul({s}, {normal_dir})";
    }

    public override float EvalCPU(int c)
    {
        return 1;
    }
}
