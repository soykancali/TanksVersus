using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Recoil_Brake_CS : MonoBehaviour
	{
	
		public float Recoil_Time = 0.25f;
		public float Return_Time = 0.45f;
		public float Recoil_Length = 0.2f;

		int barrelType = 0; // Set by "Barrel_Base".

		bool isReady = true;
		Transform thisTransform;
		Vector3 initialPos;
		const float HALF_PI = Mathf.PI * 0.5f;

		void Complete_Turret ()
		{ // Called from 'Turret_Finishing".
			thisTransform = transform;
			initialPos = thisTransform.localPosition;
		}

		void Fire_Linkage (int direction)
		{
			if (isReady && (barrelType == 0 || barrelType == direction)) {
				isReady = false;
				StartCoroutine ("Recoil_Brake");
			}
		}

		IEnumerator Recoil_Brake ()
		{
			// Move backward.
			float tempTime = 0.0f;
			while (tempTime < Recoil_Time) {
				float rate = Mathf.Sin (HALF_PI * (tempTime / Recoil_Time));
				thisTransform.localPosition = new Vector3 (initialPos.x, initialPos.y, initialPos.z - (rate * Recoil_Length));
				tempTime += Time.deltaTime;
				yield return null;
			}
			// Return to the initial position.
			tempTime = 0.0f;
			while (tempTime < Return_Time) {
				float rate = Mathf.Sin (HALF_PI * (tempTime / Return_Time) + HALF_PI);
				thisTransform.localPosition = new Vector3 (initialPos.x, initialPos.y, initialPos.z - (rate * Recoil_Length));
				tempTime += Time.deltaTime;
				yield return null;
			}
			isReady = true;
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			Destroy (this);
		}

		void Set_Barrel_Type (int type)
		{ // Called from "Barrel_Base_CS".
			barrelType = type;
		}

	}

}