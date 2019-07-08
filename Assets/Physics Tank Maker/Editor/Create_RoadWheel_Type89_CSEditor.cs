﻿using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_RoadWheel_Type89_CS))]
	public class Create_RoadWheel_Type89_CSEditor : Editor
	{

		SerializedProperty Fit_ST_FlagProp;

		SerializedProperty DistanceProp;
		SerializedProperty SpringProp;
		SerializedProperty ParentArm_NumProp;
		SerializedProperty ParentArm_SpacingProp;
		SerializedProperty ParentArm_Offset_YProp;
		SerializedProperty ParentArm_AngleLimitProp;
		SerializedProperty ParentArm_MassProp;
		SerializedProperty ParentArm_L_MeshProp;
		SerializedProperty ParentArm_R_MeshProp;
		SerializedProperty ParentArm_L_MaterialProp;
		SerializedProperty ParentArm_R_MaterialProp;
	
		SerializedProperty ChildArm_NumProp;
		SerializedProperty ChildArm_SpacingProp;
		SerializedProperty ChildArm_Offset_YProp;
		SerializedProperty ChildArm_AngleLimitProp;
		SerializedProperty ChildArm_MassProp;
		SerializedProperty ChildArm_L_MeshProp;
		SerializedProperty ChildArm_R_MeshProp;
		SerializedProperty ChildArm_L_MaterialProp;
		SerializedProperty ChildArm_R_MaterialProp;
	
		SerializedProperty Wheel_NumProp;
		SerializedProperty Wheel_SpacingProp;
		SerializedProperty Wheel_Offset_YProp;
		SerializedProperty Wheel_MassProp;
		SerializedProperty Wheel_RadiusProp;
		SerializedProperty Wheel_Collider_MaterialProp;
		SerializedProperty Wheel_MeshProp;
		SerializedProperty Wheel_MaterialProp;
		SerializedProperty Wheel_Collider_MeshProp;

		SerializedProperty Drive_WheelProp;
		SerializedProperty Wheel_ResizeProp;
		SerializedProperty ScaleDown_SizeProp;
		SerializedProperty Return_SpeedProp;
		SerializedProperty Wheel_DurabilityProp;
		SerializedProperty RealTime_FlagProp;
	
		Transform parentTransform;

		void OnEnable ()
		{
			Fit_ST_FlagProp = serializedObject.FindProperty ("Fit_ST_Flag");

			DistanceProp = serializedObject.FindProperty ("Distance");
			SpringProp = serializedObject.FindProperty ("Spring");
			ParentArm_NumProp = serializedObject.FindProperty ("ParentArm_Num");
			ParentArm_SpacingProp = serializedObject.FindProperty ("ParentArm_Spacing");
			ParentArm_Offset_YProp = serializedObject.FindProperty ("ParentArm_Offset_Y");
			ParentArm_AngleLimitProp = serializedObject.FindProperty ("ParentArm_AngleLimit");
			ParentArm_MassProp = serializedObject.FindProperty ("ParentArm_Mass");
			ParentArm_L_MeshProp = serializedObject.FindProperty ("ParentArm_L_Mesh");
			ParentArm_R_MeshProp = serializedObject.FindProperty ("ParentArm_R_Mesh");
			ParentArm_L_MaterialProp = serializedObject.FindProperty ("ParentArm_L_Material");
			ParentArm_R_MaterialProp = serializedObject.FindProperty ("ParentArm_R_Material");
		
			ChildArm_NumProp = serializedObject.FindProperty ("ChildArm_Num");
			ChildArm_SpacingProp = serializedObject.FindProperty ("ChildArm_Spacing");
			ChildArm_Offset_YProp = serializedObject.FindProperty ("ChildArm_Offset_Y");
			ChildArm_AngleLimitProp = serializedObject.FindProperty ("ChildArm_AngleLimit");
			ChildArm_MassProp = serializedObject.FindProperty ("ChildArm_Mass");
			ChildArm_L_MeshProp = serializedObject.FindProperty ("ChildArm_L_Mesh");
			ChildArm_R_MeshProp = serializedObject.FindProperty ("ChildArm_R_Mesh");
			ChildArm_L_MaterialProp = serializedObject.FindProperty ("ChildArm_L_Material");
			ChildArm_R_MaterialProp = serializedObject.FindProperty ("ChildArm_R_Material");
		
			Wheel_NumProp = serializedObject.FindProperty ("Wheel_Num");
			Wheel_SpacingProp = serializedObject.FindProperty ("Wheel_Spacing");
			Wheel_Offset_YProp = serializedObject.FindProperty ("Wheel_Offset_Y");
			Wheel_MassProp = serializedObject.FindProperty ("Wheel_Mass");
			Wheel_RadiusProp = serializedObject.FindProperty ("Wheel_Radius");
			Wheel_Collider_MaterialProp = serializedObject.FindProperty ("Wheel_Collider_Material");
			Wheel_MeshProp = serializedObject.FindProperty ("Wheel_Mesh");
			Wheel_MaterialProp = serializedObject.FindProperty ("Wheel_Material");
			Wheel_Collider_MeshProp = serializedObject.FindProperty ("Wheel_Collider_Mesh");

			Drive_WheelProp = serializedObject.FindProperty ("Drive_Wheel");
			Wheel_ResizeProp = serializedObject.FindProperty ("Wheel_Resize");
			ScaleDown_SizeProp = serializedObject.FindProperty ("ScaleDown_Size");
			Return_SpeedProp = serializedObject.FindProperty ("Return_Speed");
			Wheel_DurabilityProp = serializedObject.FindProperty ("Wheel_Durability");
			RealTime_FlagProp = serializedObject.FindProperty ("RealTime_Flag");

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

			// for Static Wheel
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheel Type", MessageType.None, true);
			Fit_ST_FlagProp.boolValue = EditorGUILayout.Toggle ("Fit for Static Tracks", Fit_ST_FlagProp.boolValue);

			// 'Parent Arm' settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("'Parent Arm' settings", MessageType.None, true);
			EditorGUILayout.Slider (DistanceProp, 0.1f, 10.0f, "Distance");
			EditorGUILayout.Slider (SpringProp, 0.0f, 1000000.0f, "Spring Force");
			EditorGUILayout.IntSlider (ParentArm_NumProp, 0, 3, "Number");
			EditorGUILayout.Slider (ParentArm_SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Slider (ParentArm_Offset_YProp, -1.0f, 1.0f, "Anchor Offset");
			EditorGUILayout.Slider (ParentArm_AngleLimitProp, 0.0f, 90.0f, "Limit Angle");
			EditorGUILayout.Slider (ParentArm_MassProp, 0.1f, 300.0f, "Mass");
			ParentArm_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", ParentArm_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ParentArm_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", ParentArm_R_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ParentArm_L_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Left", ParentArm_L_MaterialProp.objectReferenceValue, typeof(Material), false);
			ParentArm_R_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Right", ParentArm_R_MaterialProp.objectReferenceValue, typeof(Material), false);

			// 'Child Arm' settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("'Child Arm' settings", MessageType.None, true);
			EditorGUILayout.IntSlider (ChildArm_NumProp, 0, 3, "Number");
			EditorGUILayout.Slider (ChildArm_SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Slider (ChildArm_Offset_YProp, -1.0f, 1.0f, "Anchor Offset");
			EditorGUILayout.Slider (ChildArm_AngleLimitProp, 0.0f, 90.0f, "Limit Angle");
			EditorGUILayout.Slider (ChildArm_MassProp, 0.1f, 300.0f, "Mass");
			ChildArm_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", ChildArm_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ChildArm_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", ChildArm_R_MeshProp.objectReferenceValue, typeof(Mesh), false);
			ChildArm_L_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Left", ChildArm_L_MaterialProp.objectReferenceValue, typeof(Material), false);
			ChildArm_R_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Right", ChildArm_R_MaterialProp.objectReferenceValue, typeof(Material), false);

			// Wheels settings
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheels settings", MessageType.None, true);
			EditorGUILayout.IntSlider (Wheel_NumProp, 0, 3, "Number");
			EditorGUILayout.Slider (Wheel_SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Slider (Wheel_Offset_YProp, -1.0f, 1.0f, "Anchor Offset");
			EditorGUILayout.Slider (Wheel_MassProp, 0.1f, 300.0f, "Mass");
			GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			EditorGUILayout.Slider (Wheel_RadiusProp, 0.01f, 1.0f, "SphereCollider Radius");
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			Wheel_Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Wheel_Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
			Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Wheel_MeshProp.objectReferenceValue, typeof(Mesh), false);
			Wheel_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material", Wheel_MaterialProp.objectReferenceValue, typeof(Material), false);
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				Wheel_Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh Collider", Wheel_Collider_MeshProp.objectReferenceValue, typeof(Mesh), false);
			}
			// Scripts settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Scripts settings", MessageType.None, true);
			// Drive Wheel
			Drive_WheelProp.boolValue = EditorGUILayout.Toggle ("Drive Wheel", Drive_WheelProp.boolValue);
			EditorGUILayout.Space ();
			// Wheel Resize
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				Wheel_ResizeProp.boolValue = EditorGUILayout.Toggle ("Wheel Resize Script", Wheel_ResizeProp.boolValue);
				if (Wheel_ResizeProp.boolValue) {
					EditorGUILayout.Slider (ScaleDown_SizeProp, 0.1f, 3.0f, "Scale Size");
					EditorGUILayout.Slider (Return_SpeedProp, 0.01f, 0.1f, "Return Speed");
				}
				EditorGUILayout.Space ();
			}
			// Durability
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				EditorGUILayout.Slider (Wheel_DurabilityProp, 1, 1000000, "Wheel Durability");
				if (Wheel_DurabilityProp.floatValue >= 1000000) {
					Wheel_DurabilityProp.floatValue = Mathf.Infinity;
				}
			}

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
			// Create Parent Arm
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				Vector3 pos;
				pos.x = 0.0f;	
				pos.y = DistanceProp.floatValue / 2.0f;
				pos.z = -ParentArm_SpacingProp.floatValue * i;
				int num = i + 1;
				Set_ParentArm ("L", pos, num);
			}
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				Vector3 pos;
				pos.x = 0.0f;	
				pos.y = -DistanceProp.floatValue / 2.0f;
				pos.z = -ParentArm_SpacingProp.floatValue * i;
				int num = i + 1;
				Set_ParentArm ("R", pos, num);
			}
			// Create Child Arm
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					Vector3 pos;
					pos.x = ChildArm_Offset_YProp.floatValue;
					pos.y = DistanceProp.floatValue / 2.0f;
					pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
					pos.z += (-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j);
					int num = (j + 1) + (ChildArm_NumProp.intValue * i);
					int parentNum = i + 1;
					Set_ChildArm ("L", pos, num, parentNum);
				}
			}
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					Vector3 pos;
					pos.x = ChildArm_Offset_YProp.floatValue;
					pos.y = -DistanceProp.floatValue / 2.0f;
					pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
					pos.z += (-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j);
					int num = (j + 1) + (ChildArm_NumProp.intValue * i);
					int parentNum = i + 1;
					Set_ChildArm ("R", pos, num, parentNum);
				}
			}
			// Create Wheel
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					for (int k = 0; k < Wheel_NumProp.intValue; k++) {
						Vector3 pos;
						pos.x = ChildArm_Offset_YProp.floatValue + Wheel_Offset_YProp.floatValue;
						pos.y = DistanceProp.floatValue / 2.0f;
						pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
						pos.z += (Wheel_NumProp.intValue - 1) * Wheel_SpacingProp.floatValue / 2.0f;
						pos.z += ((-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j)) + (-Wheel_SpacingProp.floatValue * k);
						int num = (k + 1) + (Wheel_NumProp.intValue * j) + (Wheel_NumProp.intValue * ChildArm_NumProp.intValue * i);
						int parentNum = (j + 1) + (ChildArm_NumProp.intValue * i);
						Set_Wheel ("L", pos, num, parentNum);
					}
				}
			}
			for (int i = 0; i < ParentArm_NumProp.intValue; i++) {
				for (int j = 0; j < ChildArm_NumProp.intValue; j++) {
					for (int k = 0; k < Wheel_NumProp.intValue; k++) {
						Vector3 pos;
						pos.x = ChildArm_Offset_YProp.floatValue + Wheel_Offset_YProp.floatValue;
						pos.y = -DistanceProp.floatValue / 2.0f;
						pos.z = (ChildArm_NumProp.intValue - 1) * ChildArm_SpacingProp.floatValue / 2.0f;
						pos.z += (Wheel_NumProp.intValue - 1) * Wheel_SpacingProp.floatValue / 2.0f;
						pos.z += ((-ParentArm_SpacingProp.floatValue * i) + (-ChildArm_SpacingProp.floatValue * j)) + (-Wheel_SpacingProp.floatValue * k);
						int num = (k + 1) + (Wheel_NumProp.intValue * j) + (Wheel_NumProp.intValue * ChildArm_NumProp.intValue * i);
						int parentNum = (j + 1) + (ChildArm_NumProp.intValue * i);
						Set_Wheel ("R", pos, num, parentNum);
					}
				}
			}
		}

		void Set_ParentArm (string direction, Vector3 position, int number)
		{
			// Create ParentArm GameObject
			GameObject gameObject = new GameObject ("ParentArm_" + direction + "_" + number);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
			// Mesh
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			if (direction == "L") {
				meshFilter.mesh = ParentArm_L_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ParentArm_L_MaterialProp.objectReferenceValue as Material;
			} else {
				meshFilter.mesh = ParentArm_R_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ParentArm_R_MaterialProp.objectReferenceValue as Material;
			}
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = ParentArm_MassProp.floatValue;
			// Reinforce SphereCollider
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = 0.45f;
			// ConfigurableJoint
			ConfigurableJoint configJoint = gameObject.AddComponent < ConfigurableJoint > ();
			configJoint.connectedBody = parentTransform.parent.gameObject.GetComponent < Rigidbody > ();
			configJoint.anchor = new Vector3 (0.0f, ParentArm_Offset_YProp.floatValue, 0.0f);
			configJoint.axis = Vector3.zero;
			configJoint.secondaryAxis = Vector3.zero;
			configJoint.xMotion = ConfigurableJointMotion.Locked;
			configJoint.yMotion = ConfigurableJointMotion.Limited;
			configJoint.zMotion = ConfigurableJointMotion.Locked;
			configJoint.angularXMotion = ConfigurableJointMotion.Limited;
			configJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configJoint.angularZMotion = ConfigurableJointMotion.Locked;
			SoftJointLimit softJointLimit = configJoint.linearLimit; // Set Linear Limit
			softJointLimit.limit = 0.1f;
			configJoint.linearLimit = softJointLimit;
			softJointLimit = configJoint.lowAngularXLimit; // Set Low Angular XLimit
			softJointLimit.limit = -ParentArm_AngleLimitProp.floatValue;
			configJoint.lowAngularXLimit = softJointLimit;
			softJointLimit = configJoint.highAngularXLimit; // Set High Angular XLimit
			softJointLimit.limit = ParentArm_AngleLimitProp.floatValue;
			configJoint.highAngularXLimit = softJointLimit;
			JointDrive jointDrive = configJoint.yDrive; // Set Vertical Spring.
			//jointDrive.mode = JointDriveMode.Position ;
			jointDrive.positionSpring = SpringProp.floatValue;
			configJoint.yDrive = jointDrive;
			// Set Layer
			gameObject.layer = 10; // Ignore All
		}

		void Set_ChildArm (string direction, Vector3 position, int number, int parentNumber)
		{
			// Create ChildArm GameObject
			GameObject gameObject = new GameObject ("ChildArm_" + direction + "_" + number);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, -90.0f);
			// Add components
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			if (direction == "L") {
				meshFilter.mesh = ChildArm_L_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ChildArm_L_MaterialProp.objectReferenceValue as Material;
			} else {
				meshFilter.mesh = ChildArm_R_MeshProp.objectReferenceValue as Mesh;
				meshRenderer.material = ChildArm_R_MaterialProp.objectReferenceValue as Material;
			}
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = ChildArm_MassProp.floatValue;
			// Reinforce SphereCollider
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = 0.3f;
			// HingeJoint
			HingeJoint hingeJoint;
			hingeJoint = gameObject.AddComponent < HingeJoint > ();
			hingeJoint.connectedBody = parentTransform.Find ("ParentArm_" + direction + "_" + parentNumber).gameObject.GetComponent < Rigidbody > ();
			hingeJoint.anchor = new Vector3 (0.0f, ChildArm_Offset_YProp.floatValue, 0.0f);
			hingeJoint.axis = new Vector3 (1.0f, 0.0f, 0.0f);
			hingeJoint.useLimits = true;
			JointLimits jointLimits = hingeJoint.limits;
			jointLimits.max = ChildArm_AngleLimitProp.floatValue;
			jointLimits.min = -ChildArm_AngleLimitProp.floatValue;
			hingeJoint.limits = jointLimits;
			// Set Layer
			gameObject.layer = 10; // Ignore All
		}

		void Set_Wheel (string direction, Vector3 position, int number, int parentNumber)
		{
			// Create RoadWheel GameObject
			GameObject gameObject = new GameObject ("RoadWheel_" + direction + "_" + number);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			if (direction == "L") {
				gameObject.transform.localRotation = Quaternion.identity;
			} else {
				gameObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 180.0f);
			}
			// Mesh
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			meshRenderer.material = Wheel_MaterialProp.objectReferenceValue as Material;
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Wheel_MassProp.floatValue;
			// SphereCollider
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Wheel_RadiusProp.floatValue;
			sphereCollider.material = Wheel_Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			// MeshCollider
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				if (Wheel_Collider_MeshProp.objectReferenceValue) {
					MeshCollider meshCollider = gameObject.AddComponent < MeshCollider > ();
					meshCollider.sharedMesh = Wheel_Collider_MeshProp.objectReferenceValue as Mesh;
					meshCollider.material = Wheel_Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
					meshCollider.convex = true;
					meshCollider.enabled = false;
				}
			}
			// HingeJoint
			HingeJoint hingeJoint;
			hingeJoint = gameObject.AddComponent < HingeJoint > ();
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.axis = new Vector3 (0.0f, 1.0f, 0.0f);
			hingeJoint.connectedBody = parentTransform.Find ("ChildArm_" + direction + "_" + parentNumber).gameObject.GetComponent < Rigidbody > ();
			// Drive_Wheel_CS
			Drive_Wheel_CS driveScript = gameObject.AddComponent < Drive_Wheel_CS > ();
			driveScript.Radius = Wheel_RadiusProp.floatValue;
			driveScript.Drive_Flag = Drive_WheelProp.boolValue;
			// Wheel_Resize_CS
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				if (Wheel_ResizeProp.boolValue) {
					Wheel_Resize_CS resizeScript;
					resizeScript = gameObject.AddComponent < Wheel_Resize_CS > ();
					resizeScript.ScaleDown_Size = ScaleDown_SizeProp.floatValue;
					resizeScript.Return_Speed = Return_SpeedProp.floatValue;
				}
			}
			// Damage_Control_CS
			if (Fit_ST_FlagProp.boolValue == false) { // for Physics Tracks
				Damage_Control_CS damageScript;
				damageScript = gameObject.AddComponent < Damage_Control_CS > ();
				damageScript.Type = 8; // 8=Wheel
				damageScript.Durability = Wheel_DurabilityProp.floatValue;
				if (direction == "L") {
					damageScript.Direction = 0;
				} else {
					damageScript.Direction = 1;
				}
			}
			// Stabilizer_CS
			gameObject.AddComponent < Stabilizer_CS > ();
			// Set Layer
			gameObject.layer = 9;
		}
	
	}

}