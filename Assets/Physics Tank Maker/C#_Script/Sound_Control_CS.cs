using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Sound_Control_CS : MonoBehaviour
	{
		public int Type = 0;
		// Engine Sound from Driving Wheels.
		public float Min_Engine_Pitch = 1.0f;
		public float Max_Engine_Pitch = 2.0f;
		public float Min_Engine_Volume = 0.1f;
		public float Max_Engine_Volume = 0.3f;
		public float Max_Velocity = 7.0f;
		Rigidbody leftRigidbody;
		Rigidbody rightRigidbody;
		float leftCircumference;
		float rightCircumference;
		float currentRate;
		const float DOUBLE_PI = Mathf.PI * 2.0f;

		// Impact Sound from MainBody.
		public float Min_Impact = 0.25f;
		public float Max_Impact = 0.5f;
		public float Min_Impact_Pitch = 0.3f;
		public float Max_Impact_Pitch = 1.0f;
		public float Min_Impact_Volume = 0.1f;
		public float Max_Impact_Volume = 0.5f;
		float previousVelocity;
		bool isPrepared = true;
		Rigidbody mainRigidBody;
		float clipLength;

		// Turret Motor Sound.
		public float Max_Motor_Volume = 0.5f;
		Turret_Horizontal_CS turretScript;
		Cannon_Vertical_CS cannonScript;

		AudioSource thisAudioSource;

		bool isCurrent;
		int myID;

		void Awake ()
		{
			thisAudioSource = GetComponent < AudioSource > ();
			if (thisAudioSource == null) {
				Debug.LogError ("AudioSource cannot be found in " + transform.name);
				Destroy (this);
			}
			thisAudioSource.playOnAwake = false;
		}

		void Start ()
		{
			switch (Type) {
			case 0: // Engine Sound.
				thisAudioSource.loop = true;
				thisAudioSource.volume = 0.0f;
				thisAudioSource.Play ();
				if (transform.parent.GetComponentInChildren < Static_Track_CS > () == null && transform.parent.GetComponentInChildren < Track_Scroll_CS > () == null) { // Case of "Physics_Track" or "Scroll_Track".
					Set_Reference ();
				}
				break;
			case 1: // Impact Sound.
				thisAudioSource.loop = false;
				mainRigidBody = GetComponent < Rigidbody > ();
				clipLength = thisAudioSource.clip.length;
				break;
			case 2: // Turret Motor Sound.
				thisAudioSource.loop = true;
				thisAudioSource.volume = 0.0f;
				thisAudioSource.Play ();
				turretScript = GetComponent < Turret_Horizontal_CS > ();
				break;
			case 3: // Cannon Motor Sound.
				thisAudioSource.loop = true;
				thisAudioSource.volume = 0.0f;
				thisAudioSource.Play ();
				cannonScript = GetComponent < Cannon_Vertical_CS > ();
				break;
			}
		}

		// Case of "Physics_Track".
		void Set_Reference ()
		{
			Drive_Wheel_CS[] driveScripts = transform.parent.GetComponentsInChildren < Drive_Wheel_CS > ();
			foreach (Drive_Wheel_CS driveScript in driveScripts) {
				if (driveScript.name == "SprocketWheel_L") {
					leftRigidbody = driveScript.GetComponent < Rigidbody > ();
				} else if (driveScript.name == "SprocketWheel_R") {
					rightRigidbody = driveScript.GetComponent < Rigidbody > ();
				}
			}
			if (leftRigidbody == null || rightRigidbody == null) { // There is no SprocketWheel in the tank. (case of wheeled vehicle.)
				foreach (Drive_Wheel_CS driveScript in driveScripts) {
					if (driveScript.name == "SteeredWheel_L" && driveScript.Drive_Flag) {
						leftRigidbody = driveScript.GetComponent < Rigidbody > ();
					} else if (driveScript.name == "SteeredWheel_R" && driveScript.Drive_Flag) {
						rightRigidbody = driveScript.GetComponent < Rigidbody > ();
					}
				}
			}
			if (leftRigidbody && rightRigidbody) {
				leftCircumference = leftRigidbody.GetComponent < SphereCollider > ().radius * DOUBLE_PI;
				rightCircumference = rightRigidbody.GetComponent < SphereCollider > ().radius * DOUBLE_PI;
			} else {
				Debug.LogError ("Reference Wheels for the engine sound can not be found.");
				Destroy (this);
			}
		}
	
		// Case of "Static_Track".
		void Get_Static_Track (Static_Track_CS trackScript)
		{ // Called from "Static_Track".
			if (Type == 0) { // Engine Sound.
				if (trackScript.Reference_L && trackScript.Reference_R) {
					leftRigidbody = trackScript.Reference_L.GetComponent < Rigidbody > ();
					rightRigidbody = trackScript.Reference_R.GetComponent < Rigidbody > ();
					leftCircumference = trackScript.Reference_L.GetComponent < SphereCollider > ().radius * DOUBLE_PI;
					rightCircumference = trackScript.Reference_R.GetComponent < SphereCollider > ().radius * DOUBLE_PI;
				} else {
					Debug.LogWarning ("Reference Wheels for the engine sound can not be found.");
					Destroy (this);
				}
			}
		}

		// Case of "Scroll_Track".
		void Get_Track_Scroll (Track_Scroll_CS scrollScript)
		{ // Called from "Track_Scroll".
			if (Type == 0) { // Engine Sound.
				if (scrollScript.Reference_Wheel) {
					if (scrollScript.Reference_Wheel.localPosition.y > 0) {
						leftRigidbody = scrollScript.Reference_Wheel.GetComponent < Rigidbody > ();
						leftCircumference = scrollScript.Reference_Wheel.GetComponent < SphereCollider > ().radius * DOUBLE_PI;
					} else {
						rightRigidbody = scrollScript.Reference_Wheel.GetComponent < Rigidbody > ();
						rightCircumference = scrollScript.Reference_Wheel.GetComponent < SphereCollider > ().radius * DOUBLE_PI;
					}
				} else {
					Debug.LogWarning ("Reference Wheels for the engine sound can not be found.");
					Destroy (this);
				}
			}
		}

		void FixedUpdate ()
		{
			switch (Type) {
			case 0:
				Engine_Sound ();
				break;
			case 1:
				if (isCurrent && isPrepared) {
					StartCoroutine ("Impact_Sound");
				}
				break;
			case 2:
				if (isCurrent) {
					Turret_Motor_Sound ();
				}
				break;
			case 3:
				if (isCurrent) {
					Cannon_Motor_Sound ();
				}
				break;
			}
		}

		void Engine_Sound ()
		{
			float leftVelocity;
			float rightVelocity;
			if (leftRigidbody) {
				leftVelocity = leftRigidbody.angularVelocity.magnitude / DOUBLE_PI * leftCircumference;
			} else {
				leftVelocity = 0.0f;
			}
			if (rightRigidbody) {
				rightVelocity = rightRigidbody.angularVelocity.magnitude / DOUBLE_PI * rightCircumference;
			} else {
				rightVelocity = 0.0f;
			}
			float targetRate = (leftVelocity + rightVelocity) / 2.0f / Max_Velocity;
			currentRate = Mathf.MoveTowards (currentRate, targetRate, 0.02f);
			thisAudioSource.pitch = Mathf.Lerp (Min_Engine_Pitch, Max_Engine_Pitch, currentRate);
			thisAudioSource.volume = Mathf.Lerp (Min_Engine_Volume, Max_Engine_Volume, currentRate);
		}

		IEnumerator Impact_Sound ()
		{
			float currentVelocity = mainRigidBody.velocity.y;
			float impact = Mathf.Abs (previousVelocity - currentVelocity);
			if (impact > Min_Impact) {
				isPrepared = false;
				float rate = impact / Max_Impact;
				thisAudioSource.pitch = Mathf.Lerp (Min_Impact_Pitch, Max_Impact_Pitch, rate);
				thisAudioSource.volume = Mathf.Lerp (Min_Impact_Volume, Max_Impact_Volume, rate);
				thisAudioSource.Play ();
				yield return new WaitForSeconds (clipLength);
				isPrepared = true;
			}
			previousVelocity = mainRigidBody.velocity.y;
		}

		void Turret_Motor_Sound ()
		{
			float targetVolume = Mathf.Lerp (0.0f, Max_Motor_Volume, Mathf.Abs (turretScript.Current_Rate));
			thisAudioSource.volume = Mathf.MoveTowards (thisAudioSource.volume, targetVolume, 0.02f);
		}

		void Cannon_Motor_Sound ()
		{
			float targetVolume = Mathf.Lerp (0.0f, Max_Motor_Volume, Mathf.Abs (cannonScript.Current_Rate));
			thisAudioSource.volume = Mathf.MoveTowards (thisAudioSource.volume, targetVolume, 0.02f);
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			thisAudioSource.Stop ();
			Destroy (this);
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			if (Type != 1) { // except for Impact sound.
				thisAudioSource.Stop ();
				Destroy (this);
			}
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

	}

}