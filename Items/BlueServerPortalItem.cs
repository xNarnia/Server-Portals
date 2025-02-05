using ServerPortals.Tiles;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ServerPortals.Items
{
	public class BlueServerPortalItem : PortalParentItem
	{
		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 34;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.rare = 11;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 2000;

			// Registers a vertical animation with 3 frames and each one will last 7 ticks (~ 1/9 second)
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(12, 3));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation

			Item.DefaultToPlaceableTile(TileType<BlueServerPortalTile>(), 0);
		}

		public override bool IsLoadingEnabled(Mod mod) => GetInstance<Config>().LoadIncludedPortals;
	}
}