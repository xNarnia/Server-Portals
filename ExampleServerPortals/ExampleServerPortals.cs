using Terraria.ModLoader;
using static ServerPortals.ServerPortals;

namespace ExampleServerPortals
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class ExampleServerPortals : Mod
	{
        public override void PostSetupContent()
        {
            base.PostSetupContent();

            // Allows ServerPortals to find ServerPortalTile types
            // and register them for loading/saving to your World Files
            ServerPortalsMod.ScanForPortals(this);
        }
    }
}
