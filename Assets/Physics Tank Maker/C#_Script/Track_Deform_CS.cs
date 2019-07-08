using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ System.Serializable]
	public class IntArray
	{
		public int[] intArray;
		public IntArray (int[] newIntArray)
		{
			intArray = newIntArray;
		}
	}

	public class Track_Deform_CS : MonoBehaviour
	{
	
		public int Anchor_Num;
		public Transform[] Anchor_Array;
		public float[] Width_Array;
		public float[] Height_Array;

		public float[] Initial_Pos_Array;
		public Vector3[] Initial_Vertices;
		public IntArray[] Movable_Vertices_List;

		public bool Is_Prepared = false;

		Mesh thisMesh;
		MainBody_Setting_CS mainScript;

		void Start ()
		{
			thisMesh = GetComponent < MeshFilter > ().mesh;
			// Check 'Anchor_Array'
			for (int i = 0; i < Anchor_Array.Length; i++) {
				if (Anchor_Array [i] == null) {
					Debug.LogError ("Anchor Wheel is not assigned in " + this.name);
					Destroy (this);
				}
			}
		}

		void Update ()
		{
			if (mainScript.Visible_Flag) { // MainBody is visible by any camera.
				Vector3[] tempVertices = new Vector3 [ Initial_Vertices.Length ];
				Initial_Vertices.CopyTo (tempVertices, 0);
				for (int i = 0; i < Anchor_Array.Length; i++) {
					float tempDist = Anchor_Array [i].localPosition.x - Initial_Pos_Array [i];
					for (int j = 0; j < Movable_Vertices_List [i].intArray.Length; j++) {
						tempVertices [Movable_Vertices_List [i].intArray [j]].y += tempDist;
					}
				}
				thisMesh.vertices = tempVertices;
			}
		}

		void OnDrawGizmosSelected ()
		{
			if (Is_Prepared) {
				Gizmos.color = Color.green;
				for (int i = 0; i < Anchor_Array.Length; i++) {
					if (Anchor_Array [i] != null) {
						Vector3 tempSize = new Vector3 (0.0f, Height_Array [i], Width_Array [i]);
						Vector3 tempCenter = Anchor_Array [i].position;
						Gizmos.DrawWireCube (tempCenter, tempSize);
					}
				}
			}
		}

		void Get_MainScript (MainBody_Setting_CS tempScript)
		{ // Called from "MainBody_Setting_CS".
			mainScript = tempScript;
		}

	}

}