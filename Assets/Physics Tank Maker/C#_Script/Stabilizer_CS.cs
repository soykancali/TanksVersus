using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Stabilizer_CS : MonoBehaviour
	{
	
		Transform thisTransform;
		float initialPosY;
		Vector3 angles;
		int direction ;

		void Start ()
		{
			thisTransform = transform;
			initialPosY = thisTransform.localPosition.y;
			angles = thisTransform.localEulerAngles;
			// Set direction.
			if ( initialPosY > 0.0f ) { // Left
				direction = 0 ;
			} else { // Right
				direction = 1 ;
			}
		}

		void Update ()
		{
			// Stabilize position.
			Vector3 currentPos = thisTransform.localPosition;
			currentPos.y = initialPosY;
			thisTransform.localPosition = currentPos;
			// Stabilize angle.
			angles.y = thisTransform.localEulerAngles.y;
			thisTransform.localEulerAngles = angles;
		}

		void TrackBroken_Linkage (int tempDirection)
		{ // Called from "Damage_Control_CS" in Physics_Track piece, Track_Collider.
			if (tempDirection == direction) {
				// Remove this script.
				Destroy (this);
			}
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			// Remove this script.
			Destroy (this);
		}

	}

}