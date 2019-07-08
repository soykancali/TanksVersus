using UnityEngine;
using System;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_TrackBelt_CS))]
	public class Create_TrackBelt_CSEditor : Editor
	{

		SerializedProperty Rear_FlagProp;
		SerializedProperty SelectedAngleProp;
		SerializedProperty Angle_RearProp;
		SerializedProperty NumberProp;
		SerializedProperty SpacingProp;
		SerializedProperty DistanceProp;
		SerializedProperty Track_MassProp;
		SerializedProperty Collider_InfoProp;
		SerializedProperty Collider_MaterialProp;
		SerializedProperty Track_R_MeshProp;
		SerializedProperty Track_L_MeshProp;
		SerializedProperty Track_R_MaterialProp;
		SerializedProperty Track_L_MaterialProp;

		SerializedProperty SubJoint_TypeProp;
		SerializedProperty Reinforce_RadiusProp;

		SerializedProperty Special_OffsetProp;

		SerializedProperty Track_DurabilityProp;
		SerializedProperty BreakForceProp;

		SerializedProperty RealTime_FlagProp;
		SerializedProperty Static_FlagProp;
		SerializedProperty Prefab_FlagProp;
	
		string[] angleNames = { "10", "11.25", "12", "15", "18", "20", "22.5", "25.71", "30", "36", "45", "60", "90" };
		int[] angleValues = { 1000, 1125, 1200, 1500, 1800, 2000, 2250, 2571, 3000, 3600, 4500, 6000, 9000 };
		string[] subJointNames = { "All", "Every Two pieces", "None" };

		Transform parentTransform;

		void OnEnable ()
		{
			Rear_FlagProp = serializedObject.FindProperty ("Rear_Flag");
			SelectedAngleProp = serializedObject.FindProperty ("SelectedAngle");
			Angle_RearProp = serializedObject.FindProperty ("Angle_Rear");
			NumberProp = serializedObject.FindProperty ("Number_Straight");
			SpacingProp = serializedObject.FindProperty ("Spacing");
			DistanceProp = serializedObject.FindProperty ("Distance");
			Track_MassProp = serializedObject.FindProperty ("Track_Mass");
			Collider_InfoProp = serializedObject.FindProperty ("Collider_Info");
			Collider_MaterialProp = serializedObject.FindProperty ("Collider_Material");
			Track_R_MeshProp = serializedObject.FindProperty ("Track_R_Mesh");
			Track_L_MeshProp = serializedObject.FindProperty ("Track_L_Mesh");
			Track_R_MaterialProp = serializedObject.FindProperty ("Track_R_Material");
			Track_L_MaterialProp = serializedObject.FindProperty ("Track_L_Material");

			SubJoint_TypeProp = serializedObject.FindProperty ("SubJoint_Type");
			Reinforce_RadiusProp = serializedObject.FindProperty ("Reinforce_Radius");

			Special_OffsetProp = serializedObject.FindProperty ("Special_Offset");

			Track_DurabilityProp = serializedObject.FindProperty ("Track_Durability");
			BreakForceProp = serializedObject.FindProperty ("BreakForce");

			RealTime_FlagProp = serializedObject.FindProperty ("RealTime_Flag");
			Static_FlagProp = serializedObject.FindProperty ("Static_Flag");
			Prefab_FlagProp = serializedObject.FindProperty ("Prefab_Flag");
		
			if (Selection.activeGameObject) {
				parentTransform = Selection.activeGameObject.transform;
			}
		}

		public override void OnInspectorGUI ()
		{
			bool isPrepared;
			if (parentTransform.parent == null || parentTransform.parent.gameObject.GetComponent<Rigidbody> () == null) {
				isPrepared = false;
			} else {
				isPrepared = true;
			}
		
			if (isPrepared) {
				Set_Inspector ();
				if (GUI.changed && RealTime_FlagProp.boolValue) {
					Create ();
				}
				if (Event.current.commandName == "UndoRedoPerformed") {
					Create ();
				}
			}
		}

		void Set_Inspector ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
			if (EditorApplication.isPlaying == false) {

				// Basic settings
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Basic settings", MessageType.None, true);
				EditorGUILayout.Slider (DistanceProp, 0.1f, 10.0f, "Distance");
				EditorGUILayout.Slider (SpacingProp, 0.05f, 1.0f, "Spacing");
				EditorGUILayout.Slider (Track_MassProp, 0.1f, 100.0f, "Mass");
				// Shape settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Shape settings", MessageType.None, true);
				SelectedAngleProp.intValue = EditorGUILayout.IntPopup ("Angle of Front Arc", SelectedAngleProp.intValue, angleNames, angleValues);
				Rear_FlagProp.boolValue = EditorGUILayout.Toggle ("Set Rear Arc", Rear_FlagProp.boolValue);
				if (Rear_FlagProp.boolValue) {
					Angle_RearProp.intValue = EditorGUILayout.IntPopup ("Angle of Rear Arc", Angle_RearProp.intValue, angleNames, angleValues);
				}
				EditorGUILayout.IntSlider (NumberProp, 0, 80, "Number of Straight");
				// Collider settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Collider settings", MessageType.None, true);
				Collider_InfoProp.boundsValue = EditorGUILayout.BoundsField ("Box Collider", Collider_InfoProp.boundsValue);
				Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
				// Mesh settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Mesh settings", MessageType.None, true);
				Track_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", Track_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
				Track_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", Track_R_MeshProp.objectReferenceValue, typeof(Mesh), false);
				Track_L_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Left", Track_L_MaterialProp.objectReferenceValue, typeof(Material), false);
				Track_R_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Right", Track_R_MaterialProp.objectReferenceValue, typeof(Material), false);
				// Reinforce settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Reinforce settings", MessageType.None, true);
				SubJoint_TypeProp.intValue = EditorGUILayout.Popup ("Reinforce Type", SubJoint_TypeProp.intValue, subJointNames);
				if (SubJoint_TypeProp.intValue != 2) {
					EditorGUILayout.Slider (Reinforce_RadiusProp, 0.1f, 10.0f, "Radius of SphereCollider");
				}
				// Special settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Specail settings for Unity5", MessageType.None, true);
				EditorGUILayout.Slider (Special_OffsetProp, -0.1f, 0.1f, "Offset for Unity5");
				// Durability settings
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Durability settings", MessageType.None, true);
				EditorGUILayout.Slider (Track_DurabilityProp, 1.0f, 1000000.0f, "Track Durability");
				if (Track_DurabilityProp.floatValue >= 1000000) {
					Track_DurabilityProp.floatValue = Mathf.Infinity;
				}
				EditorGUILayout.Slider (BreakForceProp, 10000.0f, 1000000.0f, "HingeJoint BreakForce");
				if (BreakForceProp.floatValue >= 1000000) {
					BreakForceProp.floatValue = Mathf.Infinity;
				}
				// for Static Track
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Edit Static Track", MessageType.None, true);
				Static_FlagProp.boolValue = EditorGUILayout.Toggle ("for Static Track", Static_FlagProp.boolValue);

				// Update Value
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				RealTime_FlagProp.boolValue = EditorGUILayout.Toggle ("Real Time Update", RealTime_FlagProp.boolValue);
				if (GUILayout.Button ("Update Value")) {
					if (RealTime_FlagProp.boolValue == false) {
						Create ();
					}
				}
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

			} else { // in PlayMode.
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Edit Static Track", MessageType.None, true);
				if (Static_FlagProp.boolValue) { // for making Static_Track
					if (!Prefab_FlagProp.boolValue) { // Static_Track is not prepared yet.
						RealTime_FlagProp.boolValue = false;
						EditorGUILayout.Space ();
						EditorGUILayout.Space ();
						//if (GUILayout.Button ("Set all the 'Sus Spring Forces' to Infinity.")) {
						//	Fix_Suspension ();
						//}
						//if (GUILayout.Button ("Increase all the 'Sus Spring Target Angle'")) {
						//	Increase_TargetAngle ();
						//}
						//if (GUILayout.Button ("Decrease all the 'Sus Spring Target Angle'")) {
						//	Decrease_TargetAngle ();
						//}
						if (GUILayout.Button ("Change into Static Track")) {
							Change_Static_Track ();
							Prefab_FlagProp.boolValue = true;
						}
						EditorGUILayout.Space ();
						EditorGUILayout.Space ();
					} else { // Static_Track has been prepared.
						EditorGUILayout.Space ();
						EditorGUILayout.Space ();
						if (GUILayout.Button ("Create Prefab in 'Assets' folder")) {
							Create_Prefab ();
						}
						EditorGUILayout.Space ();
						EditorGUILayout.Space ();
					}
				}
			}

			//
			serializedObject.ApplyModifiedProperties ();
		}

	
		void Create ()
		{			
			// Delete Objects
			int childCount = parentTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (parentTransform.GetChild (0).gameObject);
			}
			// Create Track Pieces	(Preparation)
			if (Rear_FlagProp.boolValue == false) {
				Angle_RearProp.intValue = SelectedAngleProp.intValue;
			}
			float frontAng = SelectedAngleProp.intValue / 100.0f;
			float rearAng = Angle_RearProp.intValue / 100.0f;
			float frontRad = SpacingProp.floatValue / (2.0f * Mathf.Tan (Mathf.PI / (360.0f / frontAng)));
			float rearRad = SpacingProp.floatValue / (2.0f * Mathf.Tan (Mathf.PI / (360.0f / rearAng)));
			float height = frontRad - rearRad;
			float bottom;
			float slopeAngle;
			if (Mathf.Abs (height) > SpacingProp.floatValue * NumberProp.intValue || NumberProp.intValue == 0) {
				bottom = 0.0f;
				slopeAngle = 0.0f;
			} else {
				slopeAngle = Mathf.Asin (height / (SpacingProp.floatValue * NumberProp.intValue));
				if (slopeAngle != 0.0f) {
					bottom = height / Mathf.Tan (slopeAngle);
				} else {
					bottom = SpacingProp.floatValue * NumberProp.intValue;
				}
				slopeAngle *= Mathf.Rad2Deg;
			}
			// Create Front Arc
			int num = 0;
			Vector3 centerPos;
			centerPos.x = frontRad;
			centerPos.y = DistanceProp.floatValue / 2.0f;
			centerPos.z = 0.0f;
			Vector3 pos;
			pos.y = DistanceProp.floatValue / 2.0f;
			for (int i = 0; i <= 180 / frontAng; i++) {
				num++;
				pos.x = frontRad * Mathf.Sin (Mathf.Deg2Rad * (270.0f + (frontAng * i)));
				pos.x += centerPos.x;
				pos.z = frontRad * Mathf.Cos (Mathf.Deg2Rad * (270.0f + (frontAng * i)));
				Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), i * frontAng, num);
				Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), i * frontAng, num);
			}
			// Create Upper Straight
			if (bottom != 0.0f) {
				centerPos.x = (frontRad * 2.0f) - (height / NumberProp.intValue / 2.0f);
				centerPos.z = -((SpacingProp.floatValue / 2.0f) + (bottom / NumberProp.intValue / 2.0f));
				for (int i = 0; i < NumberProp.intValue; i++) {
					num++;
					pos.x = centerPos.x - (height / NumberProp.intValue * i);
					pos.z = centerPos.z - (bottom / NumberProp.intValue * i);
					Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), 180.0f + slopeAngle, num);
					Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), 180.0f + slopeAngle, num);
				}
			}
			// Create Rear Arc
			centerPos.x = frontRad;
			centerPos.z = -(bottom + SpacingProp.floatValue);
			for (int i = 0; i <= 180 / rearAng; i++) {
				num++;
				pos.x = rearRad * Mathf.Sin (Mathf.Deg2Rad * (90.0f + (rearAng * i)));
				pos.x += centerPos.x;
				pos.z = rearRad * Mathf.Cos (Mathf.Deg2Rad * (90.0f + (rearAng * i)));
				pos.z += centerPos.z;
				Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), 180.0f + (i * rearAng), num);
				Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), 180.0f + (i * rearAng), num);
			}
			// Create lower Straight
			if (bottom != 0.0f) {
				centerPos.x = (frontRad - rearRad) - (height / NumberProp.intValue / 2.0f);
				centerPos.z = -(bottom + (SpacingProp.floatValue / 2.0f)) + (bottom / NumberProp.intValue / 2.0f);
				for (int i = 0; i < NumberProp.intValue; i++) {
					num++;
					pos.x = centerPos.x - (height / NumberProp.intValue * i);
					pos.z = centerPos.z + (bottom / NumberProp.intValue * i);
					Create_TrackPiece ("L", new Vector3 (pos.x, pos.y, pos.z), -slopeAngle, num);
					Create_TrackPiece ("R", new Vector3 (pos.x, -pos.y, pos.z), -slopeAngle, num);
				}
			}
			// Create Reinforce Collider.
			if (SubJoint_TypeProp.intValue != 2) {
				for (int i = 0; i < num; i++) {
					if (SubJoint_TypeProp.intValue == 0 || (i + 1) % 2 == 0) {
						Create_Reinforce ("L", i + 1);
						Create_Reinforce ("R", i + 1);
					}
				}
			}
			// Add RigidBody and Joint.
			Finishing ("L");
			Finishing ("R");
		}

		void Create_TrackPiece (string direction, Vector3 position, float angleY, int number)
		{
			//Create gameobject & Set transform
			GameObject gameObject = new GameObject ("TrackBelt_" + direction + "_" + number);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = Quaternion.Euler (0.0f, angleY, -90.0f);
			// Mesh
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			if (direction == "L") {
				meshFilter.mesh = Track_L_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = Track_L_MaterialProp.objectReferenceValue as Material;
			} else {
				meshFilter.mesh = Track_R_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = Track_R_MaterialProp.objectReferenceValue as Material;
			}
			// BoxCollider
			BoxCollider boxCollider = gameObject.AddComponent < BoxCollider > ();
			if (direction == "L") {
				boxCollider.center = Collider_InfoProp.boundsValue.center;
			} else {
				boxCollider.center = Collider_InfoProp.boundsValue.center;
				boxCollider.center = new Vector3 (-boxCollider.center.x, boxCollider.center.y, boxCollider.center.z);
			}
			boxCollider.size = Collider_InfoProp.boundsValue.size;
			boxCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			// Stabilizer_CS
			gameObject.AddComponent < Stabilizer_CS > ();
			// Damage_Control_CS
			Damage_Control_CS damageScript;
			damageScript = gameObject.AddComponent < Damage_Control_CS > ();
			damageScript.Type = 6; // 6 = Track
			damageScript.Durability = Track_DurabilityProp.floatValue;
			if (direction == "L") {
				damageScript.Direction = 0;
			} else {
				damageScript.Direction = 1;
			}
			// Static_Track_Setting_CS
			if (Static_FlagProp.boolValue) {
				gameObject.AddComponent < Static_Track_Setting_CS > ();
			}
			// Set Layer
			gameObject.layer = 0;
		}

		void Create_Reinforce (string direction, int number)
		{
			//Create gameobject & Set transform
			Transform basePiece = parentTransform.Find ("TrackBelt_" + direction + "_" + number);
			GameObject gameObject = new GameObject ("Reinforce_" + direction + "_" + number);
			gameObject.transform.position = basePiece.position;
			gameObject.transform.rotation = basePiece.rotation;
			gameObject.transform.parent = basePiece;
			// SphereCollider
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Reinforce_RadiusProp.floatValue;
			// Set Layer
			gameObject.layer = 10; // Ignore All.
		}

		void Finishing (string direction)
		{
			// Add RigidBody.
			for (int i = 1; i <= parentTransform.childCount; i++) {
				Transform basePiece = parentTransform.Find ("TrackBelt_" + direction + "_" + i);
				if (basePiece) {
					// Add RigidBody.
					Rigidbody rigidbody = basePiece.gameObject.AddComponent < Rigidbody > ();
					rigidbody.mass = Track_MassProp.floatValue;
					// Special Offset for Unity5.
					if (i % 2 == 0) {
						basePiece.position += basePiece.forward * Special_OffsetProp.floatValue;
					}
					// for Static_Track creating
					if (Static_FlagProp.boolValue) {
						rigidbody.drag = 10.0f;
					}
				}
			}
			// Add HingeJoint.
			for (int i = 1; i <= parentTransform.childCount; i++) {
				Transform basePiece = parentTransform.Find ("TrackBelt_" + direction + "_" + i);
				if (basePiece) {
					HingeJoint hingeJoint = basePiece.gameObject.AddComponent < HingeJoint > ();
					hingeJoint.anchor = new Vector3 (0.0f, 0.0f, SpacingProp.floatValue / 2.0f);
					hingeJoint.axis = new Vector3 (1.0f, 0.0f, 0.0f);
					hingeJoint.breakForce = BreakForceProp.floatValue;
					Transform frontPiece = parentTransform.Find ("TrackBelt_" + direction + "_" + (i + 1));
					if (frontPiece) {
						hingeJoint.connectedBody = frontPiece.GetComponent < Rigidbody > ();
					} else {
						frontPiece = parentTransform.Find ("TrackBelt_" + direction + "_1");
						if (frontPiece) {
							hingeJoint.connectedBody = frontPiece.GetComponent < Rigidbody > ();
						}
					}
				}
			}
		}
			
		void Fix_Suspension () {
			HingeJoint [] hingeJoints = parentTransform.root.GetComponentsInChildren < HingeJoint > ();
			foreach ( HingeJoint hingeJoint in hingeJoints ) {
				hingeJoint.useLimits = false;
				JointSpring jointSpring = hingeJoint.spring;
				jointSpring.spring = Mathf.Infinity;
				hingeJoint.spring = jointSpring;
			}
		}

		void Increase_TargetAngle () {
			HingeJoint [] hingeJoints = parentTransform.root.GetComponentsInChildren < HingeJoint > ();
			foreach (HingeJoint hingeJoint in hingeJoints) {
				if (hingeJoint.useSpring) {
					hingeJoint.useLimits = false;
					JointSpring jointSpring = hingeJoint.spring;
					float targetPos = jointSpring.targetPosition;
					jointSpring.targetPosition += Mathf.Sign (targetPos);
					hingeJoint.spring = jointSpring;
				}
			}
		}

		void Decrease_TargetAngle () {
			HingeJoint [] hingeJoints = parentTransform.root.GetComponentsInChildren < HingeJoint > ();
			foreach (HingeJoint hingeJoint in hingeJoints) {
				if (hingeJoint.useSpring) {
					hingeJoint.useLimits = false;
					JointSpring jointSpring = hingeJoint.spring;
					float targetPos = jointSpring.targetPosition;
					if (jointSpring.targetPosition < 0) {
						if (jointSpring.targetPosition >= -1.0f) {
							return;
						}
					} else {
						if (jointSpring.targetPosition <= 1.0f) {
							return;
						}
					}
					jointSpring.targetPosition -= Mathf.Sign (targetPos);
					hingeJoint.spring = jointSpring;
				}
			}
		}

		void Change_Static_Track ()
		{
			parentTransform.BroadcastMessage ("Set_Static_Track_Value", SendMessageOptions.DontRequireReceiver);
			Time.timeScale = 0.0f;
		}

		void Create_Prefab ()
		{
			GameObject newObject = new GameObject ("NewObject");
			newObject.transform.parent = parentTransform.parent;
			newObject.transform.localPosition = parentTransform.localPosition;
			newObject.transform.localRotation = parentTransform.localRotation;
			// Add Static_Track_CS.
			Static_Track_CS trackScript = newObject.AddComponent < Static_Track_CS > ();
			trackScript.Type = 9; // Parent ;
			trackScript.Length = Collider_InfoProp.boundsValue.size.z;
			// Enable "Static_Track_CS".
			int childCount = parentTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				Transform childTransform = parentTransform.GetChild (0);
				childTransform.GetComponent < Static_Track_CS > ().enabled = true;
				childTransform.parent = newObject.transform;
			}
			// Create Prefab.
			PrefabUtility.CreatePrefab ("Assets/" + "Static_Track" + DateTime.Now.ToString ("yyMMdd_HHmmss") + ".prefab", newObject);
			// Message.
			if (parentTransform.childCount == 0) {
				Debug.Log ("New 'Static_Track' has been created in 'Assets' folder.");
			} else {
				Debug.LogWarning ("The script fails to create a prefab of 'Static_Track'.");
			}
			DestroyImmediate (newObject);
		}
	
	}

}