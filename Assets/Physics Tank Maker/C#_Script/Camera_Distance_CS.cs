using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(Camera))]
	[ RequireComponent (typeof(AudioListener))]
	public class Camera_Distance_CS : MonoBehaviour
	{
		public float FPV_FOV = 50.0f;
		public float TPV_FOV = 30.0f;
		public float Clipping_Planes_Near = 0.05f;
		public float Min_Distance = 1.0f;
		public float Max_Distance = 30.0f;

		Transform thisTransform;
		Transform parentTransform;
		float currentDistance;
		float targetDistance;

		Camera thisCamera;
		AudioListener thisAudioListener;
		bool isTPV = true;
		int gunCamMode = 0;
		bool isRcCamEnabled = false;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			this.tag = "MainCamera";
			thisCamera = GetComponent < Camera > ();
			thisCamera.enabled = false;
			thisCamera.cullingMask = -1;
			thisCamera.depth = 0;
			thisCamera.nearClipPlane = Clipping_Planes_Near;
			thisCamera.fieldOfView = TPV_FOV;
			thisAudioListener = GetComponent < AudioListener > ();
			thisAudioListener.enabled = false;
			thisTransform = transform;
			parentTransform = thisTransform.parent;
			thisTransform.LookAt (parentTransform);
		}

		void Update ()
		{
			if (isCurrent && thisCamera.enabled) {
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
				case 4:
					Mouse_Input ();
					break;
				case 5:
					Mouse_Input ();
					break;
				case 10:
					Mouse_Input ();
					break;
				}
			}
		}

		void KeyBoard_Input ()
		{
			if (Input.GetKey ("e")) {
				Forward (0.5f);
			} else if (Input.GetKey ("q")) {
				Backward (0.5f);
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("Fire3")) {
				Forward (0.5f);
			} else if (Input.GetButton ("Fire1")) {
				Backward (0.5f);
			}
		}

		void Trigger_Input ()
		{
			if (Input.GetButton ("Fire1") && Input.GetAxis ("Vertical") > 0) {
				Forward (0.5f);
			} else if (Input.GetButton ("Fire1") && Input.GetAxis ("Vertical") < 0) {
				Backward (0.5f);
			}
		}

		void Mouse_Input ()
		{
			if (Input.GetMouseButton (1)) {
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					targetDistance -= 3.0f;
				} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					targetDistance += 3.0f;
				}
			}
			if (targetDistance - currentDistance < -0.1f) {
				currentDistance = Mathf.MoveTowards (currentDistance, targetDistance, 0.5f);
				Forward (0.5f);
			} else if (targetDistance - currentDistance > 0.1f) {
				currentDistance = Mathf.MoveTowards (currentDistance, targetDistance, 0.5f);
				Backward (0.5f);
			} else {
				targetDistance = 0.0f;
				currentDistance = 0.0f;
			}
		}

		void Forward (float rate)
		{
			if (Vector3.Distance (thisTransform.position, parentTransform.position) > Min_Distance) {
				thisTransform.position += thisTransform.forward * rate;
			} else { 
				if (isTPV) {
					isTPV = false;
					parentTransform.SendMessage ("Switch_View", isTPV, SendMessageOptions.DontRequireReceiver); // Send Message to "Look_At_Point".
					thisTransform.localPosition = Vector3.zero;
					thisCamera.fieldOfView = FPV_FOV;
				}
				targetDistance = 0.0f;
				currentDistance = 0.0f;
			}
		}

		void Backward (float rate)
		{
			if (Vector3.Distance (thisTransform.position, parentTransform.position) < Max_Distance) {
				thisTransform.position -= thisTransform.forward * rate;
				if (isTPV == false) {
					isTPV = true;
					parentTransform.SendMessage ("Switch_View", isTPV, SendMessageOptions.DontRequireReceiver); // Send Message to "Look_At_Point".
					thisTransform.position -= thisTransform.forward * 3.0f;
					thisCamera.fieldOfView = TPV_FOV;
				}
			} else {
				targetDistance = 0.0f;
				currentDistance = 0.0f;
			}
		}

		public void Change_GunCamera_Mode (int tempMode)
		{ // Called from "Gun_Camera". (Also called when the turret is broken.)
			gunCamMode = tempMode;
			if (isCurrent) {
				Control_Enabled ();
			}
		}

		void Switch_Camera (bool flag)
		{ // Called from "RC_Camera".
			isRcCamEnabled = flag;
			Control_Enabled ();
		}

		void Control_Enabled ()
		{
			if (gunCamMode == 2 || isRcCamEnabled) { // Gun_Camera is full screen or RC_Camera is enabled now.
				thisCamera.enabled = false;
				thisAudioListener.enabled = false;
				this.tag = "Untagged";
			} else {
				thisCamera.enabled = true;
				thisAudioListener.enabled = true;
				this.tag = "MainCamera";
			}
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
				Control_Enabled ();
			} else if (isCurrent) {
				isCurrent = false;
				thisCamera.enabled = false;
				thisAudioListener.enabled = false;
			}
		}

	}

}