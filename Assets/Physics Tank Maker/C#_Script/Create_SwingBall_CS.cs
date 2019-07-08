using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ ExecuteInEditMode]
	public class Create_SwingBall_CS : MonoBehaviour
	{
	
		public float Distance = 2.7f;
		public int Num = 1;
		public float Spacing = 1.7f;
		public float Mass = 10.0f;
		public bool Gravity = false;
		public float Radius = 0.1f;
		public float Range = 0.3f;
		public float Spring = 500.0f;
		public float Damper = 100.0f;
		public int Layer = 0;
		public PhysicMaterial Collider_Material;

		void Awake ()
		{
			if (Application.isPlaying) {
				Destroy (this);
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