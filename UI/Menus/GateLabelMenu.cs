using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Graphics;
using ServerPortals.Tiles;

namespace ServerPortals.UI
{
	public class GateLabelMenu : UIState
	{
		public bool Visible { get; set; } = true;
		public byte BlueTextByte { get; set; }
		public static Vector2 Pos { get; set; }
		public static string ServerIP { get; set; }
		public static int ServerPort { get; set; }
		public static string ServerName { get; set; }
		public static string ServerDescription { get; set; }

		public GateLabelMenu()
		{
			Pos = Vector2.Zero;
			ServerName = "";
			ServerIP = "";
			ServerDescription = "";
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			BlueTextByte = (byte)(Main.mouseTextColor - (255 - 150));

			Utils.DrawBorderString(Main.spriteBatch, $"Teleport to Server", 
				Pos + new Vector2(16, 16), 
				new Color(BlueTextByte, BlueTextByte, Main.mouseTextColor, Main.mouseTextColor));

			Utils.DrawBorderString(Main.spriteBatch, 
				$"{ServerName}\nIP: {ServerIP}:{ServerPort.ToString()}\n{ServerDescription}", 
				Pos + new Vector2(16, 40), 
				new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor));
		}

		public static void UpdateLabelUsing(IServerPortal portalTile)
		{
			ServerIP = portalTile.IP;
			ServerPort = portalTile.Port;
			ServerName = portalTile.Name;
			ServerDescription = portalTile.Description;
		}
	}
}
