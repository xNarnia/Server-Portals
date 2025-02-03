using ExampleServerPortals.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExampleServerPortals.Items
{
    public class ExamplePlaceablePrefilledPortalItem : ModItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 15;

            Item.DefaultToPlaceableTile(ModContent.TileType<ExamplePlaceablePrefilledPortalTile>(), 0);
        }
    }
}
