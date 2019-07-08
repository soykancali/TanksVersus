using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_SwingBall_CS))]
	public class Create_SwingBall_CSEditor : Editor
	{
	
		SerializedProperty DistanceProp;
		SerializedProperty NumProp;
		SerializedProperty SpacingProp;
		SerializedProperty MassProp;
		SerializedProperty GravityProp;
		SerializedProperty RadiusProp;
		SerializedProperty RangeProp;
		SerializedProperty SpringProp;
		SerializedProperty DamperProp;
		SerializedProperty LayerProp;
		SerializedProperty Collider_MaterialProp;

		string[] layerNames = { "Reinforce (10)", "Default (0)" };
	
		Transform parentTransform;

		void OnEnable ()
		{
			DistanceProp = serializedObject.FindProperty ("Distance");
			NumProp = serializedObject.FindProperty ("Num");
			SpacingProp = serializedObject.FindProperty ("Spacing");
			MassProp = serializedObject.FindProperty ("Mass");
			GravityProp = serializedObject.FindProperty ("Gravity");
			RadiusProp = serializedObject.FindProperty ("Radius");
			RangeProp = serializedObject.FindProperty ("Range");
			SpringProp = serializedObject.FindProperty ("Spring");
			DamperProp = serializedObject.FindProperty ("Damper");
			LayerProp = serializedObject.FindProperty ("Layer");
			Collider_MaterialProp = serializedObject.FindProperty ("Collider_Material");
		
			if (Selection.activeGameObject) {
				parentTransform = Selection.activeGameObject.transform;
			}
		}

		public override void OnInspectorGUI ()
		{
			bool Work_Flag;
			if (parentTransform.parent == null || parentTransform.parent.gameObject.GetComponent<Rigidbody> () == null) {
				Work_Flag = false;
			} else {
				Work_Flag = true;
			}
		
			if (Work_Flag) {
				Set_Inspector ();
				if (GUI.changed) {
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

			// Balls settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Balls settings", MessageType.None, true);
			EditorGUILayout.Slider (DistanceProp, 0.1f, 10.0f, "Distance");
			EditorGUILayout.IntSlider (NumProp, 0, 30, "Number");
			EditorGUILayout.Slider (SpacingProp, 0.1f, 10.0f, "Spacing");
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (MassProp, 0.1f, 300.0f, "Mass");
			GravityProp.boolValue = EditorGUILayout.Toggle ("Use Gravity", GravityProp.boolValue);
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (RadiusProp, 0.01f, 10.0f, "SphereCollider Radius");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (RangeProp, 0.0f, 1.0f, "Movable Range");
			EditorGUILayout.Slider (SpringProp, 0.0f, 10000.0f, "Spring Force");
			EditorGUILayout.Slider (DamperProp, 0.0f, 10000.0f, "Damper Force");
			EditorGUILayout.Space ();
			LayerProp.intValue = EditorGUILayout.Popup ("Layer", LayerProp.intValue, layerNames);
			Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);

			// Update Value
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Update Value")) {
				Create ();
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
		
			// Create Ball	
			for (int i = 0; i < NumProp.intValue; i++) {
				Vector3 pos;
				pos.x = 0.0f;
				pos.y = DistanceProp.floatValue / 2.0f;
				pos.z = -SpacingProp.floatValue * i;
				Create_Ball ("L", pos, i + 1);
			}
			for (int i = 0; i < NumProp.intValue; i++) {
				Vector3 pos;
				pos.x = 0.0f;
				pos.y = -DistanceProp.floatValue / 2.0f;
				pos.z = -SpacingProp.floatValue * i;
				Create_Ball ("R", pos, i + 1);
			}
		}

		void Create_Ball (string direction, Vector3 position, int number)
		{
			//Create gameobject & Set transform
			GameObject gameObject = new GameObject ("SwingBall_" + direction + "_" + number);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			// SphereCollider
			SphereCollider sphereCollider;
			sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = RadiusProp.floatValue;
			sphereCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = MassProp.floatValue;
			rigidbody.useGravity = GravityProp.boolValue;
			// ConfigurableJoint
			ConfigurableJoint configJoint = gameObject.AddComponent < ConfigurableJoint > ();
			configJoint.connectedBody = parentTransform.parent.gameObject.GetComponent<Rigidbody> ();
			configJoint.anchor = Vector3.zero;
			configJoint.axis = Vector3.zero;
			configJoint.secondaryAxis = Vector3.zero;
			configJoint.xMotion = ConfigurableJointMotion.Locked;
			configJoint.yMotion = ConfigurableJointMotion.Limited;
			configJoint.zMotion = ConfigurableJointMotion.Locked;
			configJoint.angularXMotion = ConfigurableJointMotion.Locked;
			configJoint.angularYMotion = ConfigurableJointMotion.Locked;
			configJoint.angularZMotion = ConfigurableJointMotion.Locked;
			SoftJointLimit softJointLimit = configJoint.linearLimit; // Set Vertical Range
			softJointLimit.limit = RangeProp.floatValue;
			configJoint.linearLimit = softJointLimit;
			JointDrive jointDrive = configJoint.yDrive; // Set Vertical Spring.
			//jointDrive.mode = JointDriveMode.Position ;
			jointDrive.positionSpring = SpringProp.floatValue;
			jointDrive.positionDamper = DamperProp.floatValue;
			configJoint.yDrive = jointDrive;
			// Set Layer
			switch (LayerProp.intValue) {
			case 0:
				gameObject.layer = 10; // Reinforce (ignore all collisions)
				break;
			case 1:
				gameObject.layer = 0; // Default
				break;
			}
		}
	
	}

}