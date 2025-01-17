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
	public class PortalParentTile : ModTile, IServerPortal
	{
		public string IP { get; set; }
		public int Port { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int LeftPoint { get; set; }
		public int TopPoint { get; set; }

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);

			// We set processedCoordinates to true so our Hook_AfterPlacement gets top left coordinates, regardless of Origin.
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<PortalTileEntity>().Hook_AfterPlacement, -1, 0, true);

			// Allow hanging from ceilings
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorLeft = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorRight = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(1);

			// Allow attaching to a solid object that is to the left of the sign
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(2);

			// Allow attaching to a solid object that is to the right of the sign
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(3);

			// Allow attaching to a wall behind the sign
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(4);

			// Allow attaching sign to the ground
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.StyleHorizontal = true;
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124 };
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.addAlternate(5);
			TileObjectData.addTile(Type);
		}

		public override bool CanPlace(int i, int j)
		{
			if (!ServerPortals.DataIsValid())
			{
				ServerPortals.OpenMenu();
				return false;
			}

			IP = ServerPortals.ServerTransferMenu.InputServerIP.Text;
			int.TryParse(ServerPortals.ServerTransferMenu.InputServerPort.Text, out int OutPort);
			Port = OutPort;
			Name = ServerPortals.ServerTransferMenu.InputServerName.Text;
			Description = ServerPortals.ServerTransferMenu.InputServerDescription.Text;

			return base.CanPlace(i, j);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			ModContent.GetInstance<PortalTileEntity>().Kill(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			if (PortalTileEntity.ServerSelectLock)
				return true;

			Tile tile = Main.tile[i, j];
			int left = i - tile.TileFrameX % 36 / 18;
			int top = j - tile.TileFrameY / 18;

			int index = GetInstance<PortalTileEntity>().Find(left, top);
			if (index == -1)
			{
				return false;
			}
			PortalTileEntity tileEntity = (PortalTileEntity)TileEntity.ByID[index];

			Netplay.ListenPort = tileEntity.Port;
			if (Netplay.SetRemoteIP(tileEntity.IP))
			{
				PortalTileEntity.ServerSelectLock = true;
				Main.CloseNPCChatOrSign();
				ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectToServerIP), 1);
			}
			else
			{
				Main.NewText("Could not find server!");
			}
			return true;
		}
		
		public override void MouseOverFar(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int left = i - tile.TileFrameX % 36 / 18;
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

		private void ConnectToServerIP(object threadContext)
		{
			Ping pingSender = new Ping();
			PingOptions options = new PingOptions();
			options.DontFragment = true;

			string data = "a";
			byte[] buffer = Encoding.ASCII.GetBytes(data);
			int timeout = 1000;

			PingReply reply = null;
			try
			{
				reply = pingSender.Send(Netplay.ServerIP, timeout, buffer, options);
			}
			catch
			{
				Main.NewText("Could not ping destination server!");
				return;
			}

			if (reply.Status == IPStatus.Success)
			{
				WorldGen.SaveAndQuit(() =>
				{
					Main.menuMode = 10;
					Netplay.StartTcpClient();
				});
			}
			else
			{
				Main.NewText("Ping to server timed out!");
			}

			PortalTileEntity.ServerSelectLock = false;
		}
	}
}