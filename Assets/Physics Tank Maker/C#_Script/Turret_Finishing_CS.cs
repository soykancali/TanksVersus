using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Turret_Finishing_CS : MonoBehaviour
	{

		public bool Child_Flag = false;
		public Transform Parent_Transform;
	
		Transform turretBase;
		Transform cannonBase;
		Transform barrelBase;

		Transform thisTransform;

		void Start ()
		{
			thisTransform = transform;
			// Count the number of barrels.
			int barrelCount = 0;
			for (int i = 0; i < thisTransform.childCount; i++) {
				if (thisTransform.GetChild (i).name.Substring (0, 11) == "Barrel_Base") {
					barrelCount += 1;
				}
			}
			if (barrelCount == 1) { // Single Barrel
				Single_Barrel ();
			} else { // Multiple Barrels
				Multiple_Barrels ();
			}
		}

		void Single_Barrel ()
		{
			turretBase = thisTransform.Find ("Turret_Base");
			cannonBase = thisTransform.Find ("Cannon_Base");
			barrelBase = thisTransform.Find ("Barrel_Base");
			if (turretBase && cannonBase && barrelBase) {
				// Change the hierarchy.
				barrelBase.parent = cannonBase;
				cannonBase.parent = turretBase;
				Finishing ();
			} else {
				Error_Message ();
			}
		}

		void Multiple_Barrels ()
		{
			turretBase = thisTransform.Find ("Turret_Base");
			if (turretBase) {
				for (int i = 1; i <= thisTransform.childCount; i++) {
					cannonBase = thisTransform.Find ("Cannon_Base_" + i);
					if (cannonBase) {
						cannonBase.parent = turretBase;
						for (int j = 1; j <= thisTransform.childCount; j++) {
							barrelBase = thisTransform.Find ("Barrel_Base_" + i);
							if (barrelBase) {
								barrelBase.parent = cannonBase;
							}
						}
					}
				}
			} else { // Turret_Base cannot be found.
				Error_Message ();
			}
			// Check the new hierarchy.
			if (thisTransform.childCount > 1) {
				Error_Message ();
			}
			Finishing ();
		}

		void Finishing ()
		{
			// Send message to all the turret parts.
			BroadcastMessage ("Complete_Turret", SendMessageOptions.DontRequireReceiver);
			if (Child_Flag) { // Child turret.
				// Send message to "Damage_Control" in the Turret. 
				turretBase.BroadcastMessage ("Set_ChildTurret", SendMessageOptions.DontRequireReceiver);
			} else {
				Destroy (this);
			}
		}

		void Update ()
		{ // Only for Child Turret.
			if (Child_Flag) {
				if (Parent_Transform) {
					thisTransform.parent = Parent_Transform.Find ("Turret_Base"); // Change this parent.
				} else {
					Debug.LogError ("'Parent_Transform' for the Child Turret is not assigned in " + thisTransform.root.name);
				}
			}
			Destroy (this);
		}

		void Error_Message ()
		{
			Debug.LogError ("'Turret_Finishing_CS(Script)' could not change the hierarchy of the Turret. " + thisTransform.root.name);
			Debug.LogWarning ("Make sure the names of 'Turret_Base', 'Cannon_Base' and 'Barrel_Base'.");
			Destroy (this);
		}
	
	}

}