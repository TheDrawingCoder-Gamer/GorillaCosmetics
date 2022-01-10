﻿using GorillaCosmetics.Utils;
using GorillaCosmetics.Data;
using UnityEngine;
using System.Reflection;
using Newtonsoft.Json;
using Photon.Pun;
using System.Collections;
using System;

namespace GorillaCosmetics.Data.Behaviours
{
    public class MaterialPreviewButton : GorillaTriggerBox
    {
		public GorillaMaterial material;

		static bool canPress = true;
		private void OnTriggerEnter(Collider collider)
		{
			if (!canPress) return;

			if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null)
			{
				GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
				// do stuff
				if(material != null)
				{
					canPress = false;
					string matName = material.Descriptor?.MaterialName ?? "Default";
					Debug.Log("Swapping to: " + matName);
					AssetLoader.SelectMaterial(matName);
					GorillaCosmetics.selectedMaterial.Value = matName;
					StartCoroutine(ButtonDelay());
					try
					{
						UpdateMaterialValue();
					}
					catch (Exception e)
					{
						Debug.LogWarning("Error selecting mat.");
						Debug.LogWarning(e);
					}
				}
				if (component != null)
				{
					GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
				}
			}
		}

		private void UpdateMaterialValue()
        {
			string material = this.material.Descriptor.MaterialName;

			GorillaTagger gorillaTagger = GorillaTagger.Instance;
			VRRig offlineVRRig = gorillaTagger.offlineVRRig;
			if (offlineVRRig == null) offlineVRRig = gorillaTagger.myVRRig; // this will probably break stuff. TOO BAD!

			string hatCS = offlineVRRig.hat;
			string face = offlineVRRig.face;
			string badge = offlineVRRig.badge;
			VRRigHatJSON hatJSON = new VRRigHatJSON();
			hatJSON.hat = hatCS;
			// I don't know if this is right, but I'm not sure how red is doing it so i'm taking my best guess.
			Debug.Log(hatCS);
            if (hatCS.Contains("}") && hatCS.Contains("{"))
			{
				// it's probably json. I really should implement a better check for this.
				var json = JsonConvert.DeserializeObject<VRRigHatJSON>(hatCS);
				hatJSON.hat = json.hat;
			}

			hatJSON.material = material;
			string hatMessage = JsonConvert.SerializeObject(hatJSON);

			if (offlineVRRig)
            {
				// locally update it
				offlineVRRig.LocalUpdateCosmetics(badge, face, hatMessage);
            }
			VRRig myVRRig = gorillaTagger.myVRRig;
            if (myVRRig)
            {
				PhotonView photonView = myVRRig.photonView;

				photonView.RPC("UpdateCosmetics", RpcTarget.All, new object[] { badge, face, hatMessage });
				PhotonNetwork.SendAllOutgoingCommands();
			}
		}

		private void OnDisable() => canPress = true;
		private void OnDestroy() => canPress = true;

		private static IEnumerator ButtonDelay()
		{
			yield return new WaitForSeconds(2f);
			canPress = true;
		}
	}
}
