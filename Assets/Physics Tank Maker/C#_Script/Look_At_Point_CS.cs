using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Look_At_Point_CS : MonoBehaviour
	{

		public float Offset_X = 0.0f;
		public float Offset_Y = -1.0f;
		public float Offset_Z = 0.75f;
		public float Horizontal_Speed = 3.0f;
		public float Vertical_Speed = 2.0f;
		public bool Invert_Flag = false;
	
		Transform thisTransform;
		Vector3 initialPos;
		float angY;
		float angZ;
		bool isThirdPersonView = true;
		float horizontal;
		float vertical;
		int invertNum = 1;
		Camera mainCamera;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Start ()
		{
			thisTransform = transform;
			initialPos = thisTransform.localPosition;
			thisTransform.localPosition = initialPos + new Vector3 (Offset_X, Offset_Y, Offset_Z);
			angY = thisTransform.eulerAngles.y;
			angZ = thisTransform.eulerAngles.z;
			if (Invert_Flag) {
				invertNum = -1;
			} else {
				invertNum = 1;
			}
			mainCamera = GetComponentInChildren < Camera > ();
			// for bugs.
			this.enabled = false;
			this.enabled = true;
		}

		void Update ()
		{
			if (isCurrent && mainCamera.enabled) {
				switch (inputType) {
				case 0:
					KeyBoard_Input ();
					break;
				case 1:
					GamePad_Input ();
					break;
				case 2:
					KeyBoard_Input ();
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
				if (isThirdPersonView) {
					Rotate_TPS ();
				} else {
					Rotate_FPS ();
				}
			}
		}

		void KeyBoard_Input ()
		{
			horizontal = Input.GetAxis ("Horizontal2");
			vertical = Input.GetAxis ("Vertical2");
		}

		void GamePad_Input ()
		{
			if (Input.GetButton ("L_Button") == false) {
				horizontal = Input.GetAxis ("Horizontal2");
				vertical = Input.GetAxis ("Vertical2");
			} else {
				horizontal = 0.0f;
				vertical = 0.0f;
			}
		}

		Vector2 previousMousePos;

		void Mouse_Input ()
		{
			if (Input.GetMouseButtonDown (1)) {
				previousMousePos = Input.mousePosition;
			} else if (Input.GetMouseButton (1)) {
				horizontal = (Input.mousePosition.x - previousMousePos.x) * 0.1f;
				vertical = (Input.mousePosition.y - previousMousePos.y) * 0.1f;
				previousMousePos = Input.mousePosition;
			} else {
				horizontal = 0.0f;
				vertical = 0.0f;
			}
		}

		void Rotate_TPS ()
		{
			angY += horizontal * Horizontal_Speed;
			angZ -= vertical * Vertical_Speed * invertNum;
			thisTransform.eulerAngles = new Vector3 (0.0f, angY, angZ);
		}

		void Rotate_FPS ()
		{
			Vector3 tempAngles = thisTransform.localEulerAngles;
			tempAngles.y += horizontal * Horizontal_Speed;
			tempAngles.z -= vertical * Vertical_Speed * invertNum;
			thisTransform.localEulerAngles = tempAngles;
		}

		void Switch_View (bool isTPV)
		{ // Called from Main_Camera (Camera_Distance_CS).
			isThirdPersonView = isTPV;
			if (isThirdPersonView) { // Third Person View
				angY = thisTransform.eulerAngles.y;
				angZ = thisTransform.eulerAngles.z;
				thisTransform.localPosition = initialPos + new Vector3 (Offset_X, Offset_Y, Offset_Z);
			} else { // First Person View
				thisTransform.localEulerAngles = new Vector3 (0.0f, 90.0f, 0.0f);
				thisTransform.localPosition = initialPos;
			}	
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			// Change the parent to MainBody.
			thisTransform.parent = GetComponentInParent < MainBody_Setting_CS > ().transform;
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

	}

}