using Humanizer;
using ServerPortals.TileEntities;
using ServerPortals.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static ServerPortals.ServerPortals;

namespace ExampleServerPortals.Tiles
{
    public class ExamplePlaceablePrefilledPortalTile : ServerPortalTile
    {
        public override void SetStaticDefaults()
        {
            // If you forget to call this, your entire portal will break!
            base.SetStaticDefaults();

            // Tile properties
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };

            // Allows your portal to display it's stored internal information when hovering over it
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<ServerPortalTileEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.addTile(Type);
        }

        public override bool CanPlace(int i, int j)
        {
            ServerPortalsMod.ServerTransferMenu.InputServerIP.Text = "";
            ServerPortalsMod.ServerTransferMenu.InputServerPort.Text = "7777";
            ServerPortalsMod.ServerTransferMenu.InputServerName.Text = "Localhost";
            ServerPortalsMod.ServerTransferMenu.InputServerDescription.Text = "Look, it's me!";
            return true;
        }
    }
}
