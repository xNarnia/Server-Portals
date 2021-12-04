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
	public class EmberPortalTile : PortalParentTile, IServerPortal
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 5);
			TileObjectData.newTile.Height = 6;

			// We set processedCoordinates to true so our Hook_AfterPlacement gets top left coordinates, regardless of Origin.
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<PortalTileEntity>().Hook_AfterPlacement, -1, 0, true);

			// Allow attaching sign to the ground
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("birB's Ember Gate");
			AddMapEntry(new Color(150, 150, 250), name);
			dustType = mod.DustType("Sparkle");
			disableSmartCursor = true;
			adjTiles = new int[] { Type };
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

		public override void MouseOverFar(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int left = i - tile.frameX % 54 / 18;
			int top = j - tile.frameY / 18;

			int index = GetInstance<PortalTileEntity>().Find(left, top);
			if (index != -1)
			{
				PortalTileEntity tileEntity = (PortalTileEntity)TileEntity.ByID[index];
				GateLabelMenu.UpdateLabelUsing(tileEntity);

				Player player = Main.LocalPlayer;
				player.noThrow = 2;
				player.showItemIcon = false;
				GateLabelMenu.Pos = new Vector2(Main.mouseX, Main.mouseY);
				ServerPortals.ShowLabel();
			}
		}
	}
}