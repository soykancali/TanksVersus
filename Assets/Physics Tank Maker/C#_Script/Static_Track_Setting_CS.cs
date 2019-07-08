using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	
	public class Static_Track_Setting_CS : MonoBehaviour
	{

		Transform frontTransform;
		Transform rearTransform;

		int type;
		string anchorName;
		string anchorParentName;

		void Start ()
		{
			Transform parentTransform = transform.parent;
			string baseName = this.name.Substring (0, 12); // e.g. "TrackBelt_L_"
			int thisNum = int.Parse (this.name.Substring (12)); // e.g. "1"
			// Find front piece.
			frontTransform = parentTransform.Find (baseName + (thisNum + 1)); // Find a piece having next number.
			if (frontTransform == null) { // It must be the last piece.
				frontTransform = parentTransform.Find (baseName + 1); // The 1st piece.
			}
			// Find rear piece.
			rearTransform = parentTransform.Find (baseName + (thisNum - 1)); // Find a piece having previous number.
			if (rearTransform == null) { // It must be the 1st piece.
				rearTransform = transform.parent.Find (baseName + (transform.parent.childCount / 2)); // The last piece.
			}
		}

		void OnCollisionStay (Collision collision)
		{
			foreach (ContactPoint contactPoint in collision.contacts) {
				if (contactPoint.otherCollider.transform.parent) {
					if (contactPoint.otherCollider.name.Length >= 9 && contactPoint.otherCollider.name.Substring (0, 9) == "RoadWheel") {
						anchorName = contactPoint.otherCollider.name;
						anchorParentName = contactPoint.otherCollider.transform.parent.name;
						type = 1; // Anchor type
					}
				}
			}
		}

		void OnCollisionExit ()
		{
			anchorName = null;
			anchorParentName = null;
			type = 0; // Static type
		}

		void Set_Static_Track_Value ()
		{ // Called from "Create_TrackBelt_CSEditor".
			// Add Script and set values.
			Static_Track_CS trackScript = gameObject.AddComponent < Static_Track_CS > ();
			trackScript.Front_Transform = frontTransform;
			trackScript.Rear_Transform = rearTransform;
			if (type == 1) { // Anchor type
				trackScript.Type = 1;
				trackScript.Anchor_Name = anchorName;
				trackScript.Anchor_Parent_Name = anchorParentName;
			} else if (frontTransform.GetComponent < Static_Track_Setting_CS > ().type == 1) { // The front piece is Anchor type.
				trackScript.Type = 2; // Dynamic type
			} else if (rearTransform.GetComponent < Static_Track_Setting_CS > ().type == 1) { // The rear piece is Anchor type.
				trackScript.Type = 2; // Dynamic type
			} else {
				trackScript.Type = 0; // Static type
			}
			trackScript.enabled = false;
			// Remove needless components.
			HingeJoint hingeJoint = GetComponent < HingeJoint > ();
			if (hingeJoint) {
				Destroy (hingeJoint);
			}
			Rigidbody rigidbody = GetComponent < Rigidbody > ();
			if (rigidbody) {
				Destroy (rigidbody);
			}
			Damage_Control_CS damageScript = GetComponent < Damage_Control_CS > ();
			if (damageScript) {
				Destroy (damageScript);
			}
			Stabilizer_CS stabilizerScript = GetComponent < Stabilizer_CS > ();
			if (stabilizerScript) {
				Destroy (stabilizerScript);
			}
			// Disable BoxCollider.
			BoxCollider boxCollider = GetComponent < BoxCollider > ();
			if (boxCollider) {
				boxCollider.enabled = false;
			}
			// Remove child objects such as Reinforce piece.
			for (int i = 0; i < transform.childCount; i++) {
				Destroy (transform.GetChild (0).gameObject);
			}
			// Destroy this script/
			Destroy (this);
		}

	}

}