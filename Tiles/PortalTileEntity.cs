using Microsoft.Xna.Framework;
using ServerPortals.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace ServerPortals.Tiles
{
	public class PortalTileEntity : ModTileEntity, IServerPortal
	{
		// Half the width in Tile Coordinates.
		internal const int range = 50;
		internal const int drawBorderWidth = 5;

		public string IP { get; set; }
		public int Port { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public static bool ServerSelectLock { get; set; } = false;
		public static bool NeedToSend { set; get; } = false;

		public void SetData(Server server)
		{
			IP = server.IP;
			Port = server.Port;
			Name = server.Name;
			Description = server.Description;
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			base.NetReceive(reader, lightReceive);
			IP = reader.ReadString();
			Port = reader.ReadInt32();
			Name = reader.ReadString();
			Description = reader.ReadString();
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			base.NetSend(writer, lightSend);
			writer.Write(IP);
			writer.Write(Port);
			writer.Write(Name);
			writer.Write(Description);
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"ServerIP", IP},
				{"ServerPort", Port},
				{"ServerName", Name},
				{"ServerDescription", Description}
			};
		}

		public override void Load(TagCompound tag)
		{
			IP = tag.Get<string>("ServerIP");
			Port = tag.Get<int>("ServerPort");
			Name = tag.Get<string>("ServerName");
			Description = tag.Get<string>("ServerDescription");
		}

		public override bool ValidTile(int i, int j)
		{
			List<int> validTiles = new List<int>();
			validTiles.Add(ModContent.TileType<PortalParentTile>());
			validTiles.Add(ModContent.TileType<EmberPortalTile>());
			validTiles.Add(ModContent.TileType<DemonEyePortalTile>());

			Tile tile = Main.tile[i, j];
			return tile.active() && validTiles.Contains(tile.type) && tile.frameX == 0 && tile.frameY == 0;
		}

		public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
		{
			Tile tile = Main.tile[i, j];
			int left = i - tile.frameX % 36 / 18;
			int top = j - tile.frameY / 18;

			if (Main.netMode == 1)
			{
				NetMessage.SendTileRange(Main.myPlayer, i, j, 3, 6);
				NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, left, top, type);
				ServerPortals.ClientSendPortalPlacement(ServerTransferCreationMenu.GetData(), i, j);
				return -1;
			}

			int id = Place(left, top);

			PortalTileEntity tileEntity = (PortalTileEntity)TileEntity.ByID[id];
			tileEntity.SetData(ServerTransferCreationMenu.GetData());

			return ID;
		}
	}
}
