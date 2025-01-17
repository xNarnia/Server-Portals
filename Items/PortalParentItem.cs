using ServerPortals.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ServerPortals.Items
{
	public class PortalParentItem : ModItem
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
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 2000;
			Item.DefaultToPlaceableTile(TileType<PortalParentTile>(), 0);
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				ServerPortals.OpenMenu();
				return false;
			}
			else
			{
				return base.CanUseItem(player);
			}
		}

		public override void RightClick(Player player)
		{
		}
	}
}