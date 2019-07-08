using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Drive_Wheel_CS : MonoBehaviour
	{
	
		public bool Drive_Flag = true;
		public float Radius = 0.3f;

		Rigidbody thisRigidbody;
		Transform thisTransform;
		bool isLeft;
		bool isSteeredWheel;
		float maxAngVelocity;

		Quaternion storedRot;
		bool isFixed = false;

		Drive_Control_CS controlScript;

		void Start ()
		{
			thisRigidbody = GetComponent < Rigidbody > ();
			thisTransform = transform;
			// Set direction
			if (transform.localEulerAngles.z == 0.0f) {
				isLeft = true; // Left
			} else {
				isLeft = false; // Right
			}
			// Set isSteeredWheel.
			if (GetComponentInParent < Steer_Wheel_CS > ()) { // Steered Wheel.
				isSteeredWheel = true;
			} else {
				isSteeredWheel = false;
			}
		}

		void Update ()
		{ // only for wheels with Physics_Track.
			if (controlScript.Fix_Useless_Rotaion) {
				Fix_Rotaion ();
			}
		}

		void Fix_Rotaion ()
		{ // only for wheels with Physics_Track.
			if (controlScript.Parking_Brake) {
				if (isFixed == false) {
					isFixed = true;
					storedRot = thisTransform.localRotation;
				}
			} else {
				isFixed = false;
				return;
			}
			if (isFixed) {
				thisTransform.localRotation = storedRot;
			}
		}

		void FixedUpdate ()
		{
			if (controlScript.Acceleration_Flag) {
				Acceleration_Mode ();
			} else {
				Constant_Mode ();
			}
		}

		void Acceleration_Mode ()
		{
			if (isLeft) { // Left
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = maxAngVelocity * controlScript.L_Speed_Rate;
				// Set Angular Drag.
				if (isSteeredWheel == false) {
					thisRigidbody.angularDrag = controlScript.L_Brake_Drag;
				}
				// Add Torque.
				if (Drive_Flag) {
					if (controlScript.Is_Forward_L) { // Forward.
						thisRigidbody.AddRelativeTorque (0.0f, -controlScript.Torque, 0.0f);
					} else { // Backward.
						thisRigidbody.AddRelativeTorque (0.0f, controlScript.Torque, 0.0f);
					}
				}
			} else { // Right
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = maxAngVelocity * controlScript.R_Speed_Rate;
				// Set Angular Drag.
				if (isSteeredWheel == false) {
					thisRigidbody.angularDrag = controlScript.R_Brake_Drag;
				}
				// Add Torque.
				if (Drive_Flag) {
					if (controlScript.Is_Forward_R) { // Forward.
						thisRigidbody.AddRelativeTorque (0.0f, controlScript.Torque, 0.0f);
					} else { // Backward.
						thisRigidbody.AddRelativeTorque (0.0f, -controlScript.Torque, 0.0f);
					}
				}
			}
		}

		void Constant_Mode ()
		{
			if (isLeft) { // Left
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = Mathf.Abs (maxAngVelocity * controlScript.L_Input_Rate);
				// Set Angular Drag.
				if (isSteeredWheel == false) {
					thisRigidbody.angularDrag = controlScript.L_Brake_Drag;
				}
				// Add Torque.
				if (Drive_Flag) {
					thisRigidbody.AddRelativeTorque (0.0f, controlScript.Torque * Mathf.Sign (controlScript.L_Input_Rate), 0.0f);
				}
			} else { // Right
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = Mathf.Abs (maxAngVelocity * controlScript.R_Input_Rate);
				// Set Angular Drag.
				if (isSteeredWheel == false) {
					thisRigidbody.angularDrag = controlScript.R_Brake_Drag;
				}
				// Add Torque.
				if (Drive_Flag) {
					thisRigidbody.AddRelativeTorque (0.0f, controlScript.Torque * Mathf.Sign (controlScript.R_Input_Rate), 0.0f);
				}
			}
		}

		void Get_Drive_Control (Drive_Control_CS script)
		{ // Called from "Drive_Control" in the MainBody.
			controlScript = script;
			maxAngVelocity = Mathf.Deg2Rad * ((controlScript.Max_Speed / (2.0f * Radius * Mathf.PI)) * 360.0f);
			// To solve physics issue of Unity5.
			maxAngVelocity = Mathf.Clamp (maxAngVelocity, 0.0f, controlScript.MaxAngVelocity_Limit);
		}

		void TrackBroken_Linkage (int tempDirection)
		{ // Called from "Damage_Control_CS" in Physics_Track, TrackCollider.
			switch (tempDirection){
			 case 0: // Left
				if (isLeft == false) {
					return;
				}
				break;
			case 1: // Right
				if (isLeft){
					return;
				}
				break;
			case 2: // Both
				break;
			}
			// Lock the wheels.
			thisRigidbody.angularDrag = Mathf.Infinity;
			// Resize SphereCollider.
			MeshFilter meshFilter = GetComponent < MeshFilter > ();
			if (meshFilter && meshFilter.mesh) {
				SphereCollider sphereCollider = GetComponent <SphereCollider> ();
				if (sphereCollider) {
					sphereCollider.radius = meshFilter.mesh.bounds.extents.x;
				}
			}
			// Remove this script.
			Destroy (this);
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			// Lock the wheels.
			thisRigidbody.angularDrag = Mathf.Infinity;
			// Disable this script.
			this.enabled = false ;
		}
	
	}

}