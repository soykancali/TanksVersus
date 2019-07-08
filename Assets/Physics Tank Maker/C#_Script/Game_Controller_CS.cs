using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ChobiAssets.PTM
{

	[ System.Serializable]
	public class TankProp
	{
		public Tank_ID_Control_CS topScript;
		public MainBody_Setting_CS bodyScript;
		public Transform bodyTransform;
		public AI_CS aiScript;
	}

	public class Game_Controller_CS : MonoBehaviour
	{

		public float Assign_Frequency = 3.0f;
		public float Time_Scale = 1.0f;
		public float Gravity = -9.81f;
		public float Fixed_TimeStep = 0.02f;

		public RC_Camera_CS RC_Cam_Script; // Set by "RC_Camera_CS".

		public Tank_ID_Control_CS[] Operable_Tanks; // Referred to from RC_Camera.
		List < TankProp > friendlyTanks = new List < TankProp > ();
		List < TankProp > hostileTanks = new List < TankProp > ();

		float assignCount;
		float currentTimeScale;
		int currentID = 1;

		void Awake ()
		{
			this.tag = "GameController"; // This tag is referred to by "Tank_ID_Control".
			// Modify the Physics and Time manager.
			currentTimeScale = Time_Scale;
			Time.timeScale = currentTimeScale;
			Physics.gravity = new Vector3 (0.0f, Gravity, 0.0f);
			Physics.sleepThreshold = 0.5f;
			Time.fixedDeltaTime = Fixed_TimeStep;
			// Declare array.
			Operable_Tanks = new Tank_ID_Control_CS [ 11 ];
		}

		public void Receive_ID (Tank_ID_Control_CS topScript)
		{ // Called from "Tank_ID_Control" at the opening.
			if (topScript.Tank_ID != 0) { // Operable Tank
				if (Operable_Tanks [topScript.Tank_ID] == null) { // Operable_Tanks[ # ] is empty.
					Operable_Tanks [topScript.Tank_ID] = topScript;
					Store_Components (topScript);
					return;
				} else { // Operable_Tanks[ # ] is not empty.
					for (int i = 1; i < Operable_Tanks.Length; i++) { // Search empty ID number.
						if (Operable_Tanks [i] == null) {
							Operable_Tanks [i] = topScript;
							topScript.Tank_ID = i; // Change Tank_ID.
							Store_Components (topScript);
							return;
						}
					}
					// "Operable_Tanks" is full.
					topScript.Tank_ID = 0; // Change Tank_ID.
				}
			}
			// Not operable, or "Operable_Tanks" is full.
			Store_Components (topScript);
		}

		void Store_Components (Tank_ID_Control_CS topScript)
		{ // Store MainBody's transform and sctipt ,and AI scripts.
			TankProp tankProp = new TankProp ();
			tankProp.topScript = topScript;
			tankProp.bodyScript = topScript.GetComponentInChildren < MainBody_Setting_CS > ();
			tankProp.bodyTransform = tankProp.bodyScript.transform;
			tankProp.aiScript = tankProp.bodyTransform.GetComponentInChildren < AI_CS > ();
			topScript.Stored_TankProp = tankProp; // Store tankProp in "Tank_ID_Control_CS".
			if (topScript.Relationship == 0) { // Friend
				friendlyTanks.Add (tankProp);
			} else if (topScript.Relationship == 1) { // Hostile
				hostileTanks.Add (tankProp);
			}
		}

		void Update ()
		{
			if (assignCount > Assign_Frequency) {
				assignCount = 0.0f;
				Assign_Target (friendlyTanks, hostileTanks);
				Assign_Target (hostileTanks, friendlyTanks);
			} else {
				assignCount += Time.deltaTime;
			}
			if (Input.anyKeyDown) {
				Key_Check ();
			}
		}

		void Assign_Target (List < TankProp > teamA, List < TankProp > teamB)
		{
			float shortestDist = 10000.0f;
			bool isFound = false;
			int targetIndex = 0;
			for (int i = 0; i < teamA.Count; i++) {
				if (teamA [i].aiScript && teamA [i].aiScript.No_Attack == false && teamA [i].aiScript.Detect_Flag == false) {
					for (int j = 0; j < teamB.Count; j++) {
						if (teamB [j].bodyTransform.root.tag != "Finish") {
							float tempDist = Vector3.Distance (teamA [i].bodyTransform.position, teamB [j].bodyTransform.position);
							if (tempDist < teamA [i].aiScript.Visibility_Radius && tempDist < shortestDist) {
								if (teamA [i].aiScript.RayCast_Check (teamB [j])) {
									shortestDist = tempDist;
									isFound = true;
									targetIndex = j;
								}
							}
						}
					}
					if (isFound) {
						teamA [i].aiScript.Set_Target (teamB [targetIndex]);
					}
				}
			}
		}

		void Key_Check ()
		{
			if (Input.GetKeyDown (KeyCode.Keypad1) || Input.GetKeyDown ("1")) {
				Cast_Current_ID (1);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad2) || Input.GetKeyDown ("2")) {
				Cast_Current_ID (2);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad3) || Input.GetKeyDown ("3")) {
				Cast_Current_ID (3);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad4) || Input.GetKeyDown ("4")) {
				Cast_Current_ID (4);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad5) || Input.GetKeyDown ("5")) {
				Cast_Current_ID (5);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad6) || Input.GetKeyDown ("6")) {
				Cast_Current_ID (6);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad7) || Input.GetKeyDown ("7")) {
				Cast_Current_ID (7);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad8) || Input.GetKeyDown ("8")) {
				Cast_Current_ID (8);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad9) || Input.GetKeyDown ("9")) {
				Cast_Current_ID (9);
				return;
			} else if (Input.GetKeyDown (KeyCode.Keypad0) || Input.GetKeyDown ("0")) {
				Cast_Current_ID (10);
				return;
			} else if (Input.GetKeyDown (KeyCode.KeypadPlus)) {
				Set_TimeScale (0.5f); // Time peed up.
				return;
			} else if (Input.GetKeyDown (KeyCode.KeypadMinus)) {
				Set_TimeScale (-0.5f); // Time speed down.
				return;
			} else if (Input.GetKeyDown (KeyCode.KeypadEnter)) {
				Set_TimeScale (0.0f); // Time speed reset.
				return;
			} else if (Input.GetKeyDown (KeyCode.Backspace)) {
				// Reload the scene.
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}
		}

		void Cast_Current_ID (int tempID)
		{
			if (tempID < Operable_Tanks.Length) { // To avoid overflowing.
				if (Operable_Tanks [tempID]) {
					currentID = tempID;
					// Broadcast current ID to all tanks.
					for (int i = 0; i < Operable_Tanks.Length; i++) {
						if (Operable_Tanks [i]) {
							Operable_Tanks [i].BroadcastMessage ("Receive_Current_ID", currentID, SendMessageOptions.DontRequireReceiver);
						}
					}
					// Send current ID to RC_Camera.
					if (RC_Cam_Script) {
						RC_Cam_Script.Receive_Current_ID (currentID);
					}
				}
			}
		}

		void Set_TimeScale (float value)
		{
			if (value == 0.0f) {
				currentTimeScale = Time_Scale;
			} else {
				currentTimeScale += value;
				currentTimeScale = Mathf.Clamp (currentTimeScale, 0.0f, 10.0f);
			}
			Time.timeScale = currentTimeScale;
		}

		public void ReSpawn_ReSetting (TankProp storedTankProp)
		{ // Called from "Tank_ID_Control" when ReSpawn, also called from "Remove_Tank()" in this script.
			// Store components again.
			TankProp tankProp = new TankProp ();
			if (storedTankProp.topScript.Relationship == 0) { // friend
				tankProp = friendlyTanks.Find (delegate ( TankProp tempTankProp) {
					return tempTankProp == storedTankProp;
				});
			} else { // hostile
				tankProp = hostileTanks.Find (delegate ( TankProp tempTankProp) {
					return tempTankProp == storedTankProp;
				});
			}
			tankProp.bodyScript = tankProp.topScript.GetComponentInChildren < MainBody_Setting_CS > ();
			tankProp.bodyTransform = tankProp.bodyScript.transform;
			tankProp.aiScript = tankProp.bodyTransform.GetComponentInChildren < AI_CS > ();
			tankProp.topScript.Stored_TankProp = tankProp; // Store tankProp (pass by reference) in "Tank_ID_Control_CS".
			// Reset assignCount.
			assignCount = Assign_Frequency;
		}

		public bool Remove_Tank (TankProp storedTankProp)
		{ // Called from "Tank_ID_Control".
			// Remove reference in the array.
			int tankID = storedTankProp.topScript.Tank_ID;
			if (tankID != 0) { // Operable Tanks.
				if (Operable_Tanks [tankID]) {
					Operable_Tanks [tankID] = null;
				}
			}
			// Remove components in the List.
			if (storedTankProp.topScript.Relationship == 0) { // friend
				friendlyTanks.Remove (storedTankProp);
			} else { // hostile
				hostileTanks.Remove (storedTankProp);
			}
			// Change the current ID when the tank is current operable tank.
			if (tankID == currentID) {
				for (int i = 1; i < Operable_Tanks.Length; i++) {
					if (Operable_Tanks [i]) {
						Cast_Current_ID (i);
						return true;
					}
				}
				return false;
			}
			return true;
		}

	}

}