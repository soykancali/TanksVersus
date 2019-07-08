using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{

	public class Trigger_Collider_CS : MonoBehaviour
	{
	
		List < Event_Controller_CS > eventScripts = new List < Event_Controller_CS > ();

		void Awake ()
		{
			this.gameObject.layer = 2; // Ignore Raycast.
			// Set 'isTrigger' of all the colliders.
			Collider[] colliders = GetComponents < Collider > ();
			for (int i = 0; i < colliders.Length; i++) {
				colliders [i].isTrigger = true;
			}
			// Make the mesh invisible.
			MeshRenderer meshRenderer = GetComponent < MeshRenderer > ();
			if (meshRenderer) {
				meshRenderer.enabled = false;
			}
		}

		void OnTriggerEnter (Collider collider)
		{
			if (collider.gameObject.layer == 11) {
				for (int i = 0; i < eventScripts.Count; i++) {
					if (eventScripts [i]) {
						eventScripts [i].Detect_Collider (collider.transform.root);
					}
				}
			}
		}

		public void Get_Event_Controller (Event_Controller_CS eventScript)
		{
			eventScripts.Add (eventScript);
		}

	}

}