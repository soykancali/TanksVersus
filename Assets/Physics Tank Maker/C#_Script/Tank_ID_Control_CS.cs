using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	public class Tank_ID_Control_CS: MonoBehaviour
	{
		public int Tank_ID = 1;
		public int Relationship;
		public int ReSpawn_Times = 0;
		public float Attack_Multiplier = 1.0f;
		public int Input_Type = 4;
		public int Turn_Type;
		public string Marker_Name = "Pos_Marker";

		// 'public' is needed for Editor script.
		public bool ReSpawn_Flag;
		public string Prefab_Path;
	
		// AI patrol settings.
		public GameObject WayPoint_Pack;
		public int Patrol_Type = 1; // 0 = Order, 1 = Random.
		public Transform Follow_Target;
		// AI combat settings.
		public bool No_Attack = false;
		public float Visibility_Radius = 512.0f;
		public float Approach_Distance = 256.0f;
		public float OpenFire_Distance = 512.0f;
		public float Lost_Count = 20.0f;
		public bool Face_Enemy = false;
		public float Face_Offest_Angle = 0.0f;
		// AI text settings.
		public Text AI_State_Text;
		public string Tank_Name;
		// AI Auto respawn settings.
		public float ReSpawn_Interval = 10.0f;
		public float Remove_Time = 30.0f;

		Transform thisTransform;
		Game_Controller_CS controllerScript;
		public TankProp Stored_TankProp; // Stored by "Game_Controller_CS". (passed by reference)

		public bool Is_Current; // Referred to from "PosMarker".
		int currentID = 1;

		void Awake ()
		{
			thisTransform = transform;
			// Change the hierarchy.
			if (GetComponentInParent < Event_Controller_CS > () == null) {
				thisTransform.parent = null;
			}
		}

		void Start ()
		{
			// Overwrite values when the tank is spawned by "Event_Controller_CS".
			Event_Controller_CS eventScript = GetComponentInParent < Event_Controller_CS > ();
			if (eventScript) { // The tank is spawned by "Event_Controller_CS".
				Tank_ID = eventScript.Tank_ID;
				Relationship = eventScript.Relationship;
				ReSpawn_Times = eventScript.ReSpawn_Times;
				Attack_Multiplier = eventScript.Attack_Multiplier;
				Input_Type = eventScript.Input_Type;
				Turn_Type = eventScript.Turn_Type;
				if (eventScript.OverWrite_Flag) { // Overwrite AI settings.
					WayPoint_Pack = eventScript.WayPoint_Pack;
					Patrol_Type = eventScript.Patrol_Type;
					Follow_Target = eventScript.Follow_Target;
					No_Attack = eventScript.No_Attack;
					Visibility_Radius = eventScript.Visibility_Radius;
					Approach_Distance = eventScript.Approach_Distance;
					OpenFire_Distance = eventScript.OpenFire_Distance;
					Lost_Count = eventScript.Lost_Count;
					Face_Enemy = eventScript.Face_Enemy;
					Face_Offest_Angle = eventScript.Face_Offest_Angle;
					AI_State_Text = eventScript.AI_State_Text;
					Tank_Name = eventScript.Tank_Name;
					ReSpawn_Interval = eventScript.ReSpawn_Interval;
					Remove_Time = eventScript.Remove_Time;
				}
			}
			// Set the root's tag.
			Set_Tag ();
			// Find "Game_Controller" in the scene.
			GameObject controllerObject = GameObject.FindGameObjectWithTag ("GameController");
			if (controllerObject) {
				controllerScript = controllerObject.GetComponent < Game_Controller_CS > ();
			}
			if (controllerScript) {
				controllerScript.Receive_ID (this); // Send this reference, and get proper Tank_ID.
			} else {
				Debug.LogError ("There is no 'Game_Controller' in the scene.");
			}
			// Find "Pos_Marker" and send message.
			if (string.IsNullOrEmpty (Marker_Name) == false) {
				GameObject markerObject = GameObject.Find (Marker_Name) ;
				if (markerObject) {
					UI_PosMarker_Control_CS markerScript = markerObject.GetComponent < UI_PosMarker_Control_CS > ();
					if (markerScript == null) {
						markerScript = markerObject.AddComponent < UI_PosMarker_Control_CS > ();
					}
					markerScript.Create_PosMarker (Stored_TankProp);
				} else {
					Debug.LogWarning (Marker_Name + " cannot be found in the scene.");
				}
			}
			// Send settings to all the child parts.
			Send_Settings ();
		}

		void Set_Tag ()
		{ // This function is called at the opening, and also called from 'ReSpawn ()'.
			if (Relationship == 0) { // Friendly.
				thisTransform.root.tag = "Player";
			} else { // Hostile.
				thisTransform.root.tag = "Untagged";
			}
		}

		void Send_Settings ()
		{ // This function is called at the opening, and also called from 'ReSpawn ()'.
			// Check the input type.
			if (GetComponentInChildren < AI_CS > ()) { // This tank must be an AI tank.
				Input_Type = 10; // AI Input
			} else if (Input_Type == 10) { // This tank has no AI, but Input_Type is set to "AI".
				Input_Type = 4; // Mouse Input
			}
			// Broadcast the input type.
			BroadcastMessage ("Set_Input_Type", Input_Type, SendMessageOptions.DontRequireReceiver);
			if (Input_Type == 0 || Input_Type == 1 || Input_Type == 5) {
				BroadcastMessage ("Set_Turn_Type", Turn_Type, SendMessageOptions.DontRequireReceiver);
			}
			// Broadcast Tank_ID and Current_ID.
			BroadcastMessage ("Set_Tank_ID", Tank_ID, SendMessageOptions.DontRequireReceiver);
			BroadcastMessage ("Receive_Current_ID", currentID, SendMessageOptions.DontRequireReceiver);
		}

		void Update ()
		{
			if (Is_Current && Input.GetKeyDown (KeyCode.Return)) {
				if (ReSpawn_Flag && ReSpawn_Times > 0) {
					ReSpawn ();
				}
			}
		}

		void ReSpawn ()
		{
			// Make sure that the prefab exists.
			GameObject tempObject = Resources.Load (Prefab_Path) as GameObject;
			if (tempObject == null) {
				ReSpawn_Flag = false;
				return;
			}
			ReSpawn_Times -= 1;
			// This object is continuously used even when a new tank is spawned.
			// Destroy child parts.
			int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}
			if (thisTransform.childCount == 0) { // Destroying succeeded.
				// Set the root's tag again.
				Set_Tag ();
				// Instantiate the prefab.
				GameObject newObject = Instantiate (Resources.Load (Prefab_Path), thisTransform.position, thisTransform.rotation) as GameObject;
				// Change the hierarchy of the new tank.
				childCount = newObject.transform.childCount;
				for (int i = 0; i < childCount; i++) {
					newObject.transform.GetChild (0).parent = thisTransform; // New child objects are moved under this object as its children.
				}
				// Destroy the top object of the new tank.
				DestroyImmediate (newObject);
				// Broadcast settings to the new children.
				Send_Settings ();
				// Reset the stored components in the "Game_Controller".
				controllerScript.ReSpawn_ReSetting (Stored_TankProp);
			}
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			thisTransform.root.tag = "Finish";
			if (Input_Type == 10) { // AI
				if (ReSpawn_Flag && ReSpawn_Times > 0) {
					StartCoroutine ("Auto_ReSpawn");
				} else {
					if (Remove_Time != Mathf.Infinity) {
						StartCoroutine ("Remove_Tank", Remove_Time);
					}
				}
			}
		}

		IEnumerator Auto_ReSpawn ()
		{
			yield return new WaitForSeconds (ReSpawn_Interval);
			ReSpawn ();
		}

		public IEnumerator Remove_Tank (float count)
		{ // Also called from Event_Controller.
			yield return new WaitForSeconds (count);
			// Remove this reference in the "Game_Controller", and check the current ID.
			if (controllerScript.Remove_Tank (Stored_TankProp)) {
				Destroy (gameObject);
			}
		}

		void Receive_Current_ID (int id)
		{
			currentID = id;
			if (id == Tank_ID) {
				Is_Current = true;
			} else {
				Is_Current = false;
			}
		}

	}

}