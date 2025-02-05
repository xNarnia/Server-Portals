using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ServerPortals.TileEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace ServerPortals.Tiles
{
	public class BlueServerPortalTile : ServerPortalTile
	{
		private Asset<Texture2D> glowTexture;
		private float playerDistance = 0;

		public override void SetStaticDefaults()
		{
			glowTexture = ModContent.Request<Texture2D>(Texture + "_GlowMask");

			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 3);
			TileObjectData.newTile.Height = 4;

			// We set processedCoordinates to true so our Hook_AfterPlacement gets top left coordinates, regardless of Origin.
			// Sends data to the server in multiplayer servers
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<ServerPortalTileEntity>().Hook_AfterPlacement, -1, 0, true);

			// Allow attaching sign to the ground
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}

		// Since we're using PreDraw to include a GlowMask, we'll handle animating the tile in PreDraw instead
		//public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		//{
		//	frameXOffset = Main.tileFrame[Type] % 3 * 54;
		//}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			Main.NewText(playerDistance);

			// Player Distance 90 or less
			frameCounter += 15 - (int)Math.Clamp(playerDistance / 15, 0, 10);
			if (frameCounter >= 60)
			{
				frameCounter = 0;
				frame = ++frame % 3;
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
			
			if(tile.TileFrameX == 0 && tile.TileFrameY % 54 == 0)
				playerDistance = Vector2.Distance(Main.LocalPlayer.position, new Vector2(i, j).ToWorldCoordinates()) - 30;

			// Animated tile
			var frameXOffset = Main.tileFrame[Type] % 3 * 54;

			// Tile width = 16 * 2 (+2 per padding)
			// Tile height = 16 * 4 (+2 per padding)
			int frameX = tile.TileFrameX % 36 + frameXOffset;
			int frameY = tile.TileFrameY % 72;

			frameX += (tile.TileFrameX / 36) * 36;

			spriteBatch.Draw(
				TextureAssets.Tile[Type].Value,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(frameX, frameY, 16, 16),
				Lighting.GetColor(i, j), 0f, default, 1f, SpriteEffects.None, 0f);

			// Draw glowmask
			spriteBatch.Draw(
				glowTexture.Value,
				new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
				new Rectangle(frameX, frameY, 16, 16),
				Lighting.GetColor(i, j) * 1.3f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 20f / 255f * 2.5f;
			g = 50f / 255f * 2.5f;
			b = 100f / 255f * 2.5f;
		}
	}
}
