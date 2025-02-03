using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using ReLogic.Graphics;
using ServerPortals.UI;
using System.Net;
using Terraria.Enums;
using Terraria.DataStructures;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using ServerPortals.TileEntities;

namespace ServerPortals.Tiles
{
    public class EmberPortalTile : PortalParentTile, IServerPortal
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 5);
			TileObjectData.newTile.Height = 6;

			// We set processedCoordinates to true so our Hook_AfterPlacement gets top left coordinates, regardless of Origin.
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<ServerPortalTileEntity>().Hook_AfterPlacement, -1, 0, true);

			// Allow attaching sign to the ground
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			frameXOffset = Main.tileFrame[Type] % 4 * 54;
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			// Spend 9 ticks on each of 6 frames, looping
			frameCounter++;
			// Or, more compactly:
			if (++frameCounter >= 20)
			{
				frameCounter = 0;
				frame = ++frame % 7;
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.93f;
			g = 0.11f;
			b = 0.12f;
		}
	}
}