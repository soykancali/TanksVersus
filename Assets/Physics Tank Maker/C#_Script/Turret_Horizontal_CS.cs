using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	public class Turret_Horizontal_CS : MonoBehaviour
	{

		public bool Limit_Flag;
		public float Max_Right = 170.0f;
		public float Max_Left = 170.0f;
		public float Speed_Mag = 10.0f;
		public float Buffer_Angle = 5.0f;
		public float Acceleration_Time = 0.5f;
		public float Deceleration_Time = 0.1f;
		public float OpenFire_Angle = 180.0f;
		public string Aim_Marker_Name = "Aim_Marker";

		public Vector3 Target_Pos; // Referred to from "Cannon_Vertical" and "Cannon_Fire".
		public Vector3 Adjust_Ang; // Referred to from "Cannon_Vertical".
		public float Current_Rate; // Referred to from "Sound_Control_CS".
		public bool OpenFire_Flag = true; // Referred to from "Cannon_Fire".

		bool isMoving = false;
		bool isTracking = false;
		bool isTrouble = false;
		float angY;
		Transform targetTransform;
		Rigidbody targetRigidbody;
		Vector3 targetOffset;
		Transform thisTransform;
		Transform bodyTransform;
		Transform rootTransform;
		Image markerImage;
		Transform markerTransform;
		AI_CS aiScript;
		Bullet_Generator_CS generatorScript;
		Camera gunCam;
		int mode; // 0=Keep initial positon, 1=Free aiming, 2=Lock On.
		int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Start ()
		{
			// Find Marker Image.
			if (string.IsNullOrEmpty (Aim_Marker_Name) == false) {
				GameObject markerObject = GameObject.Find (Aim_Marker_Name);
				if (markerObject) {
					markerImage = markerObject.GetComponent < Image > ();
				}
				if (markerImage) {
					markerTransform = markerImage.transform;
				} else {
					Debug.LogWarning (Aim_Marker_Name + " cannot be found in the scene.");
				}
			}
		}

		void Complete_Turret ()
		{ // Called from 'Turret_Finishing" when the sorting is finished.
			thisTransform = transform;
			bodyTransform = transform.parent;
			rootTransform = thisTransform.root;
			angY = thisTransform.localEulerAngles.y;
			Max_Right = angY + Max_Right;
			Max_Left = angY - Max_Left;
			generatorScript = GetComponentInChildren < Bullet_Generator_CS > ();
			// Find Gun_Camera.
			Camera[] cameras = GetComponentsInChildren < Camera > ();
			foreach (Camera tempCam in cameras) {
				if (tempCam.tag == "Untagged") {
					gunCam = tempCam;
					break;
				}
			}
		}

		void Update ()
		{
			if (isCurrent) {
				switch (inputType) {
				case 0:
					KeyBoard_Input ();
					break;
				case 1:
					Stick_Input ();
					break;
				case 2:
					Trigger_Input ();
					break;
				case 3:
					Stick_Input ();
					break;
				}
			}
		}

		void LateUpdate ()
		{
			if (isCurrent) {
				switch (inputType) {
				case 4:
					Mouse_Input ();
					Marker_Control ();
					break;
				case 5:
					Mouse_Input ();
					Marker_Control ();
					break;
				case 10:
					Marker_Control ();
					break;
				}
			}
		}

		void Marker_Control ()
		{
			if (markerImage) {
				switch (mode) {
				case 0: // Keep initial positon
					markerImage.enabled = false;
					return;
				case 1: // Free aiming
					markerImage.enabled = true;
					markerImage.color = Color.white;
					break;
				case 2: // Lock On
					markerImage.enabled = true;
					markerImage.color = Color.red;
					break;
				}
				Vector3 tempPos = Camera.main.WorldToScreenPoint (Target_Pos);
				if (tempPos.z > 1000.0f) {
					tempPos.z = 1000.0f; // UI Image cannot be displayed over 1000 meters away from the camera.
				} else if (tempPos.z < 0.0f) { // Behind of the camera.
					markerImage.enabled = false;
				}
				markerTransform.position = tempPos;
			}
		}

		void FixedUpdate ()
		{
			if (isMoving) {
				switch (inputType) {
				case 4:
					Auto_Turn ();
					break;
				case 5:
					Auto_Turn ();
					break;
				case 10:
					Auto_Turn ();
					break;
				}
			}
		}

		void KeyBoard_Input ()
		{
			if ((Input.GetButton ("Fire1") || Input.GetKey ("z"))) {
				Rotate (Input.GetAxis ("Horizontal"));
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("L_Button")) {
				Rotate (Input.GetAxis ("Horizontal2"));
			}
		}

		void Trigger_Input ()
		{
			if (Input.GetButton ("Fire1") == false && Input.GetButton ("Jump") == false && Input.GetAxis ("Horizontal") != 0) {
				Rotate (Input.GetAxis ("Horizontal"));
			}
		}

		void Rotate (float rate)
		{
			if (isTrouble == false) {
				if (rate != 0.0f) {
					Current_Rate = Mathf.MoveTowards (Current_Rate, rate, Time.deltaTime / Acceleration_Time);
				} else {
					Current_Rate = Mathf.MoveTowards (Current_Rate, rate, Time.deltaTime / Deceleration_Time);
				}
				angY += Speed_Mag * Current_Rate * Time.deltaTime;
				if (Limit_Flag) {
					angY = Mathf.Clamp (angY, Max_Left, Max_Right);
				}
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, angY, 0.0f));
			}
		}

		Vector3 previousMousePos;

		void Mouse_Input ()
		{
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				switch (mode) {
				case 0: // Keep initial positon
					mode = 1;
					break;
				case 1: // Free aiming
					mode = 0;
					break;
				case 2: // Lock On
					mode = 1;
					break;
				}
				Switch_Mode ();
				return;
			} else if (Input.GetMouseButtonDown (2)) {
				Cast_Ray_LockOn (Input.mousePosition);
				mode = 2; // Lock On.
				Switch_Mode ();
				return;
			}
			switch (mode) {
			case 1: // Free aiming
				Cast_Ray_Free (Input.mousePosition);
				break;
			case 2: // Lock On
			// Adjust aiming.
				if (Input.GetKeyDown (KeyCode.Space)) {
					previousMousePos = Input.mousePosition;
				} else if (Input.GetKey (KeyCode.Space)) {
					if (Input.GetMouseButton (1) == false) {
						Adjust_Ang += (Input.mousePosition - previousMousePos) * 0.02f;
						previousMousePos = Input.mousePosition;
					}
				} else if (Input.GetKeyUp (KeyCode.Space)) {
					Adjust_Ang = Vector3.zero;
				}
				break;
			}
		}

		void Switch_Mode ()
		{
			switch (mode) {
			case 0:
				isTracking = false;
				BroadcastMessage ("Stop_Tracking", SendMessageOptions.DontRequireReceiver);// Send message to "Cannon_Vertical".
				break;
			case 1:
				targetTransform = null;
				isTracking = true;
				BroadcastMessage ("Start_Tracking", false, SendMessageOptions.DontRequireReceiver);// Send message to "Cannon_Vertical".
				isMoving = true;
				break;
			case 2:
				Adjust_Ang = Vector3.zero;
				isTracking = true;
				BroadcastMessage ("Start_Tracking", true, SendMessageOptions.DontRequireReceiver);// Send message to "Cannon_Vertical".
				isMoving = true;
				break;

			}
		}

		void Cast_Ray_LockOn (Vector3 mousePos)
		{
			// Detect the camera clicked.
			Camera currentCam;
			if (gunCam && gunCam.enabled && gunCam.pixelRect.Contains (mousePos)) { // Cursor is within gun camera window.
				currentCam = gunCam;
			} else { // Cursor is out of gun camera window.
				currentCam = Camera.main;
			}
			Ray ray = currentCam.ScreenPointToRay (mousePos);
			RaycastHit raycastHit;
			if (Physics.Raycast (ray, out raycastHit, 1500.0f, layerMask)) {
				Transform colliderTransform = raycastHit.collider.transform;
				if (colliderTransform.root != rootTransform) {
					if (raycastHit.transform.GetComponent < Rigidbody > ()) { //When 'raycastHit.collider.transform' is Turret, 'raycastHit.transform' is the MainBody.
						targetTransform = colliderTransform;
						targetOffset = targetTransform.InverseTransformPoint (raycastHit.point);
						if (targetTransform.localScale != Vector3.one) { // for Armor_Collider.
							targetOffset.x *= targetTransform.localScale.x;
							targetOffset.y *= targetTransform.localScale.y;
							targetOffset.z *= targetTransform.localScale.z;
						}
						return;
					} else {
						targetTransform = null;
					}
					Target_Pos = raycastHit.point;
				} else { // Ray hits itsel
					targetTransform = null;
					mousePos.z = 50.0f;
					Target_Pos = currentCam.ScreenToWorldPoint (mousePos);
				}
			} else { // Ray does not hit anythig.
				targetTransform = null;
				mousePos.z = 500.0f;
				Target_Pos = currentCam.ScreenToWorldPoint (mousePos);
			}
		}

		void Cast_Ray_Free (Vector3 mousePos)
		{
			// Detect the camera that the cursor is on.
			Camera currentCam;
			if (gunCam && gunCam.enabled && gunCam.pixelRect.Contains (mousePos)) { // Cursor is within gun camera window.
				currentCam = gunCam;
			} else { // Cursor is out of gun camera window.
				currentCam = Camera.main;
			}
			mousePos.z = 1500.0f;
			Target_Pos = currentCam.ScreenToWorldPoint (mousePos);
		}

		void AI_Set_Lock_On (Transform tempTransform)
		{ // Called from AI.
			targetTransform = tempTransform;
			targetRigidbody = targetTransform.GetComponent < Rigidbody > ();
			mode = 2;
			Switch_Mode ();
		}

		void AI_Reset_Lock_On ()
		{ // Called from AI.
			targetTransform = null;
			mode = 0;
			Switch_Mode ();
		}

		void Auto_Turn ()
		{
			float targetAng;
			if (isTracking) {
				// Update Target position.
				if (targetTransform) {
					Target_Pos = targetTransform.position + (targetTransform.forward * targetOffset.z) + (targetTransform.right * targetOffset.x) + (targetTransform.up * targetOffset.y);
				}
				// Calculate Angle.
				if (Limit_Flag == false) {
					Vector3 localPos = thisTransform.InverseTransformPoint (Target_Pos);
					targetAng = Vector2.Angle (Vector2.up, new Vector2 (localPos.x, localPos.z)) * Mathf.Sign (localPos.x);
				} else {
					Vector3 localPos = bodyTransform.InverseTransformPoint (Target_Pos);
					targetAng = Vector2.Angle (Vector2.up, new Vector2 (localPos.x, localPos.z)) * Mathf.Sign (localPos.x);
					targetAng -= angY;
				}
				targetAng += Adjust_Ang.x;
			} else { // Return to the initial position.
				targetAng = Mathf.DeltaAngle (thisTransform.localEulerAngles.y, 0.0f);
				if (Mathf.Abs (targetAng) < 0.01f) {
					isMoving = false;
				}
			}
			float sign = Mathf.Sign (targetAng);
			targetAng = Mathf.Abs (targetAng);
			// Calculate Turn Rate.
			float targetRate = Mathf.Lerp (0.0f, 1.0f, targetAng / (Speed_Mag * Time.fixedDeltaTime + Buffer_Angle)) * sign;
			if (targetAng > Buffer_Angle) {
				Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.fixedDeltaTime / Acceleration_Time);
			} else {
				Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.fixedDeltaTime / Deceleration_Time);
			}
			// Rotate
			if (isTrouble == false) {
				angY += Speed_Mag * Current_Rate * Time.fixedDeltaTime;
				if (Limit_Flag) {
					angY = Mathf.Clamp (angY, Max_Left, Max_Right);
					if (angY <= Max_Left || angY >= Max_Right) {
						Current_Rate = 0.0f;
					}
				}
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, angY, 0.0f));
			}
			// Set OpenFire_Flag.
			if (targetAng <= OpenFire_Angle) {
				OpenFire_Flag = true; // Referred to from "Cannon_Vertical".
			} else {
				OpenFire_Flag = false; // Referred to from "Cannon_Vertical".
			}
		}

		public void AI_Random_Offset ()
		{ // Called from "Cannon_Fire".
			Vector3 newOffset;
			newOffset.x = Random.Range (-1.0f, 1.0f);
			newOffset.y = Random.Range (-aiScript.AI_Lower_Offset, aiScript.AI_Upper_Offset);
			newOffset.z = Random.Range (-1.0f, 1.0f);
			// Lead distance
			float bulletVelocity = 0.0f;
			switch (generatorScript.Bullet_Type) {
			case 0:
				bulletVelocity = generatorScript.Bullet_Force;
				break;
			case 1:
				bulletVelocity = generatorScript.Bullet_Force_HE;
				break;
			}
			newOffset += targetTransform.InverseTransformPoint (targetRigidbody.position + targetRigidbody.velocity) * (aiScript.Target_Distance / bulletVelocity);
			targetOffset = newOffset;
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			if (isCurrent && markerImage) {
				markerImage.enabled = false;
			}
			Destroy (this);
		}

		public bool Trouble (float count)
		{ // Called from "Damage_Control_CS" in Turret.
			if (isTrouble == false) {
				isTrouble = true;
				StartCoroutine ("Trouble_Count", count);
				return true;
			} else {
				return false;
			}
		}

		IEnumerator Trouble_Count (float count)
		{
			yield return new WaitForSeconds (count);
			isTrouble = false;
		}

		void Set_Input_Type (int type)
		{
			inputType = type;
		}

		void Set_Tank_ID (int id)
		{
			myID = id;
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else if (isCurrent) {
				isCurrent = false;
				if (markerImage) {
					markerImage.enabled = false;
				}
			}
		}

		void Get_AI (AI_CS script)
		{
			aiScript = script;
			OpenFire_Angle = aiScript.Fire_Angle;
		}
	
	}

}