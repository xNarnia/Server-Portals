using Microsoft.Xna.Framework;
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

		public string ServerIP { get; set; }
		public int ServerPort { get; set; }
		public string ServerName { get; set; }
		public string ServerDescription { get; set; }
		public static bool ServerSelectLock { get; set; } = false;
		public static bool NeedToSend { set; get; } = false;

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			base.NetReceive(reader, lightReceive);
			ServerIP = reader.ReadString();
			ServerPort = reader.ReadInt32();
			ServerName = reader.ReadString();
			ServerDescription = reader.ReadString();
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			base.NetSend(writer, lightSend);
			writer.Write(ServerIP);
			writer.Write(ServerPort);
			writer.Write(ServerName);
			writer.Write(ServerDescription);
		}

		public override TagCompound Save()
		{
			return new TagCompound
			{
				{"ServerIP", ServerIP},
				{"ServerPort", ServerPort},
				{"ServerName", ServerName},
				{"ServerDescription", ServerDescription}
			};
		}

		public override void Load(TagCompound tag)
		{
			ServerIP = tag.Get<string>("ServerIP");
			ServerPort = tag.Get<int>("ServerPort");
			ServerName = tag.Get<string>("ServerName");
			ServerDescription = tag.Get<string>("ServerDescription");
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
	}
}
