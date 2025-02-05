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
using System.Threading;
using System.Net.NetworkInformation;
using System.Text;
using Ionic.Zlib;
using System.Reflection;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServerPortals.TileEntities;
using tModPorter;

namespace ServerPortals
{
    public class ServerPortals : Mod
	{
		public static ServerPortals ServerPortalsMod { get; private set; }
		public Config Config { get; set; }
		public GateLabelMenu GateLabel { get; set; }
		public ServerTransferCreationMenu ServerTransferMenu { get; set; }
		public List<int> PortalTileTypes { get; set; }

		public UserInterface _gateLabelUI {  get; set; }
		public UserInterface _serverTransferUI {  get; set; }

		public bool ShowText;
		public string Text;
		public Vector2 Pos;

		public ServerPortals()
		{
			PortalTileTypes = new List<int>();
			Pos = Vector2.Zero;
		}

		public void OpenMenu()
		{
			if(_serverTransferUI.CurrentState != ServerTransferMenu)
				_serverTransferUI.SetState(ServerTransferMenu);
		}

		public void CloseMenu()
		{
			if (_serverTransferUI.CurrentState != null)
				_serverTransferUI.SetState(null);
		}

		public void ShowLabel()
			=> _gateLabelUI.SetState(GateLabel);

		public void HideLabel()
			=> _gateLabelUI.SetState(null);

		public bool DataIsValid()
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

		public bool MenuIsOpen() => _serverTransferUI.CurrentState == ServerTransferMenu;

		public void JoinServer(string remoteAddress, int port)
		{
            Netplay.ListenPort = port;
            if (Netplay.SetRemoteIP(remoteAddress))
            {
                ServerPortalTileEntity.ServerSelectLock = true;
                Main.CloseNPCChatOrSign();
                ThreadPool.QueueUserWorkItem(new WaitCallback(ConnectToServerIP), 1);
            }
            else
            {
                Main.NewText("Could not find server!");
            }
        }

		public void SetServerInfo(string serverIP, int serverPort, string serverName = "", string serverDesc = "")
		{
			ServerTransferMenu.InputServerIP.Text = serverIP;
			ServerTransferMenu.InputServerPort.Text = serverPort.ToString();
			ServerTransferMenu.InputServerName.Text = serverName;
			ServerTransferMenu.InputServerDescription.Text = serverDesc;
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

            ServerPortalTileEntity.ServerSelectLock = false;
        }

		/// <summary>
		/// Scans for all ServerPortalTiles in supplied Mod object
		/// </summary>
		/// <param name="mod">The mod to scan for ServerPortalTiles in.</param>
        public void ScanForPortals(Mod mod)
		{
            foreach (Type type in Assembly.GetAssembly(mod.GetType()).GetTypes())
            {
                if (typeof(ServerPortalTile).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass)
                {
					// Supply type at runtime by using reflection to find the ModContent.TileType method and invoking it
                    MethodInfo method = typeof(ModContent).GetMethod("TileType", BindingFlags.Public | BindingFlags.Static);
                    MethodInfo genericMethod = method.MakeGenericMethod(type);

                    PortalTileTypes.Add((int)genericMethod.Invoke(null, null));
                }
            }
        }

        public override void Load()
		{
			ServerPortalsMod = this;

			Config = ModContent.GetInstance<Config>();

			GateLabel = new GateLabelMenu();
			GateLabel.Activate();

			ServerTransferMenu = new ServerTransferCreationMenu();
			ServerTransferMenu.Activate();

			_serverTransferUI = new UserInterface();
			_gateLabelUI = new UserInterface();

			ScanForPortals(this);

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

		public void ReceiveClientSendTEUpdate(BinaryReader reader, int sender)
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

		public void ClientSendTEUpdate(int id)
		{
			if (Main.netMode == 1)
			{
				ModPacket packet = ServerPortalsMod.GetPacket();
				packet.Write((byte)MessageType.ClientSendTEUpdate);
				packet.Write(id);
				TileEntity.Write(packet, TileEntity.ByID[id], true);
				packet.Send();
			}
		}

		public void ReceiveClientPortalPlacement(BinaryReader reader, int whoAmI)
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

					var instance = ModContent.GetInstance<ServerPortalTileEntity>();

					int id = instance.Find(x, y);

					if (id == -1)
					{
						id = ModContent.GetInstance<ServerPortalTileEntity>().Place(x, y);
					}

					ServerPortalTileEntity tileEntity = (ServerPortalTileEntity)TileEntity.ByID[id];
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

		public void ClientSendPortalPlacement(Server server, int tileX, int tileY)
		{
			if (Main.netMode == 1)
			{
				ModPacket packet = ServerPortalsMod.GetPacket();
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