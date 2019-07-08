using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(Camera))]
	[ RequireComponent (typeof(AudioListener))]
	public class Gun_Camera_CS : MonoBehaviour
	{

		public Camera_Distance_CS MainCamera_Script;
		public string Reticle_Name = "Reticle";
		public float Small_Width = 0.4f;
		public float Small_Height = 0.4f;

		int mode = 0;
		Camera thisCamera;
		AudioListener thisListener;
		Image reticleImage;
		Vector2 reticleSize;
		bool reticleEnabled;

		float angleX;
		float zoomAxis;
		float angleAxis;

		RC_Camera_CS rcCameraScript;
	
		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			this.tag = "Untagged";
			thisCamera = GetComponent < Camera > ();
			thisCamera.enabled = false;
			thisCamera.cullingMask = -1;
			thisCamera.depth = 1;
			thisListener = GetComponent < AudioListener > ();
			if (thisListener == null) {
				thisListener = gameObject.AddComponent < AudioListener > ();
			}
			thisListener.enabled = false;
			AudioListener.volume = 1.0f;
			// Find the MainCamera.
			if (MainCamera_Script == null) {
				Transform mainTransform = GetComponentInParent < MainBody_Setting_CS > ().transform;
				MainCamera_Script = mainTransform.GetComponentInChildren < Camera_Distance_CS > ();
				if (MainCamera_Script == null) {
					Debug.LogError ("Gun_Camera cannont find the 'Main Camera'.");
					Destroy (this);
				}
			}
			// Find the Reticle Image.
			if (string.IsNullOrEmpty (Reticle_Name) == false) {
				GameObject reticleObject = GameObject.Find ("Reticle");
				if (reticleObject) {
					reticleImage = reticleObject.GetComponent < Image > ();
				}
				if (reticleImage) {
					reticleSize = reticleImage.rectTransform.sizeDelta;
				} else {
					Debug.LogWarning (Reticle_Name + " cannot be found in the scene.");
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
					GamePad_Input ();
					break;
				case 2:
					GamePad_Input ();
					break;
				case 3:
					GamePad_Input ();
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
				if (reticleImage) {
					reticleImage.enabled = reticleEnabled;
				}
			}
		}

		void KeyBoard_Input ()
		{
			if (Input.GetKeyDown ("r")) {
				mode += 1;
				if (mode > 2) {
					mode = 0;
				}
				Change_Mode ();
				return;
			}
			if (mode != 0) {
				if (Input.GetKey ("f")) { 
					zoomAxis = Input.GetAxisRaw ("Horizontal");
					angleAxis = Input.GetAxisRaw ("Vertical") * 0.05f;
					Zoom ();
					Rotate ();
				}
			}
		}

		void GamePad_Input ()
		{
			if (Input.GetButtonDown ("Fire2")) {
				mode += 1;
				if (mode > 2) {
					mode = 0;
				}
				Change_Mode ();
				return;
			}
			if (mode != 0) {
				if (Input.GetButton ("Jump")) {
					zoomAxis = Input.GetAxis ("Horizontal");
					angleAxis = Input.GetAxis ("Vertical") * 0.05f;
					Zoom ();
					Rotate ();
				}
			}
		}

		void Mouse_Input ()
		{
			if (Input.GetKeyDown ("r")) { 
				mode += 1;
				if (mode > 2) {
					mode = 0;
				}
				Change_Mode ();
				return;
			} else if (Input.GetMouseButtonDown (1) && mode == 2) {
				mode = 0; // Off
				Change_Mode ();
				return;
			}
			if (mode != 0 && Input.GetMouseButton (1) == false) {
				float inputAxis = Input.GetAxis ("Mouse ScrollWheel");
				if (inputAxis != 0.0f) {
					if (Input.GetKey ("f")) {
						angleAxis = inputAxis;
						Rotate ();
					} else {
						zoomAxis = inputAxis;
						Zoom ();
					}
				}
			}
		}

		void Change_Mode ()
		{
			switch (mode) {
			case 0: // Off
				thisCamera.enabled = false;
				thisListener.enabled = false;
				if (reticleImage) {
					reticleEnabled = false;
				}
				this.tag = "Untagged";
				break;
			case 1: // Small window.
				thisCamera.rect = new Rect (0.0f, 0.0f, Small_Width, Small_Height);
				thisCamera.enabled = true;
				thisListener.enabled = false;
				if (reticleImage) {
					reticleEnabled = true;
					reticleImage.transform.localScale = new Vector3 (Small_Width, Small_Height, 1.0f);
					reticleImage.transform.localPosition = new Vector3 ((-reticleSize.x * 0.5f) + (reticleSize.x * Small_Width * 0.5f), (-reticleSize.y * 0.5f) + (reticleSize.y * Small_Height * 0.5f), 0.0f);
				}
				this.tag = "Untagged";
				break;
			case 2: // Full screen.
				thisCamera.rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f);
				thisCamera.enabled = true;
				thisListener.enabled = true;
				if (reticleImage) {
					reticleEnabled = true;
					reticleImage.transform.localScale = Vector3.one;
					reticleImage.transform.localPosition = Vector3.zero;
				}
				this.tag = "MainCamera";
				break;
			}
			// Send message "Camera_Distance_CS" to Main_Camera.
			MainCamera_Script.Change_GunCamera_Mode (mode);
			// Send message to RC_Camera.
			if (rcCameraScript) {
				rcCameraScript.Change_GunCamera_Mode (mode);
			}
		}

		void Zoom ()
		{
			if (zoomAxis > 0.0f) {
				thisCamera.fieldOfView *= 0.9f;
			} else if (zoomAxis < 0.0f) {
				thisCamera.fieldOfView *= 1.1f;
			}
			thisCamera.fieldOfView = Mathf.Clamp (thisCamera.fieldOfView, 0.1f, 50.0f);
		}

		void Rotate ()
		{
			angleX -= angleAxis;
			angleX = Mathf.Clamp (angleX, 0.0f, 90.0f);
			transform.localEulerAngles = new Vector3 (angleX, 0.0f, 0.0f);
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			// Send message to "Camera_Distance_CS" in Main_Camera.
			MainCamera_Script.Change_GunCamera_Mode (0);
			if (isCurrent) {
				// Turn off Reticle Image.
				if (reticleImage) {
					reticleImage.enabled = false;
				}
				// Send message to RC_Camera.
				if (rcCameraScript) {
					rcCameraScript.Change_GunCamera_Mode (0);
				}
			}
			Destroy (this.gameObject);
		}

		public int Get_RC_Camera_Object (RC_Camera_CS tempScript)
		{ // Called from RC_Camera.
			rcCameraScript = tempScript;
			return mode;
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
				Change_Mode ();
			} else if (isCurrent) {
				isCurrent = false;
				thisCamera.enabled = false;
				thisListener.enabled = false;
				if (reticleImage) {
					reticleImage.enabled = false;
				}
			}
		}

	}

}