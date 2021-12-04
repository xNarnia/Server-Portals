using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using ServerPortals.UI;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;

namespace ServerPortals
{
	public class ServerPortals : Mod
	{
		public static ServerPortals Instance;
		internal static GateLabelMenu GateLabel;
		internal static ServerTransferCreationMenu ServerTransferMenu;

		private static UserInterface _gateLabelUI;
		private static UserInterface _serverTransferUI;

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

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"WorldLink: UI",
					delegate 
					{
						_gateLabelUI.Draw(Main.spriteBatch, new GameTime());
						_serverTransferUI.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)  
				);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			_gateLabelUI?.Update(gameTime);
			_serverTransferUI?.Update(gameTime);
			HideLabel();
		}

		// Everything below is courtesy of the amazing Blushiemagic and her Magic Storage GitHub repo
		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType type = (MessageType)reader.ReadByte();
			if (type == MessageType.ClientSendTEUpdate)
				ReceiveClientSendTEUpdate(reader, whoAmI);

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

		public enum MessageType : byte
		{
			ClientSendTEUpdate
		}
	}
}