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

namespace ServerPortals.Tiles
{
	public class DemonEyePortalTile : PortalParentTile, IServerPortal
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 4);
			TileObjectData.newTile.Height = 5;

			// We set processedCoordinates to true so our Hook_AfterPlacement gets top left coordinates, regardless of Origin.
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<PortalTileEntity>().Hook_AfterPlacement, -1, 0, true);

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
			if (++frameCounter >= 10)
			{
				frameCounter = 0;
				frame = ++frame % 4;
			}
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 40 / 255;
			g = 40 / 255;
			b = 40 / 255;
		}

		public override void MouseOverFar(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int left = i - tile.TileFrameX % 54 / 18;
			int top = j - tile.TileFrameY / 18;

			int index = GetInstance<PortalTileEntity>().Find(left, top);
			if (index != -1)
			{
				PortalTileEntity tileEntity = (PortalTileEntity)TileEntity.ByID[index];
				GateLabelMenu.UpdateLabelUsing(tileEntity);

				Player player = Main.LocalPlayer;
				player.noThrow = 2;
				player.cursorItemIconEnabled = false;
				GateLabelMenu.Pos = new Vector2(Main.mouseX / Main.UIScale, Main.mouseY / Main.UIScale);
				ServerPortals.ShowLabel();
			}
		}
	}
}