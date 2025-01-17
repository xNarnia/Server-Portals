using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ServerPortals.UI
{
	[Autoload(Side = ModSide.Client)]
	public class UISystem : ModSystem
	{
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					"ServerPortals: UI",
					delegate
					{
						ServerPortals._gateLabelUI.Draw(Main.spriteBatch, new GameTime());
						ServerPortals._serverTransferUI.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			ServerPortals._gateLabelUI?.Update(gameTime);
			ServerPortals._serverTransferUI?.Update(gameTime);
			ServerPortals.HideLabel();
		}
	}
}
