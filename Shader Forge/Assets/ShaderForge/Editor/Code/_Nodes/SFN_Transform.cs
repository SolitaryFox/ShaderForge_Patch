using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace ShaderForge
{

    [System.Serializable]
    public class SFN_Transform : SF_Node
    {


        /*
		public string[] matrixVars = new string[]{
			"UNITY_MATRIX_MVP",
			"UNITY_MATRIX_MV",
			"UNITY_MATRIX_V",
			"UNITY_MATRIX_P",
			"UNITY_MATRIX_VP",
			"UNITY_MATRIX_T_MV",
			"UNITY_MATRIX_IT_MV",
			"unity_Object2World",
			"unity_WorldToObject",
			"tangentTransform"
		};

		public string[] matrixLabels = new string[]{
			"Model*View*Projection",
			"Model*View",
			"View",
			"Projection",
			"View*Projection",
			"Transpose Model*View",
			"Inverse transpose Model*View",
			"Model to World",
			"World to Model",
			"Tangent"
		};
		*/


        public string[] spaceLabels = new string[]{
            "World",
            "Local",
            "Tangent",
            "View"
        };

        public string[] modLabels = new string[]{
            "Direction",
            "Position"
        };

        public enum Space { World, Local, Tangent, View };

        public Space spaceSelFrom = Space.World;
        public Space spaceSelTo = Space.Local;

        public enum TranMod { Direction, Position };

        public TranMod tranMod = TranMod.Direction;

        //public const int tangentID = 9;
        //public int selection = 0;

        public SFN_Transform()
        {

        }

        public override void Initialize()
        {
            base.Initialize("Transform");
            base.showColor = true;
            base.vectorDataNode = true; // This should really be renamed to "Always draw as 3D"
            UseLowerPropertyBox(true, true);
            base.shaderGenMode = ShaderGenerationMode.Manual;
            //UseLowerReadonlyValues(true,true);

            connectors = new SF_NodeConnector[]{
                SF_NodeConnector.Create(this,"XYZ","XYZ",ConType.cOutput,ValueType.VTv3,false).Outputting(OutChannel.RGB),
                SF_NodeConnector.Create(this,"IN","In",ConType.cInput,ValueType.VTv3,false).SetRequired(true)
            };
            base.node_height += 14;
            base.node_height += 20;

            base.conGroup = ScriptableObject.CreateInstance<SFNCG_Arithmetic>().Initialize(connectors[0], connectors[1]);
        }

        public override int GetEvaluatedComponentCount()
        {
            return this["IN"].GetCompCount();
        }

        public override bool IsUniformOutput()
        {
            return GetInputData("IN").uniform;
        }

        public override void PrepareRendering(Material mat)
        {
            mat.SetFloat("_FromSpace", (int)spaceSelFrom);
            mat.SetFloat("_ToSpace", (int)spaceSelTo);
        }


        // New system
        public override void RefreshValue()
        {
            RefreshValue(1, 1);
        }

        public string GetInVector(bool tangent = false, bool dir = true)
        {
            if (tangent)
                return GetConnectorByStringID("IN").TryEvaluate();
            else
            {
                if (dir) return "float4(" + GetConnectorByStringID("IN").TryEvaluate() + ",0)";
                else return "float4(" + GetConnectorByStringID("IN").TryEvaluate() + ",1)";
            }

        }

        private string Direction()
        {
            // From world space
            if (FromTo(Space.World, Space.Local))
            {
                return "mul( unity_WorldToObject, " + GetInVector() + " ).xyz";
            }
            if (FromTo(Space.World, Space.Tangent))
            {
                return "mul( tangentTransform, " + GetInVector(tangent: true) + " ).xyz";
            }
            if (FromTo(Space.World, Space.View))
            {
                return "mul( UNITY_MATRIX_V, " + GetInVector() + " ).xyz";
            }


            // From local space
            if (FromTo(Space.Local, Space.World))
            {
                return "mul( unity_ObjectToWorld, " + GetInVector() + " ).xyz";
            }
            if (FromTo(Space.Local, Space.Tangent))
            {
                return "mul( tangentTransform, " + "mul( unity_ObjectToWorld, " + GetInVector() + " ).xyz" + " ).xyz";
            }
            if (FromTo(Space.Local, Space.View))
            {
                return "UnityObjectToViewPos( " + GetInVector() + " ).xyz";
            }

            // From tangent space
            if (FromTo(Space.Tangent, Space.World))
            {
                return "mul( " + GetInVector(tangent: true) + ", tangentTransform ).xyz";
            }
            if (FromTo(Space.Tangent, Space.Local))
            {
                return "mul( unity_WorldToObject, " + "float4(mul( " + GetInVector(tangent: true) + ", tangentTransform ),0)" + " ).xyz";
            }
            if (FromTo(Space.Tangent, Space.View))
            {
                return "mul( UNITY_MATRIX_V, " + "float4(mul( " + GetInVector(tangent: true) + ", tangentTransform ),0)" + " ).xyz";
            }


            // From view space
            if (FromTo(Space.View, Space.World))
            {
                return "mul( " + GetInVector() + ", UNITY_MATRIX_V ).xyz";
            }
            if (FromTo(Space.View, Space.Local))
            {
                return "mul( " + GetInVector() + ", UNITY_MATRIX_MV ).xyz";
            }
            if (FromTo(Space.View, Space.Tangent))
            {
                return "mul( tangentTransform, " + "mul( " + GetInVector() + ", UNITY_MATRIX_V ).xyz" + " ).xyz";
            }
            return GetConnectorByStringID("IN").TryEvaluate();
        }

        private string Position()
        {
            // From world space
            if (FromTo(Space.World, Space.Local))
            {
                return "mul( unity_WorldToObject, " + GetInVector(tangent:false,dir:false) + " ).xyz";
            }
            if (FromTo(Space.World, Space.Tangent))
            {
                return "mul( tangentTransform, " + GetInVector(tangent: true) + " ).xyz";
            }
            if (FromTo(Space.World, Space.View))
            {
                return "mul( UNITY_MATRIX_V, " + GetInVector(tangent: false, dir: false) + " ).xyz";
            }


            // From local space
            if (FromTo(Space.Local, Space.World))
            {
                return "mul( unity_ObjectToWorld, " + GetInVector(tangent: false, dir: false) + " ).xyz";
            }
            if (FromTo(Space.Local, Space.Tangent))
            {
                return "mul( tangentTransform, " + "mul( unity_ObjectToWorld, " + GetInVector(tangent: false, dir: false) + " ).xyz" + " ).xyz";
            }
            if (FromTo(Space.Local, Space.View))
            {
                return "UnityObjectToViewPos( " + GetInVector(tangent: false, dir: false) + " ).xyz";
            }

            // From tangent space
            if (FromTo(Space.Tangent, Space.World))
            {
                return "mul( " + GetInVector(tangent: true) + ", tangentTransform ).xyz";
            }
            if (FromTo(Space.Tangent, Space.Local))
            {
                return "mul( unity_WorldToObject, " + "float4(mul( " + GetInVector(tangent: true) + ", tangentTransform ),0)" + " ).xyz";
            }
            if (FromTo(Space.Tangent, Space.View))
            {
                return "mul( UNITY_MATRIX_V, " + "float4(mul( " + GetInVector(tangent: true) + ", tangentTransform ),0)" + " ).xyz";
            }


            // From view space
            if (FromTo(Space.View, Space.World))
            {
                return "mul( " + GetInVector(tangent: false, dir: false) + ", UNITY_MATRIX_V ).xyz";
            }
            if (FromTo(Space.View, Space.Local))
            {
                return "mul( " + GetInVector(tangent: false, dir: false) + ", UNITY_MATRIX_MV ).xyz";
            }
            if (FromTo(Space.View, Space.Tangent))
            {
                return "mul( tangentTransform, " + "mul( " + GetInVector(tangent: false, dir: false) + ", UNITY_MATRIX_V ).xyz" + " ).xyz";
            }
            return GetConnectorByStringID("IN").TryEvaluate();
        }

        public override string Evaluate(OutChannel channel = OutChannel.All)
        {

            if (spaceSelFrom == spaceSelTo)
            { // TODO: Add warning about tunneling variable
                return GetConnectorByStringID("IN").TryEvaluate();
            }


            if (tranMod == TranMod.Direction)
                return Direction();
            else return Position();


            // TODO TODO TODO:




            /*
			if( selection != tangentID )
				return "mul( " + matrixVars[selection] + ", float4( " + GetConnectorByStringID( "IN" ).TryEvaluate() + ", 0 )).xyz";
			else
				return "mul( " + matrixVars[selection] + ", " + GetConnectorByStringID( "IN" ).TryEvaluate() + " )";
			*/
        }

        public bool FromTo(Space from, Space to)
        {
            return (spaceSelFrom == from && spaceSelTo == to);
        }

        // Pass through
        public override Vector4 EvalCPU()
        {
            return GetInputData("IN").node.EvalCPU();
        }

        const float dirLabelWidth = 28;
        public override void DrawLowerPropertyBox()
        {
            EditorGUI.BeginChangeCheck();



            Rect r = new Rect(lowerRect);
            r.width = dirLabelWidth;
            r.height = 18;

            //r.height /= 2;
            GUI.Label(r, "From", SF_Styles.MiniLabelOverflow);
            r.x += r.width;
            r.width = (lowerRect.width - dirLabelWidth);

            spaceSelFrom = (Space)UndoablePopup(r, (int)spaceSelFrom, spaceLabels, "switch transform 'from' setting");
            r.y += r.height;
            spaceSelTo = (Space)UndoablePopup(r, (int)spaceSelTo, spaceLabels, "switch transform 'to' setting");
            r.x = 0;
            r.width = dirLabelWidth;
            GUI.Label(r, "To", SF_Styles.MiniLabelOverflow);


            r.x = 0;
            r.width = dirLabelWidth;
            r.y += r.height;

            GUI.Label(r, "Mod", SF_Styles.MiniLabelOverflow);
            r.x += r.width;
            r.width = (lowerRect.width - dirLabelWidth);
            tranMod = (TranMod)UndoablePopup(r, (int)tranMod, modLabels, "switch transform 'mod' position");

            /*
			r.width -= toLabelWidth;
			float popupWidth = (r.width /= 2);
			spaceSelFrom = EditorGUI.Popup(r, spaceSelFrom, spaceLabels);
			r.x += r.width;
			r.width = toLabelWidth;
			GUI.Label(r,"to",SF_Styles.MiniLabelOverflow);
			r.x += r.width;
			r.width = popupWidth;
			spaceSelTo = EditorGUI.Popup(r, spaceSelTo, spaceLabels);
			*/

            //selection = EditorGUI.Popup( lowerRect, selection, matrixLabels );
            if (EditorGUI.EndChangeCheck())
            {
                OnUpdateNode();
            }
        }


        public override string SerializeSpecialData()
        {
            string s = "tffrom:" + (int)spaceSelFrom + ",";
            s += "tfto:" + (int)spaceSelTo;
            return s;
        }

        public override void DeserializeSpecialData(string key, string value)
        {
            switch (key)
            {
                case "tffrom":
                    spaceSelFrom = (Space)int.Parse(value);
                    break;
                case "tfto":
                    spaceSelTo = (Space)int.Parse(value);
                    break;
            }
        }


        /*
		public override Vector4 NodeOperator( int x, int y ) {
			return base.NodeOperator( x, y );
		}
		*/

    }
}