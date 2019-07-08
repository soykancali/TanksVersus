using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	
	public class Static_Wheel_CS : MonoBehaviour
	{

		public float Radius_Offset;

		Transform thisTransform;
		bool isLeft;
		float rate;
		MainBody_Setting_CS mainScript;
		Static_Track_CS trackScript;
		Track_Scroll_CS scrollScript;
		bool withStaticTrack;
		float previousReferenceAng;

		void Awake ()
		{
			thisTransform = transform;
			// Set direction. (Direction must be set in "Awake()", because "Get_Static_Track()" and "Get_Track_Scroll" is called in Start().
			if (thisTransform.localPosition.y > 0.0f) {
				isLeft = true;
			} else {
				isLeft = false;
			}
		}

		void Get_Static_Track (Static_Track_CS script)
		{
			trackScript = script;
			if (trackScript.Reference_L && trackScript.Reference_R) {
				// Set rate.
				float radius = GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset;
				if (isLeft) { // Left
					rate = trackScript.Reference_Radius_L / radius;
				} else { // Right
					rate = trackScript.Reference_Radius_R / radius;
				}
				withStaticTrack = true;
			} else {
				Debug.LogWarning ("Static_Wheel can not find the reference wheel in the Static_Tracks.");
				Destroy (this);
			}
		}

		void Get_Track_Scroll (Track_Scroll_CS script)
		{
			// Set rate.
			if (script.Reference_Wheel) {
				if ((isLeft && script.Reference_Wheel.localPosition.y > 0) || (isLeft == false && script.Reference_Wheel.localPosition.y < 0)) {
					scrollScript = script;
					float radius = GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset;
					float referenceRadius = scrollScript.Reference_Wheel.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
					rate = referenceRadius / radius;
					withStaticTrack = false;
					return;
				}
			} else {
				Debug.LogWarning ("Static_Wheel can not find the reference wheel in the Scroll_Tracks.");
				Destroy (this);
			}
		}
		
		void Update ()
		{
			if (mainScript.Visible_Flag) { // MainBody is visible by any camera.
				if (withStaticTrack) {
					Work_with_Static_Track ();
				} else {
					Work_with_Scroll_Track ();
				}
			}
		}

		void Work_with_Static_Track ()
		{
			if (trackScript) {
				Vector3 currentAng = thisTransform.localEulerAngles;
				if (isLeft) {
					currentAng.y -= trackScript.Delta_Ang_L * rate;
				} else {
					currentAng.y -= trackScript.Delta_Ang_R * rate;
				}
				thisTransform.localEulerAngles = currentAng;
			}
		}

		void Work_with_Scroll_Track ()
		{
			if (scrollScript) {
				float currentReferenceAng = scrollScript.Reference_Wheel.localEulerAngles.y;
				float deltaAng = Mathf.DeltaAngle (currentReferenceAng, previousReferenceAng);
				Vector3 currentAng = thisTransform.localEulerAngles;
				currentAng.y -= deltaAng * rate;
				thisTransform.localEulerAngles = currentAng;
				previousReferenceAng = currentReferenceAng;
			}
		}

		void TrackBroken_Linkage (int tempDirection)
		{ // Called from "Damage_Control_CS" in TrackCollider.
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
			// Resize SphereCollider.
			SphereCollider sphereCollider = GetComponent <SphereCollider> ();
			if (sphereCollider) {
				MeshFilter meshFilter = GetComponent < MeshFilter > ();
				if (meshFilter && meshFilter.mesh) {
					sphereCollider.radius = meshFilter.mesh.bounds.extents.x;
				}
			}
			// Remove this script.
			Destroy (this);
		}

		void Get_MainScript (MainBody_Setting_CS tempScript)
		{ // Called from "MainBody_Setting_CS".
			mainScript = tempScript;
		}

	}
		
}