using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using ReLogic.Graphics;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace ServerPortals.UI
{
	public class TextButton : UIPanel
	{
		public string Label;

		public TextButton() { }
		public TextButton(string label)
		{
			Label = label;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();
			PaddingLeft = 0;
			PaddingRight = 0;
			PaddingTop = 0;
			PaddingBottom = 0;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Vector2 Pos = 
				new Vector2(
					Parent.Left.Pixels 
					+ Parent.PaddingLeft 
					+ Left.Pixels
					+ PaddingLeft,
					
					Parent.Top.Pixels 
					+ Parent.PaddingTop 
					+ Top.Pixels
					+ PaddingTop);

			Utils.DrawBorderString(Main.spriteBatch, Label, Pos + new Vector2(Width.Pixels / 2 - FontAssets.MouseText.Value.MeasureString(Label).X / 2, Height.Pixels / 2 - 9f), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor));
		}
	}
}
