using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	public class Event_Controller_CS : MonoBehaviour
	{
		// Trigger settings.
		public int Trigger_Type; // 0=Timer, 1=Destroy, 2=Trigger_Collider
		public float Trigger_Time;
		public int Trigger_Setting_Type = 0; // 0=Set Manually, 1=Any Hostile tank, 2=Any Friendly tank.
		public int Trigger_Num = 1;
		public Transform[] Trigger_Tanks;
		public int Operator_Type; // 0=AND, 1=OR.
		public bool All_Trigger_Flag = true;
		public int Necessary_Num = 1;
		public Trigger_Collider_CS Trigger_Collider_Script;
		public int Useless_Event_Num;
		public Event_Controller_CS[] Useless_Events;
		public int Disabled_Event_Num;
		public Event_Controller_CS[] Disabled_Events;
		public int Event_Type; // 0=Spawn Tank , 1=Show Message , 2=Change AI Settings , 3=Remove Tank , 4=Artillery Fire ,  , 10=None
	
		// Text settings.
		public Text Event_Text;
		UI_Text_Control_CS Text_Script;
		public string Event_Message;
		public Color Event_Message_Color = Color.white;
		public float Event_Message_Time = 3.0f;

		// Tank settings.
		public bool Spawn_Tank_Flag;
		public GameObject Prefab_Object;
		public int Tank_ID = 1;
		public int Relationship;
		public int ReSpawn_Times = 0;
		public float Attack_Multiplier = 1.0f;
		public int Input_Type = 4;
		public int Turn_Type = 0;

		// AI settings.
		public bool OverWrite_Flag = true;
		// AI patrol settings.
		public GameObject WayPoint_Pack;
		public int Patrol_Type = 1;
		// 0 = Order, 1 = Random.
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
		// AI respawn settings.
		public float ReSpawn_Interval = 10.0f;
		public float Remove_Time = 30.0f;

		// New AI settings & Remove settings.
		public bool Trigger_Itself_Flag = true;
		public int Target_Num = 1;
		public Transform[] Target_Tanks;
		// New AI settings.
		public GameObject New_WayPoint_Pack;
		public int New_Patrol_Type = 1;
		public Transform New_Follow_Target;
		public bool New_No_Attack = false;
		public float New_Visibility_Radius = 512.0f;
		public float New_Approach_Distance = 256.0f;
		public float New_OpenFire_Distance = 512.0f;
		public float New_Lost_Count = 20.0f;
		public bool New_Face_Enemy = false;
		public float New_Face_Offest_Angle = 0.0f;
		public bool Renew_ReSpawn_Times_Flag = false;
		public int New_ReSpawn_Times;

		// Artillery Fire settings.
		public Artillery_Fire_CS Artillery_Script;
		public Transform Artillery_Target;
		public int Artillery_Num;

		bool isPrepared;
		float currentTime;

		void Awake ()
		{
			// Change the hierarchy.
			transform.parent = null;
		}

		void Start ()
		{
			// Check and set the trigger components.
			switch (Trigger_Type) {
			case 0: // Timer
				isPrepared = true;
				break;
			case 1: // Destroy
				isPrepared = Set_Transforms (Trigger_Tanks);
				break;
			case 2: // Trigger_Collider
				if (Trigger_Collider_Script) {
					// Send reference.
					Trigger_Collider_Script.Get_Event_Controller (this);
					if (Trigger_Setting_Type == 0) { // 'Set manually'
						isPrepared = Set_Transforms (Trigger_Tanks);
					} else { // 'Any Hostile tank' or 'Any Friendly tank'
						isPrepared = true;
					}
				} else {
					isPrepared = false;
				}
				break;
			}
			// Check and set the event components.
			switch (Event_Type) {
			case 0: // Spawn Tank
				if (Prefab_Object == null) {
					isPrepared = false;
				}
				break;
			case 1: // Show Message
				if (Event_Text == null) {
					isPrepared = false;
				} else {
					Text_Script = Event_Text.GetComponent < UI_Text_Control_CS > ();
					if (Text_Script == null) {
						Text_Script = Event_Text.gameObject.AddComponent < UI_Text_Control_CS > ();
					}
			
				}
				break;
			case 2: // Change AI Settings
				if (Trigger_Itself_Flag == false) {
					isPrepared = Set_Transforms (Target_Tanks);
				}
				break;
			case 3: // Remove Tank
				if (Trigger_Itself_Flag == false) {
					isPrepared = Set_Transforms (Target_Tanks);
				}
				break;
			case 4: // Artillery Fire
				if (Artillery_Script == null || Artillery_Target == null) {
					isPrepared = false;
				}
				break;
			case 10: // None
				break;
			}
		}

		bool Set_Transforms (Transform[] transforms)
		{
			int count = 0;
			for (int i = 0; i < transforms.Length; i++) {
				if (transforms [i]) {
					count += 1;
					transforms [i] = transforms [i].root;
				}
			}
			if (count == 0) {
				return false;
			} else {
				return true;
			}
		}

		void Update ()
		{
			if (isPrepared) {
				switch (Trigger_Type) {
				case 0: // Timer
					Timer ();
					break;
				case 1: // Destroy
					switch (Operator_Type) {
					case 0: // AND
						Check_Destroy_AND ();
						break;
					case 1: // OR
						Check_Destroy_OR ();
						break;
					}
					break;
				}
			}
		}

		void Timer ()
		{
			currentTime += Time.deltaTime;
			if (currentTime >= Trigger_Time) {
				currentTime = 0.0f;
				isPrepared = false;
				Start_Event ();
			}
		}

		void Check_Destroy_AND ()
		{
			// Check Tags.
			if (All_Trigger_Flag) {
				for (int i = 0; i < Trigger_Tanks.Length; i++) {
					if (Trigger_Tanks [i] && Trigger_Tanks [i].tag != "Finish") {
						return;
					}
				}
			} else {
				int currentNum = 0;
				for (int i = 0; i < Trigger_Tanks.Length; i++) {
					if (Trigger_Tanks [i] && Trigger_Tanks [i].tag == "Finish") {
						currentNum += 1;
					}
				}
				if (currentNum < Necessary_Num) {
					return;
				}
			}
			// Check the remaining Auto ReSpawn Times. 
			for (int i = 0; i < Trigger_Tanks.Length; i++) {
				if (Trigger_Tanks [i] && Trigger_Tanks [i].tag == "Finish") {
					Tank_ID_Control_CS topScript = Trigger_Tanks [i].GetComponentInChildren < Tank_ID_Control_CS > ();
					if (topScript && topScript.ReSpawn_Times > 0) {
						return;
					}
				}
			}
			// All the Auto ReSpawn Times are zero.
			isPrepared = false;
			Start_Event ();
		}

		void Check_Destroy_OR ()
		{
			// Check Tags and remaining Auto ReSpawn Times. 
			for (int i = 0; i < Trigger_Tanks.Length; i++) {
				if (Trigger_Tanks [i] && Trigger_Tanks [i].tag == "Finish") {
					Tank_ID_Control_CS topScript = Trigger_Tanks [i].GetComponentInChildren < Tank_ID_Control_CS > ();
					if (topScript && topScript.ReSpawn_Times == 0) {
						isPrepared = false;
						Start_Event ();
						break;
					}
				}
			}
		}

		public void Detect_Collider (Transform rootTransform)
		{ // Called from "Trigger_Collider_CS".
			if (isPrepared) {
				switch (Trigger_Setting_Type) { 
				case 0: // 'Set manually'
					switch (Operator_Type) {
					case 0: // AND
						Check_Collider_AND (rootTransform);
						break;
					case 1: // OR
						Check_Collider_OR (rootTransform);
						break;
					}
					break;
				case 1: // 'Any hostile tank'
					Check_Collider_AnyHostile (rootTransform);
					break;
				case 2: // 'Any friend tank'
					Check_Collider_AnyFriendly (rootTransform);
					break;
				}
			}
		}

		void Check_Collider_AND (Transform rootTransform)
		{
			for (int i = 0; i < Trigger_Tanks.Length; i++) {
				if (Trigger_Tanks [i] && rootTransform == Trigger_Tanks [i]) {
					Trigger_Tanks [i] = null;
					// Check the remaining trigger.
					if (All_Trigger_Flag) {
						for (int j = 0; j < Trigger_Tanks.Length; j++) {
							if (Trigger_Tanks [j]) {
								return;
							}
						}
					} else {
						int currentNum = 0;
						for (int j = 0; j < Trigger_Tanks.Length; j++) {
							if (Trigger_Tanks [j] == null) {
								currentNum += 1;
							}
						}
						if (currentNum < Necessary_Num) {
							return;
						}
					}
					// All the necessary Trigger_Tanks are Null.
					isPrepared = false;
					Start_Event ();
					break;
				}
			}
		}

		void Check_Collider_OR (Transform rootTransform)
		{
			for (int i = 0; i < Trigger_Tanks.Length; i++) {
				if (Trigger_Tanks [i] && rootTransform == Trigger_Tanks [i]) {
					isPrepared = false;
					if ((Event_Type == 2 || Event_Type == 3) && Trigger_Itself_Flag) { // "Change AI Settings" or "Remove Tank" && Target is Trigger itself.
						Target_Tanks [0] = Trigger_Tanks [i];
						Trigger_Tanks [i] = null;
						// Check the remaining trigger.
						for (int j = 0; j < Trigger_Tanks.Length; j++) {
							if (Trigger_Tanks [j]) {
								isPrepared = true;
								break;
							}
						}
					}
					Start_Event ();
					break;
				}
			}
		}

		void Check_Collider_AnyHostile (Transform rootTransform)
		{
			if (rootTransform.tag != "Player") { // Hostile
				if ((Event_Type == 2 || Event_Type == 3) && Trigger_Itself_Flag) { // "Change AI Settings" or "Remove Tank" && Target is Trigger itself.
					Target_Tanks [0] = rootTransform;
				}
				Start_Event ();
			}
		}

		void Check_Collider_AnyFriendly (Transform rootTransform)
		{
			if (rootTransform.tag == "Player") { // Friendly
				if ((Event_Type == 2 || Event_Type == 3) && Trigger_Itself_Flag) { // "Change AI Settings" or "Remove Tank" && Target is Trigger itself.
					Target_Tanks [0] = rootTransform;
				}
				Start_Event ();
			}
		}

		void Start_Event ()
		{
			// Control other triggers.
			Destroy_Useless_Events ();
			Enable_Disabled_Events ();
			// Start Event.
			switch (Event_Type) {
			case 0: // Spawn Tank
				Spawn_Tank ();
				break;
			case 1: // Show Message
				Show_Message ();
				break;
			case 2: // Change AI Settings
				Change_AI_Settings ();
				break;
			case 3: // Remove Tank
				Remove_Tank ();
				break;
			case 4: // Artillery Fire
				Artillery_Fire ();
				break;
			case 10: // None
				Destroy (this.gameObject);
				break;
			}
		}

		void Destroy_Useless_Events ()
		{
			for (int i = 0; i < Useless_Events.Length; i++) {
				if (Useless_Events [i]) {
					Destroy (Useless_Events [i].gameObject);
					Useless_Events [i] = null;
				}
			}
		}

		void Enable_Disabled_Events ()
		{
			for (int i = 0; i < Disabled_Events.Length; i++) {
				if (Disabled_Events [i]) {
					Disabled_Events [i].enabled = true;
					Disabled_Events [i] = null;
				}
			}
		}

		void Spawn_Tank ()
		{
			GameObject gameObject = Instantiate (Prefab_Object, transform.position, transform.rotation) as GameObject;
			gameObject.transform.parent = this.transform;
			Destroy (this);
		}

		void Show_Message ()
		{
			Text_Script.Receive_Text (Event_Message, Event_Message_Color, Event_Message_Time);
			Destroy (this.gameObject);
		}

		void Change_AI_Settings ()
		{
			for (int i = 0; i < Target_Tanks.Length; i++) {
				// for AI tank in standby.
				Event_Controller_CS eventScript = Target_Tanks [i].GetComponentInChildren < Event_Controller_CS > ();
				if (eventScript) {
					eventScript.WayPoint_Pack = New_WayPoint_Pack;
					eventScript.Patrol_Type = New_Patrol_Type;
					eventScript.Follow_Target = New_Follow_Target;
					eventScript.No_Attack = New_No_Attack;
					eventScript.Visibility_Radius = New_Visibility_Radius;
					eventScript.Approach_Distance = New_Approach_Distance;
					eventScript.OpenFire_Distance = New_OpenFire_Distance;
					eventScript.Lost_Count = New_Lost_Count;
					eventScript.Face_Enemy = New_Face_Enemy;
					eventScript.Face_Offest_Angle = New_Face_Offest_Angle;
				}
				// for AI tank in action.
				AI_CS aiScript = Target_Tanks [i].GetComponentInChildren < AI_CS > ();
				if (aiScript) {
					aiScript.Change_AI_Settings (this);
				}
				// for Respawn.
				Tank_ID_Control_CS topScript = Target_Tanks [i].GetComponentInChildren < Tank_ID_Control_CS > ();
				if (topScript) {
					topScript.WayPoint_Pack = New_WayPoint_Pack;
					topScript.Patrol_Type = New_Patrol_Type;
					topScript.Follow_Target = New_Follow_Target;
					topScript.No_Attack = New_No_Attack;
					topScript.Visibility_Radius = New_Visibility_Radius;
					topScript.Approach_Distance = New_Approach_Distance;
					topScript.OpenFire_Distance = New_OpenFire_Distance;
					topScript.Lost_Count = New_Lost_Count;
					topScript.Face_Enemy = New_Face_Enemy;
					topScript.Face_Offest_Angle = New_Face_Offest_Angle;
					if (Renew_ReSpawn_Times_Flag) {
						topScript.ReSpawn_Times = New_ReSpawn_Times;
					}
				}
			}
			if (isPrepared == false) {
				Destroy (this.gameObject);
			}
		}

		void Remove_Tank ()
		{
			for (int i = 0; i < Target_Tanks.Length; i++) {
				// Delete Event_Controller_CS script.
				Event_Controller_CS eventScript = Target_Tanks [i].GetComponentInChildren < Event_Controller_CS > ();
				if (eventScript) {
					Destroy (eventScript);
				}
				// Call "Remove_Tank()" in the Tank_ID_Control_CS script.
				Tank_ID_Control_CS topScript = Target_Tanks [i].GetComponentInChildren < Tank_ID_Control_CS > ();
				if (topScript) {
					topScript.StartCoroutine ("Remove_Tank", 0.0f);
				}
			}
			if (isPrepared == false) {
				Destroy (this.gameObject);
			}
		}

		void Artillery_Fire ()
		{
			if (Artillery_Script) {
				Artillery_Script.Fire (Artillery_Target, Artillery_Num);
			}
			Destroy (this.gameObject);
		}

	}

}