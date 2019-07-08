using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ ExecuteInEditMode]
	public class Create_TrackBelt_CS : MonoBehaviour
	{
		public bool Rear_Flag = false;
		public int SelectedAngle = 3600;
		public int Angle_Rear = 4500;
		public int Number_Straight = 17;
		public float Spacing = 0.3f;
		public float Distance = 2.7f;
		public float Track_Mass = 30.0f;
		public Bounds Collider_Info = new Bounds (new Vector3 (0.0f, -0.016f, 0.0f), new Vector3 (0.65f, 0.08f, 0.3f));
		public PhysicMaterial Collider_Material;
		public Mesh Track_R_Mesh;
		public Mesh Track_L_Mesh;
		public Material Track_R_Material;
		public Material Track_L_Material;

		public int SubJoint_Type = 1;
		public float Reinforce_Radius = 0.3f;

		public float Special_Offset;

		public float Track_Durability = 55000.0f;
		public float BreakForce = 5000.0f;

		public bool RealTime_Flag = true;
		public bool Static_Flag = false;
		public bool Prefab_Flag = false;


		void Awake ()
		{
			if (Application.isPlaying) {
				if (Static_Flag) { // For creating Static_Track.
					Rigidbody parentRigidbody = transform.parent.GetComponent < Rigidbody > ();
					parentRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
					parentRigidbody.drag = 15.0f;
				} else {
					Destroy (this);
				}
			}
		}

		void Update ()
		{
			if (transform.localEulerAngles.z != 90.0f) {
				Vector3 ang;
				ang.x = transform.localEulerAngles.x;
				ang.y = transform.localEulerAngles.y;
				ang.z = 90.0f;
				transform.localEulerAngles = ang;
			}
		}

	}

}