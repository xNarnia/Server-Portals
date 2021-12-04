using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ServerPortals.Items
{
	public class PortalParentItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("[c/1DDD60:Right-click to edit Server settings]\n'Your next destination is to the right, obviously!'");
		}

		public override void SetDefaults()
		{
			item.SetNameOverride("Casual Sign - Server Portal");
			item.width = 28;
			item.height = 20;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.value = 2000;
			item.createTile = TileType<Tiles.PortalParentTile>();
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