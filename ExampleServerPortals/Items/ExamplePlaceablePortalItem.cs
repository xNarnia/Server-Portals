using ServerPortals.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using static ServerPortals.ServerPortals;
using ExampleServerPortals.Tiles;

namespace ExampleServerPortals.Items
{
    public class ExamplePlaceablePortalItem : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;

            Item.DefaultToPlaceableTile(ModContent.TileType<ExamplePlaceablePortalTile>(), 0);
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
    }
}
