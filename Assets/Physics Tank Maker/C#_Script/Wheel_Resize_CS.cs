using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Wheel_Resize_CS : MonoBehaviour
	{
	
		public float ScaleDown_Size = 0.5f;
		public float Return_Speed = 0.05f;
	
		bool isSmall;

		void Start ()
		{
			if (ScaleDown_Size <= 1.0f) {
				isSmall = true;
			} else {
				isSmall = false;
			}
			transform.localScale = new Vector3 (ScaleDown_Size, ScaleDown_Size, ScaleDown_Size);
		}

		void Update ()
		{
			transform.localScale = new Vector3 (ScaleDown_Size, ScaleDown_Size, ScaleDown_Size);
			if (isSmall) {
				ScaleDown_Size += Return_Speed;
				if (ScaleDown_Size >= 1.0f) {
					transform.localScale = Vector3.one;
					Destroy (this);
				}
			} else {
				ScaleDown_Size -= Return_Speed;
				if (ScaleDown_Size <= 1.0f) {
					transform.localScale = Vector3.one;
					Destroy (this);
				}
			}
		}

	}

}