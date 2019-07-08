using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Track_Deform_CS))]
	public class Track_Deform_CSEditor : Editor
	{

		SerializedProperty Anchor_NumProp;
		SerializedProperty Anchor_ArrayProp;
		SerializedProperty Width_ArrayProp;
		SerializedProperty Height_ArrayProp;
		SerializedProperty Is_PreparedProp;

		GameObject thisGameObject;

		void OnEnable ()
		{
			Anchor_NumProp = serializedObject.FindProperty ("Anchor_Num");
			Anchor_ArrayProp = serializedObject.FindProperty ("Anchor_Array");
			Width_ArrayProp = serializedObject.FindProperty ("Width_Array");
			Height_ArrayProp = serializedObject.FindProperty ("Height_Array");
			Is_PreparedProp = serializedObject.FindProperty ("Is_Prepared");

			thisGameObject = Selection.activeGameObject;
		}

		public override void OnInspectorGUI ()
		{
			Set_Inspector ();
			if (GUI.changed) {
				Set_Vertices ();
			}
			if (Event.current.commandName == "UndoRedoPerformed") {
				Set_Vertices ();
			}
		}

		void Set_Inspector ()
		{
			if (EditorApplication.isPlaying == false) {
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				serializedObject.Update ();
				EditorGUILayout.Space ();

				EditorGUILayout.IntSlider (Anchor_NumProp, 1, 64, "Number of Anchor Wheels");
				EditorGUILayout.Space ();

				Anchor_ArrayProp.arraySize = Anchor_NumProp.intValue;
				Width_ArrayProp.arraySize = Anchor_NumProp.intValue;
				Height_ArrayProp.arraySize = Anchor_NumProp.intValue;
				for (int i = 0; i < Anchor_ArrayProp.arraySize; i++) {
					Anchor_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Anchor Wheel", Anchor_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Transform), true);
					EditorGUILayout.Slider (Width_ArrayProp.GetArrayElementAtIndex (i), 0.0f, 10.0f, "Width");
					EditorGUILayout.Slider (Height_ArrayProp.GetArrayElementAtIndex (i), 0.0f, 10.0f, "Height");
					EditorGUILayout.Space ();
				}
				if (Anchor_ArrayProp.arraySize != 0) {
					Is_PreparedProp.boolValue = true;
				} else {
					Is_PreparedProp.boolValue = false;
				}
				// Update Value
				if (GUILayout.Button ("Update Value")) {
					Set_Vertices ();
				}
				EditorGUILayout.Space ();
				serializedObject.ApplyModifiedProperties ();
			}
		}

		void Set_Vertices ()
		{
			PrefabUtility.DisconnectPrefabInstance (thisGameObject); // Break prefab connection.
			Mesh thisMesh = thisGameObject.GetComponent < MeshFilter > ().sharedMesh;
			float[] initialPosArray = new float [ Anchor_ArrayProp.arraySize ];
			IntArray[] movableVerticesList = new IntArray [ Anchor_ArrayProp.arraySize ];
			// Get vertices in the range.
			for (int i = 0; i < Anchor_ArrayProp.arraySize; i++) {
				if (Anchor_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue != null) {
					Transform anchorTransform = Anchor_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue as Transform;
					initialPosArray [i] = anchorTransform.localPosition.x;
					Vector3 anchorPos = thisGameObject.transform.InverseTransformPoint (anchorTransform.position);
					List < int > withinVerticesList = new List < int > ();
					for (int j = 0; j < thisMesh.vertices.Length; j++) {
						float distZ = Mathf.Abs (anchorPos.z - thisMesh.vertices [j].z);
						float distY = Mathf.Abs (anchorPos.y - thisMesh.vertices [j].y);
						if (distZ <= Width_ArrayProp.GetArrayElementAtIndex (i).floatValue * 0.5f && distY <= Height_ArrayProp.GetArrayElementAtIndex (i).floatValue * 0.5f) {
							withinVerticesList.Add (j);
						}
					}
					IntArray withinVerticesArray = new IntArray (withinVerticesList.ToArray ());
					movableVerticesList [i] = withinVerticesArray;
				}
			}
			// Set values.
			Track_Deform_CS deformScript = thisGameObject.GetComponent < Track_Deform_CS > ();
			deformScript.Initial_Pos_Array = initialPosArray;
			deformScript.Initial_Vertices = thisMesh.vertices;
			deformScript.Movable_Vertices_List = movableVerticesList;
		}

	}

}