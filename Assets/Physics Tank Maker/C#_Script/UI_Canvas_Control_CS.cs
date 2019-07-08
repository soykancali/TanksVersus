using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class UI_Canvas_Control_CS : MonoBehaviour
	{

		Canvas thisCanvas;
		bool isEnabled;

		void Start ()
		{
			thisCanvas = GetComponent < Canvas > ();
			isEnabled = thisCanvas.enabled;
		}

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.Delete)) {
				if (isEnabled) {
					isEnabled = false;
				} else {
					isEnabled = true;
				}
				thisCanvas.enabled = isEnabled;
			}
		}
	}

}