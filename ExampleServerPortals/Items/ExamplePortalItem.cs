using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static ServerPortals.ServerPortals;
using static Terraria.ModLoader.ModContent;

namespace ExampleServerPortals.Items
{

    public class ExamplePortalItem : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                ServerPortalsMod.OpenMenu();
                return false;
            }
            else
            {
                return base.CanUseItem(player);
            }
        }

        public override bool? UseItem(Player player)
        {
            if (ServerPortalsMod.MenuIsOpen())
                return true;

            if (!ServerPortalsMod.DataIsValid())
            {
                Main.NewText("Server information invalid! Right-click me!");
                return true;
            }
            else
            {
                if (ServerPortalsMod.DataIsValid())
                {
                    var IP = ServerPortalsMod.ServerTransferMenu.InputServerIP.Text;
                    int.TryParse(ServerPortalsMod.ServerTransferMenu.InputServerPort.Text, out int OutPort);

                    ServerPortalsMod.JoinServer(IP, OutPort);
                    return true;
                }

                return base.CanUseItem(player);
            }
        }
    }
}