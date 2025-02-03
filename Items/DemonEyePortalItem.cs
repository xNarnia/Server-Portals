using ServerPortals.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static ServerPortals.ServerPortals;

namespace ServerPortals.Items
{
	public class DemonEyePortalItem : PortalParentItem
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.rare = 11;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 2000;
			Item.DefaultToPlaceableTile(TileType<DemonEyePortalTile>(), 0);
		}

		public override bool IsLoadingEnabled(Mod mod) => GetInstance<Config>().LoadIncludedPortals;
	}
}