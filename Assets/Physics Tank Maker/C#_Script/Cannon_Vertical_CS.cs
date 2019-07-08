using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Cannon_Vertical_CS : MonoBehaviour
	{

		public float Max_Elevation = 13.0f;
		public float Max_Depression = 7.0f;
		public float Speed_Mag = 5.0f;
		public float Buffer_Angle = 5.0f;
		public bool Auto_Angle_Flag = true;
		public bool Upper_Course = false;
		public float OpenFire_Angle = 180.0f;

		public float Current_Rate; // Referred to from "Sound_Control_CS".
		public bool OpenFire_Flag; // Referred to from "Cannon_Fire".

		bool isMoving = false;
		bool isTracking = false;
		float angX;
		float grabity;
		Transform thisTransform;
		Transform parentTransform;
		Turret_Horizontal_CS turretScript;
		AI_CS aiScript;
		Bullet_Generator_CS generatorScript;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Complete_Turret ()
		{ // Called from 'Turret_Finishing".
			thisTransform = transform;
			parentTransform = transform.parent;
			angX = thisTransform.localEulerAngles.x;
			Max_Elevation = angX - Max_Elevation;
			Max_Depression = angX + Max_Depression;
			grabity = Physics.gravity.y;
			turretScript = parentTransform.GetComponent < Turret_Horizontal_CS > ();
			generatorScript = GetComponentInChildren < Bullet_Generator_CS > ();
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
			if (Input.GetKey ("z")) {
				Rotate (-Input.GetAxis ("Vertical"));
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("L_Button")) {
				Rotate (-Input.GetAxis ("Vertical2"));
			}
		}

		void Trigger_Input ()
		{
			if (Input.GetButton ("Fire1") == false && Input.GetButton ("Jump") == false && Input.GetAxis ("Vertical") != 0) {
				Rotate (-Input.GetAxis ("Vertical"));
			}
		}

		void Rotate (float rate)
		{
			angX += Speed_Mag * rate * Time.deltaTime;
			angX = Mathf.Clamp (angX, Max_Elevation, Max_Depression);
			thisTransform.localRotation = Quaternion.Euler (new Vector3 (angX, 0.0f, 0.0f));
		}

		void Start_Tracking (bool isAutoAngle)
		{ // Called from "Turret_Horizontal".
			isTracking = true;
			isMoving = true;
			Auto_Angle_Flag = isAutoAngle;
		}

		void Stop_Tracking ()
		{ // Called from "Turret_Horizontal".
			isTracking = false;
		}

		void Auto_Turn ()
		{
			float targetAng;
			if (isTracking) {
				// Calculate Angle.
				if (Auto_Angle_Flag) {
					targetAng = Auto_Angle ();
				} else {
					targetAng = Manual_Angle ();
				}
				targetAng += Mathf.DeltaAngle (0.0f, angX) + turretScript.Adjust_Ang.y;
			} else { //Return to the initial position.
				targetAng = -Mathf.DeltaAngle (angX, 0.0f); 
				if (Mathf.Abs (targetAng) < 0.01f) {
					isMoving = false;
				}
			}
			float sign = Mathf.Sign (targetAng);
			targetAng = Mathf.Abs (targetAng);
			// Calculate Turn Rate.
			Current_Rate = -Mathf.Lerp (0.0f, 1.0f, targetAng / (Speed_Mag * Time.fixedDeltaTime + Buffer_Angle)) * sign;
			// Rotate
			angX += Speed_Mag * Current_Rate * Time.fixedDeltaTime;
			angX = Mathf.Clamp (angX, Max_Elevation, Max_Depression);
			thisTransform.localRotation = Quaternion.Euler (new Vector3 (angX, 0.0f, 0.0f));
			// Set OpenFire_Flag.
			if (targetAng <= OpenFire_Angle) {
				OpenFire_Flag = true; // Referred to from "Cannon_Fire".
			} else {
				OpenFire_Flag = false; // Referred to from "Cannon_Fire".
			}
		}

		float Auto_Angle ()
		{ // Calculate the proper angle.
			float properAng;
			float distX = Vector2.Distance (new Vector2 (turretScript.Target_Pos.x, turretScript.Target_Pos.z), new Vector2 (thisTransform.position.x, thisTransform.position.z));
			float distY = turretScript.Target_Pos.y - thisTransform.position.y;
			float bulletVelocity = 0.0f;
			switch (generatorScript.Bullet_Type) {
			case 0:
				bulletVelocity = generatorScript.Bullet_Force;
				break;
			case 1:
				bulletVelocity = generatorScript.Bullet_Force_HE;
				break;
			}
			float posBase = (grabity * Mathf.Pow (distX, 2.0f)) / (2.0f * Mathf.Pow (bulletVelocity, 2.0f));
			float posX = distX / posBase;
			float posY = (Mathf.Pow (posX, 2.0f) / 4.0f) - ((posBase - distY) / posBase);
			if (posY >= 0.0f) {
				if (Upper_Course) {
					properAng = Mathf.Rad2Deg * Mathf.Atan (-posX / 2.0f + Mathf.Pow (posY, 0.5f));
				} else {
					properAng = Mathf.Rad2Deg * Mathf.Atan (-posX / 2.0f - Mathf.Pow (posY, 0.5f));
				}
			} else {
				properAng = 45.0f;
			}
			Vector3 forwardPos = parentTransform.forward;
			properAng -= Mathf.Rad2Deg * Mathf.Atan (forwardPos.y / Vector2.Distance (Vector2.zero, new Vector2 (forwardPos.x, forwardPos.z)));
			return properAng;
		}

		float Manual_Angle ()
		{ // Simply look to the target.
			float directAng;
			Vector3 localPos = parentTransform.InverseTransformPoint (turretScript.Target_Pos);
			directAng = Mathf.Rad2Deg * (Mathf.Asin ((localPos.y - thisTransform.localPosition.y) / Vector3.Distance (thisTransform.localPosition, localPos)));
			return directAng;
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			// Depress the cannon.
			thisTransform.localEulerAngles = new Vector3 (Max_Depression, 0.0f, 0.0f);
			Destroy (this);
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
			} else {
				isCurrent = false;
			}
		}

		void Get_AI (AI_CS script)
		{
			aiScript = script;
			Auto_Angle_Flag = true;
			OpenFire_Angle = aiScript.Fire_Angle;
		}

	}

}