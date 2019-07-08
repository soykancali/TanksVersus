using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Track_Scroll_CS : MonoBehaviour
	{

		[ Header ("Scroll Animation settings")]
		[ Tooltip ("Reference wheel.")] public Transform Reference_Wheel; // Referred to from "Static_Wheel_CS".
		[ Tooltip ("Scroll Rate for X axis.")] public float Scroll_Rate = 0.0005f;
		[ Tooltip ("Texture Name in the shader.")] public string Tex_Name = "_MainTex";

		Material thisMaterial;
		int direction ;
		float previousAng;
		float offsetX;
		MainBody_Setting_CS mainScript;

		void Start ()
		{
			thisMaterial = GetComponent < Renderer > ().material;
			if (Reference_Wheel == null) {
				Debug.LogWarning ("Reference Wheel is not assigned in " + this.name);
				Destroy (this);
			}
			// Set direction.
			if (Reference_Wheel.localPosition.y > 0.0f) { // Left
				direction = 0;
			} else { // Right
				direction = 1;
			}
			// Send this script to all the "Static_Wheels" and "Sound_Control".
			transform.parent.BroadcastMessage ("Get_Track_Scroll", this, SendMessageOptions.DontRequireReceiver);
		}

		void Update ()
		{
			if (mainScript.Visible_Flag) { // MainBody is visible by any camera.
				float currentAng = Reference_Wheel.localEulerAngles.y;
				float deltaAng = Mathf.DeltaAngle (currentAng, previousAng);
				offsetX += Scroll_Rate * deltaAng;
				thisMaterial.SetTextureOffset (Tex_Name, new Vector2 (offsetX, 0.0f));
				previousAng = currentAng;
			}
		}

		void TrackBroken_Linkage (int tempDirection)
		{ // Called from "Damage_Control_CS" in Track_Collider.
			if (tempDirection == direction) {
				Destroy (gameObject);
			}
		}

		void Get_MainScript (MainBody_Setting_CS tempScript)
		{ // Called from "MainBody_Setting_CS".
			mainScript = tempScript;
		}

	}

}