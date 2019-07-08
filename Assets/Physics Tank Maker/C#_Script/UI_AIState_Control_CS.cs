using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	public class UI_AIState_Control_CS : MonoBehaviour
	{

		public Color Color_Attack = Color.red;
		public Color Color_Lost = Color.magenta;
		public Color Color_Dead = Color.black;

		AI_CS aiScript;
		Color defaultColor;
		Text thisText;

		void Awake ()
		{
			thisText = GetComponent < Text > ();
			defaultColor = thisText.color;
		}

		void LateUpdate ()
		{
			if (aiScript) {
				if (aiScript.Action_Type == 0) { // Patrol mode.
					thisText.text = aiScript.Tank_Name + " = Search";
					thisText.color = defaultColor;
				} else { // Chase mode.
					if (aiScript.Detect_Flag) {
						thisText.text = aiScript.Tank_Name + " = Attack";
						thisText.color = Color_Attack;
					} else {
						thisText.text = aiScript.Tank_Name + " = Lost : " + Mathf.CeilToInt (aiScript.Losing_Count);
						thisText.color = Color_Lost;
					}
				}
			}
		}

		public void Get_AI (AI_CS script)
		{ // Called from "AI_CS".
			aiScript = script;
		}

		void Dead ()
		{ // Called from "AI_CS" when the tank is destroyed.
			thisText.text = aiScript.Tank_Name + " = Dead";
			thisText.color = Color_Dead;
		}

	}

}