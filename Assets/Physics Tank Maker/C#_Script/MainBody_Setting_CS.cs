using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(MeshFilter))]
	[ RequireComponent (typeof(MeshRenderer))]
	[ RequireComponent (typeof(Rigidbody))]
	public class MainBody_Setting_CS : MonoBehaviour
	{

		public float Body_Mass = 2000.0f;
		public Mesh Body_Mesh;

		public int Materials_Num = 1;
		public Material[] Materials;
		public Material Body_Material;

		public Mesh Collider_Mesh;
		public Mesh Sub_Collider_Mesh;
		public float Durability = 150000.0f;
		public int Turret_Number = 1;

		public int SIC = 14;
		public bool Soft_Landing_Flag;
		public float Landing_Drag = 20.0f;
		public float Landing_Time = 1.5f;

		public float AI_Upper_Offset = 1.5f;
		public float AI_Lower_Offset = 0.3f;

		// Referred to from "Static_Track", "Static_Wheel" and "Track_Interpolation".
		public bool Visible_Flag;


		void Awake ()
		{
			// Set layer collision matrix.
			Layer_Collision_Settings ();
			// Set Solver Iteration Count.
			Rigidbody thisRigidbody = GetComponent < Rigidbody > ();
			thisRigidbody.solverIterations = SIC;
			// Attach NavMeshObstacle for Bunker Tank.
			if (thisRigidbody.isKinematic) { // Bunker Tank
				NavMeshObstacle navMeshObstacle = gameObject.AddComponent < NavMeshObstacle > ();
				navMeshObstacle.carvingMoveThreshold = 1.0f;
				navMeshObstacle.carving = true;
			}
			// Send this reference to all the "Static_Track_CS", "Static_Wheel_CS", "Track_Deform_CS", "Track_Scroll_CS".
			BroadcastMessage ("Get_MainScript", this, SendMessageOptions.DontRequireReceiver); // Must be called in "Awake()".
			// Soft Landing.
			if (Soft_Landing_Flag) {
				StartCoroutine ("Soft_Landing");
			}
		}

		void Layer_Collision_Settings ()
		{
			// Layer Collision Settings.
			// Layer8 >> for CrossHair Image. (for old system using GUI_Texture)
			// Layer9 >> for wheels.
			// Layer10 >> for Suspensions and Track Reinforce.
			// Layer11 >> for MainBody.
			for (int i = 0; i <= 11; i++) {
				Physics.IgnoreLayerCollision (9, i, false);
				Physics.IgnoreLayerCollision (11, i, false);
			}
			Physics.IgnoreLayerCollision (9, 9, true); // Wheels ignore each other.
			Physics.IgnoreLayerCollision (9, 11, true); // Wheels ignore MainBody.
			for (int i = 0; i <= 11; i++) {
				Physics.IgnoreLayerCollision (10, i, true); // Suspensions and Track Reinforce are ignore all.
			}
		}

		IEnumerator Soft_Landing ()
		{
			Rigidbody thisRigidbody = GetComponent < Rigidbody > ();
			float defaultDrag = thisRigidbody.drag;
			thisRigidbody.drag = Landing_Drag;
			thisRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			yield return new WaitForSeconds (Landing_Time);
			thisRigidbody.drag = defaultDrag;
			thisRigidbody.constraints = RigidbodyConstraints.None;
		}


		void OnBecameVisible ()
		{
			Visible_Flag = true; // Referred to from "Static_Track", "Static_Wheel".
		}

		void OnBecameInvisible ()
		{
			Visible_Flag = false; // Referred to from "Static_Track", "Static_Wheel".
		}

		#if UNITY_5_2
		// for bugs in Unity 5.2
		void OnTriggerStay ()
		{
		}
		#endif

	}

}