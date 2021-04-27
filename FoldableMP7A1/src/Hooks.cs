using FistVR;
using System;
using UnityEngine;

namespace FoldableMP7A1
{
	public class Hooks
	{
		private const string MP7_ID = "Mp7a1";

		public Hooks()
		{
			Hook();
		}

		public void Dispose()
		{
			Unhook();
		}

		#region Hooks

		private void Hook()
		{
			On.FistVR.ClosedBoltWeapon.Awake += ClosedBoltWeapon_Awake;
			On.FistVR.ItemSpawnerUI.Draw_Tiles_Detail += ItemSpawnerUI_Draw_Tiles_Detail;
			On.FistVR.ShotgunMoveableStock.UpdateInteraction += ShotgunMoveableStock_UpdateInteraction;
			On.FistVR.ShotgunMoveableStock.EndInteraction += ShotgunMoveableStock_EndInteraction;
		}

		private void Unhook()
		{
			On.FistVR.ClosedBoltWeapon.Awake -= ClosedBoltWeapon_Awake;
			On.FistVR.ItemSpawnerUI.Draw_Tiles_Detail -= ItemSpawnerUI_Draw_Tiles_Detail;
			On.FistVR.ShotgunMoveableStock.UpdateInteraction -= ShotgunMoveableStock_UpdateInteraction;
			On.FistVR.ShotgunMoveableStock.EndInteraction -= ShotgunMoveableStock_EndInteraction;
		}

		#endregion

		#region Init

		// Default firearm size and apply folding stock
		private void ClosedBoltWeapon_Awake(On.FistVR.ClosedBoltWeapon.orig_Awake orig, FistVR.ClosedBoltWeapon self)
		{
			orig(self);

			if (self.ObjectWrapper.ItemID == MP7_ID)
			{
				self.Size = FVRPhysicalObject.FVRPhysicalObjectSize.Medium;
				AddFoldingHandle(self);
			}
		}

		// Add 20rnd mag to mp7 page
		private void ItemSpawnerUI_Draw_Tiles_Detail(On.FistVR.ItemSpawnerUI.orig_Draw_Tiles_Detail orig, ItemSpawnerUI self, ItemSpawnerID item)
		{
			if (item.ItemID == "PDW" + MP7_ID && item.Secondaries.Length <= 2)
			{
				AddFTWMagToItemSpawnerPage(item);
			}

			orig(self, item);
		}

		#endregion

		#region Stock

		// Can't extend stock in small/medium qb slot
		private void ShotgunMoveableStock_UpdateInteraction(On.FistVR.ShotgunMoveableStock.orig_UpdateInteraction orig, ShotgunMoveableStock self, FVRViveHand hand)
		{
			var firearm = self.Firearm;
			if (firearm != null && firearm.ObjectWrapper.ItemID == MP7_ID)
			{
				var qb = firearm.QuickbeltSlot;
				if (qb == null || (qb != null && qb.SizeLimit != FVRPhysicalObject.FVRPhysicalObjectSize.Medium && qb.SizeLimit != FVRPhysicalObject.FVRPhysicalObjectSize.Small))
				{
					orig(self, hand);
				}
			}
			else
			{
				orig(self, hand);
			}
		}

		// Change firearm size depending on stock pos
		private void ShotgunMoveableStock_EndInteraction(On.FistVR.ShotgunMoveableStock.orig_EndInteraction orig, ShotgunMoveableStock self, FVRViveHand hand)
		{

			orig(self, hand);

			var firearm = self.Firearm;
			if (firearm != null && firearm.ObjectWrapper.ItemID == MP7_ID)
			{
				float stockPos = Vector3.Distance(self.transform.position, self.ForwardPoint.position);
				if (stockPos < 0.01f)
				{
					firearm.Size = FVRPhysicalObject.FVRPhysicalObjectSize.Medium;
				}
				else
				{
					firearm.Size = FVRPhysicalObject.FVRPhysicalObjectSize.Large;
				}
			}
		}

		#endregion

		private void AddFoldingHandle(ClosedBoltWeapon weapon)
		{
			var handlePhys = weapon.transform.Find("Phys/Phys_Cube (12)");
			var geo = weapon.transform.Find("Geo");
			var flipper = geo.Find("FlipSightTrigger_Front");
			var handle = geo.Find("Handle");

			var handleFlipper = GameObject.Instantiate(flipper, geo);
			handleFlipper.transform.localPosition = new Vector3(0, 0.02f, 0.09f);
			handleFlipper.GetComponent<SphereCollider>().radius = 0.005f;

			var hflipper = handleFlipper.gameObject.GetComponent<AR15HandleSightFlipper>();
			hflipper.name = "HandleFlipper";
			hflipper.Flipsight = handle;

			weapon.Foregrip.transform.parent = handle;
			handlePhys.parent = handle;
		}

		private void AddFTWMagToItemSpawnerPage(ItemSpawnerID item)
		{
			IM.SCD.TryGetValue(ItemSpawnerID.ESubCategory.PDW, out var pdwSubCatIDs);
			foreach (var id in pdwSubCatIDs)
			{
				if (id.ItemID == "FTW.mag.MP7.20")
				{
					Array.Resize(ref item.Secondaries, 3);
					item.Secondaries[2] = id;
					break;
				}
			}
		}
	}
}
