using FistVR;
using UnityEngine;

namespace FoldableMP7A1
{
	public class Hooks
	{
		public Hooks()
		{
			Hook();
		}

		public void Dispose()
		{
			Unhook();
		}

		private void Hook()
		{
			On.FistVR.ClosedBoltWeapon.Awake += ClosedBoltWeapon_Awake;
		}

		private void ClosedBoltWeapon_Awake(On.FistVR.ClosedBoltWeapon.orig_Awake orig, FistVR.ClosedBoltWeapon self)
		{
			orig(self);

			if (self.ObjectWrapper.ItemID == "Mp7a1")
			{
				AddFoldingHandle(self);
			}
		}

		private void AddFoldingHandle(ClosedBoltWeapon weapon)
		{
			var handlePhys = weapon.transform.Find("Phys/Phys_Cube (12)");
			var geo = weapon.transform.Find("Geo");
			var flipper = geo.Find("FlipSightTrigger_Front");
			var handle = geo.Find("Handle");

			var handleFlipper = GameObject.Instantiate(flipper, geo);
			handleFlipper.transform.localPosition = new Vector3(0, 0, 0.1f);

			var hflipper = handleFlipper.gameObject.GetComponent<AR15HandleSightFlipper>();
			hflipper.name = "HandleFlipper";
			hflipper.Flipsight = handle;

			weapon.Foregrip.transform.parent = handle;
			handlePhys.parent = handle;
		}

		private void Unhook()
		{
			On.FistVR.ClosedBoltWeapon.Awake -= ClosedBoltWeapon_Awake;
		}
	}
}
