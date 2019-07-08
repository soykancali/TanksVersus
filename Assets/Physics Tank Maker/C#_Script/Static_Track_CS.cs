using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Static_Track_CS : MonoBehaviour
	{

		public int Type; // 0=Static, 1=Anchor, 2=Dynamic, 9=Parent.
		public Transform Front_Transform;
		public Transform Rear_Transform;
		public string Anchor_Name;
		public string Anchor_Parent_Name;
		public Transform Anchor_Transform;
		public Transform Reference_L; // Referred to from "Sound_Control_CS".
		public Transform Reference_R; // Referred to from "Sound_Control_CS".
		public string Reference_Name_L;
		public string Reference_Name_R;
		public string Reference_Parent_Name_L;
		public string Reference_Parent_Name_R;
		public float Length;
		public float Radius_Offset;
		public float Mass = 30.0f;

		// Referred to from all Static_Wheels.
		public float Reference_Radius_L;
		public float Reference_Radius_R;
		public float Delta_Ang_L;
		public float Delta_Ang_R;

		Transform thisTransform;
		bool isLeft; // Left = true.
		float invertValue; // Lower piece = 0.0f, Upper pieces = 180.0f.
		float Rate_L;
		float Rate_R;
		Vector3 invisiblePos;
		float invisibleAngY;
		Static_Track_CS frontScript;
		Static_Track_CS rearScript;
		Static_Track_CS parentScript;
		float halfLength;
		MainBody_Setting_CS mainScript;
		// only for Anchor.
		float initialPosX;
		float anchorInitialPosX;
		// only for Parent.
		float leftPreviousAng;
		float rightPreviousAng;
		float leftAngRate;
		float rightAngRate;
		int piecesCount;

		void Start ()
		{
			thisTransform = transform;
			switch (Type) {
			case 0: // Static.
				Initial_Settings ();
				break;
			case 1: // Anchor.
				Initial_Settings ();
				Find_Anchor ();
				break;
			case 2: // Dynamic.
				Initial_Settings ();
				break;
			case 9: // Parent.
				Parent_Settings ();
				break;
			}
		}

		void Initial_Settings ()
		{
			// Set initial position and angle.
			invisiblePos = thisTransform.localPosition;
			invisibleAngY = thisTransform.localEulerAngles.y ;
			// Set direction.
			if (invisiblePos.y > 0.0f) {
				isLeft = true; // Left
			} else {
				isLeft = false; // Right
			}
			// Set invertValue.
			if (invisibleAngY > 90.0f && invisibleAngY < 270.0f) {  // Upper piece
				invertValue = 180.0f;
			} else { // Lower piece
				invertValue = 0.0f;
			}
			// Find front, rear and parent scripts.
			if (Front_Transform) {
				frontScript = Front_Transform.GetComponent < Static_Track_CS > ();
			}
			if (Rear_Transform) {
				rearScript = Rear_Transform.GetComponent < Static_Track_CS > ();
			}
			parentScript = thisTransform.parent.GetComponent < Static_Track_CS > ();
			// Get the half length of the piece.
			halfLength = parentScript.Length * 0.5f;
		}

		void Find_Anchor ()
		{
			if (Anchor_Transform == null) { // Anchor_Transform is lost.
				// Find Anchor with reference to the name.
				if (string.IsNullOrEmpty (Anchor_Name) == false && string.IsNullOrEmpty (Anchor_Parent_Name) == false) {
					Anchor_Transform = thisTransform.parent.parent.Find (Anchor_Parent_Name + "/" + Anchor_Name);
				}
			}
			// Set initial hight.
			if (Anchor_Transform) {
				initialPosX = thisTransform.localPosition.x; // Axis X = hight.
				anchorInitialPosX = Anchor_Transform.localPosition.x; // Axis X = hight.
			} else {
				//Debug.LogWarning ("Anchor_Transform of " + thisTransform.name + " cannot be found.");
				Type = 0; // change the Type to 'Static'.
			}
		}

		void Parent_Settings ()
		{
			// Find Reference Wheels.
			if (Reference_L == null) { // Left Reference Wheel is lost.
				if (string.IsNullOrEmpty (Reference_Name_L) == false && string.IsNullOrEmpty (Reference_Parent_Name_L) == false) {
					Reference_L = thisTransform.parent.Find (Reference_Parent_Name_L + "/" + Reference_Name_L);
				}
			}
			if (Reference_R == null) { // Right Reference Wheel is lost.
				if (string.IsNullOrEmpty (Reference_Name_R) == false && string.IsNullOrEmpty (Reference_Parent_Name_R) == false) {
					Reference_R = thisTransform.parent.Find (Reference_Parent_Name_R + "/" + Reference_Name_R);
				}
			}
			// Set "Reference_Radius" and "Rate_Ang".
			if (Reference_L && Reference_R) {
				Reference_Radius_L = Reference_L.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
				leftAngRate = 360.0f / ((2.0f * Mathf.PI * Reference_Radius_L) / Length);
				Reference_Radius_R = Reference_R.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
				rightAngRate = 360.0f / ((2.0f * 3.14f * Reference_Radius_R) / Length);
			} else {
				Debug.LogError ("'Reference Wheels' for Static_Track cannot be found. " + thisTransform.root.name);
				this.enabled = false;
			}
			// Send this reference to all the "Static_Wheels" and "Sound_Control".
			thisTransform.parent.BroadcastMessage ("Get_Static_Track", this, SendMessageOptions.DontRequireReceiver);
			// Set piecesCount.
			Static_Track_CS [] childScripts = thisTransform.GetComponentsInChildren < Static_Track_CS > ();
			piecesCount = (childScripts.Length - 1 ) / 2;
		}

		void Update ()
		{
			if (mainScript.Visible_Flag && Type == 9) { // MainBody is visible by any camera, && This is the parent object.
				Speed_Control ();
			}
		}

		void LateUpdate ()
		{
			if (mainScript.Visible_Flag) { // MainBody is visible by any camera.
				switch (Type) {
				case 0: // Static.
					Slide_Control ();
					break;
				case 1: // Anchor.
					Anchor_Control ();
					Slide_Control ();
					break;
				case 2: // Dynamic.
					Dynamic_Control ();
					Slide_Control ();
					break;
				}
			}
		}

		void Anchor_Control ()
		{
			// Set position.
			invisiblePos.x = initialPosX + (Anchor_Transform.localPosition.x - anchorInitialPosX);  // Axis X = hight.
			// Calculate end positions.
			float tempRad = rearScript.invisibleAngY * Mathf.Deg2Rad;
			Vector3 rearEndPos = rearScript.invisiblePos + new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
			tempRad = frontScript.invisibleAngY * Mathf.Deg2Rad;
			Vector3 frontEndPos = frontScript.invisiblePos - new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
			// Set angle.
			invisibleAngY = Mathf.Rad2Deg * Mathf.Atan ((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + invertValue;
		}

		void Dynamic_Control ()
		{
			// Calculate end positions.
			float tempRad = rearScript.invisibleAngY * Mathf.Deg2Rad;
			Vector3 rearEndPos = rearScript.invisiblePos + new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
			tempRad = frontScript.invisibleAngY * Mathf.Deg2Rad;
			Vector3 frontEndPos = frontScript.invisiblePos - new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
			// Set position.
			invisiblePos = Vector3.Lerp (rearEndPos, frontEndPos, 0.5f);
			// Set angle.
			invisibleAngY = Mathf.Rad2Deg * Mathf.Atan ((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + invertValue;
		}

		void Slide_Control ()
		{
			if (isLeft) { // Left
				thisTransform.localPosition = Vector3.Lerp (invisiblePos, rearScript.invisiblePos, parentScript.Rate_L);
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, Mathf.LerpAngle (invisibleAngY, rearScript.invisibleAngY, parentScript.Rate_L), 270.0f));
			} else { // Right
				thisTransform.localPosition = Vector3.Lerp (invisiblePos, rearScript.invisiblePos, parentScript.Rate_R);
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, Mathf.LerpAngle (invisibleAngY, rearScript.invisibleAngY, parentScript.Rate_R), 270.0f));
			}
		}

		void Speed_Control ()
		{
			// Left
			float currentAng = Reference_L.localEulerAngles.y;
			Delta_Ang_L = Mathf.DeltaAngle (currentAng, leftPreviousAng); // Also referred to from Static_Wheels.
			Rate_L = Calculate_Rate (Rate_L, Delta_Ang_L, leftAngRate);
			leftPreviousAng = currentAng;
			// Right
			currentAng = Reference_R.localEulerAngles.y;
			Delta_Ang_R = Mathf.DeltaAngle (currentAng, rightPreviousAng); // Also referred to from Static_Wheels.
			Rate_R = Calculate_Rate (Rate_R, Delta_Ang_R, rightAngRate);
			rightPreviousAng = currentAng;
		}

		float Calculate_Rate (float rate, float diffAng, float angRate)
		{
			rate += (diffAng / angRate) % 1.0f;
			if (rate > 1.0f) {
				rate -= 1.0f;
			} else if (rate < 0.0f) {
				rate += 1.0f;
			}
			return rate;
		}

		void Start_Breaking ()
		{ // Called from "Damage_Control_CS" in TrackCollider.
			if (this.enabled) {
				// Reset parent script.
				if (parentScript) {
					parentScript.Rate_L = 0.0f;
					parentScript.Rate_R = 0.0f;
				}
				// Add Components into this piece.
				Add_Components (thisTransform);
				// Add Components into other pieces.
				Static_Track_CS tempScript = this;
				for (int i = 0; i < parentScript.piecesCount; i++) {
					Add_Components (tempScript.Front_Transform);
					tempScript = tempScript.frontScript;
				}
				// Add HingeJoint into front pieces.
				tempScript = this;
				for (int i = 0; i < parentScript.piecesCount - 1; i++) { // Add HingeJoint except for this piece.
					Add_HingeJoint (tempScript.Front_Transform, tempScript.frontScript.Front_Transform);
					tempScript = tempScript.frontScript;
				}
				// Disable and Destroy the pieces on the same side.
				thisTransform.parent.BroadcastMessage ("Disable_and_Destroy", isLeft, SendMessageOptions.DontRequireReceiver);
			}
		}

		void Add_Components (Transform tempTransform)
		{
			// Add RigidBody.
			if (tempTransform.GetComponent < Rigidbody > () == null) {
				Rigidbody rigidbody = tempTransform.gameObject.AddComponent < Rigidbody > ();
				rigidbody.mass = Mass;
			}
			// Enable BoxCollider.
			BoxCollider boxCollider = tempTransform.GetComponent < BoxCollider > ();
			boxCollider.enabled = true;
		}

		void Add_HingeJoint (Transform baseTransform, Transform connectedTransform)
		{
			HingeJoint hingeJoint = baseTransform.gameObject.AddComponent < HingeJoint > ();
			hingeJoint.connectedBody = connectedTransform.GetComponent < Rigidbody > ();
			float anchorZ = baseTransform.GetComponent < BoxCollider > ().size.z * 0.5f;
			hingeJoint.anchor = new Vector3 (0.0f, 0.0f, anchorZ);
			hingeJoint.axis = new Vector3 (1.0f, 0.0f, 0.0f);
		}

		IEnumerator Disable_and_Destroy (bool direction)
		{
			if (Type != 9 && direction == isLeft) {
				this.enabled = false;
				yield return new WaitForSeconds (1.0f);
				thisTransform.parent = null;
				Destroy (this.gameObject, 20.0f);
			}
		}

		void Get_MainScript (MainBody_Setting_CS tempScript)
		{ // Called from "MainBody_Setting_CS".
			mainScript = tempScript;
		}

	}

}