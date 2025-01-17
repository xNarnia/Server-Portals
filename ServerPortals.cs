using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using ServerPortals.UI;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;
using ServerPortals.Tiles;
using System;

namespace ServerPortals
{
	public class ServerPortals : Mod
	{
		public static ServerPortals Instance { get; private set; }
		public static GateLabelMenu GateLabel { get; set; }
		public static ServerTransferCreationMenu ServerTransferMenu { get; set; }

		public static UserInterface _gateLabelUI {  get; set; }
		public static UserInterface _serverTransferUI {  get; set; }

		public static bool ShowText;
		public static string Text;
		public static Vector2 Pos;

		public ServerPortals()
		{
			Pos = Vector2.Zero;
		}

		public static void OpenMenu()
		{
			if(_serverTransferUI.CurrentState != ServerTransferMenu)
				_serverTransferUI.SetState(ServerTransferMenu);
		}

		public static void CloseMenu()
		{
			if (_serverTransferUI.CurrentState != null)
				_serverTransferUI.SetState(null);
		}

		public static void ShowLabel()
			=> _gateLabelUI.SetState(GateLabel);

		public static void HideLabel()
			=> _gateLabelUI.SetState(null);

		public static bool DataIsValid()
		{
			int.TryParse(ServerTransferMenu.InputServerPort.Text, out int OutPort);

			if (OutPort == 0
				|| ServerTransferMenu.InputServerIP.Text == "" || ServerTransferMenu.InputServerIP.Text == null
				|| ServerTransferMenu.InputServerName.Text == "" || ServerTransferMenu.InputServerName.Text == null)
			{
				return false;
			}

			return true;
		}

		public override void Load()
		{
			Instance = this;
			
			GateLabel = new GateLabelMenu();
			GateLabel.Activate();

			ServerTransferMenu = new ServerTransferCreationMenu();
			ServerTransferMenu.Activate();

			_serverTransferUI = new UserInterface();
			_gateLabelUI = new UserInterface();

			base.Load();
		}

		public override void Unload()
		{
			base.Unload();
			_gateLabelUI = null;
			ServerTransferMenu = null;
			GateLabel = null;
		}

		public override void PostSetupContent()
		{
			base.PostSetupContent();
		}


		// Everything below is courtesy of the amazing Blushiemagic and her Magic Storage GitHub repo
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType type = (MessageType)reader.ReadByte();

			if (type == MessageType.ClientSendTEUpdate)
				ReceiveClientSendTEUpdate(reader, whoAmI);
			else if (type == MessageType.ClientSendPortalPlacement)
				ReceiveClientPortalPlacement(reader, whoAmI);

			base.HandlePacket(reader, whoAmI);
		}

		public static void ReceiveClientSendTEUpdate(BinaryReader reader, int sender)
		{
			if (Main.netMode == 2)
			{
				int id = reader.ReadInt32();
				TileEntity te = TileEntity.Read(reader, true);
				te.ID = id;
				TileEntity.ByID[id] = te;
				TileEntity.ByPosition[te.Position] = te;

				NetMessage.SendData(MessageID.TileEntitySharing, -1, sender, null, id, te.Position.X, te.Position.Y);
			}
			else if (Main.netMode == 1)
			{
				reader.ReadInt32();
			}
		}

		public static void ClientSendTEUpdate(int id)
		{
			if (Main.netMode == 1)
			{
				ModPacket packet = Instance.GetPacket();
				packet.Write((byte)MessageType.ClientSendTEUpdate);
				packet.Write(id);
				TileEntity.Write(packet, TileEntity.ByID[id], true);
				packet.Send();
			}
		}

		public static void ReceiveClientPortalPlacement(BinaryReader reader, int whoAmI)
		{
			if (Main.netMode == 2)
			{
				try
				{
					string IP = reader.ReadString();
					int Port = reader.ReadInt32();
					string Name = reader.ReadString();
					string Desc = reader.ReadString();
					int x = reader.ReadInt32();
					int y = reader.ReadInt32();

					var instance = ModContent.GetInstance<PortalTileEntity>();

					int id = instance.Find(x, y);

					if (id == -1)
					{
						id = ModContent.GetInstance<PortalTileEntity>().Place(x, y);
					}

					PortalTileEntity tileEntity = (PortalTileEntity)TileEntity.ByID[id];
					tileEntity.SetData(new Server()
					{
						IP = IP,
						Port = Port,
						Name = Name,
						Description = Desc
					});
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, id, x, y);
				}
				catch
				{
					Console.WriteLine("- Error receiving Portal Placement");
				}
			}
			else if (Main.netMode == 1)
			{
				reader.ReadString();
				reader.ReadInt32();
				reader.ReadString();
				reader.ReadString();
				reader.ReadInt32();
				reader.ReadInt32();
			}
		}

		public static void ClientSendPortalPlacement(Server server, int tileX, int tileY)
		{
			if (Main.netMode == 1)
			{
				ModPacket packet = Instance.GetPacket();
				packet.Write((byte)MessageType.ClientSendPortalPlacement);
				packet.Write(server.IP);
				packet.Write(server.Port);
				packet.Write(server.Name);
				packet.Write(server.Description);
				packet.Write(tileX);
				packet.Write(tileY);
				packet.Send();
			}
		}

		public enum MessageType : byte
		{
			ClientSendTEUpdate,
			ClientSendPortalPlacement
		}
	}
}