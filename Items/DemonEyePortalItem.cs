using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ServerPortals.Items
{
	public class DemonEyePortalItem : PortalParentItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("[c/1DDD60:Right-click to edit Server settings]\n[c/00FFFF:Sprite by @birB!]\n'A dreadful eye capable of peering into other worlds.'");
		}

		public override void SetDefaults()
		{
			item.SetNameOverride("The Eye of birB - Server Portal");
			item.width = 28;
			item.height = 20;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.rare = 11;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = 2000;
			item.createTile = TileType<Tiles.DemonEyePortalTile>();
		}
	}
}