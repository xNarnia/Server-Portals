using ServerPortals.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static ServerPortals.ServerPortals;
using ServerPortals.TileEntities;

namespace ServerPortals.Tiles
{
    public abstract class ServerPortalTile : ModTile, IServerPortal
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LeftPoint { get; set; }
        public int TopPoint { get; set; }

        public override bool CanPlace(int i, int j)
        {
            if (!ServerPortalsMod.DataIsValid())
            {
                ServerPortalsMod.OpenMenu();
                return false;
            }

            IP = ServerPortalsMod.ServerTransferMenu.InputServerIP.Text;
            int.TryParse(ServerPortalsMod.ServerTransferMenu.InputServerPort.Text, out int OutPort);
            Port = OutPort;
            Name = ServerPortalsMod.ServerTransferMenu.InputServerName.Text;
            Description = ServerPortalsMod.ServerTransferMenu.InputServerDescription.Text;

            return base.CanPlace(i, j);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = 1;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<ServerPortalTileEntity>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            if (ServerPortalTileEntity.ServerSelectLock)
                return true;

            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX % 36 / 18;
            int top = j - tile.TileFrameY / 18;

            int index = GetInstance<ServerPortalTileEntity>().Find(left, top);
            if (index == -1)
            {
                return false;
            }
            ServerPortalTileEntity tileEntity = (ServerPortalTileEntity)TileEntity.ByID[index];

            Netplay.ListenPort = tileEntity.Port;
            if (Netplay.SetRemoteIP(tileEntity.IP))
            {
                ServerPortalTileEntity.ServerSelectLock = true;
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

            int index = GetInstance<ServerPortalTileEntity>().Find(left, top);
            if (index != -1)
            {
                ServerPortalTileEntity tileEntity = (ServerPortalTileEntity)TileEntity.ByID[index];
                GateLabelMenu.UpdateLabelUsing(tileEntity);

                Player player = Main.LocalPlayer;
                player.noThrow = 2;
                player.cursorItemIconEnabled = false;
                GateLabelMenu.Pos = new Vector2(Main.mouseX / Main.UIScale, Main.mouseY / Main.UIScale);
                ServerPortalsMod.ShowLabel();
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

            ServerPortalTileEntity.ServerSelectLock = false;
        }
    }
}
