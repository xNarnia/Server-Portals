using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace ServerPortals.UI
{
	public class InputBox : UIPanel
	{
		public UIPanel Container;
		public string Label { get; } = "";
		public string Text = "";
		public bool Focused;
		public float OffsetX;

		public bool WidthIncludesLabel = false;
		private bool AdjustedForLabel = false;
		private char cursor = ' ';
		private double TotalSeconds;
		private byte baseR = 0;
		private byte baseG = 0;
		private byte baseB = 0;

		public InputBox() { }
		public InputBox(string label)
		{
			Label = label;
			PaddingTop = 0;
			PaddingBottom = 0;
			baseR = BackgroundColor.R;
			baseG = BackgroundColor.G;
			baseB = BackgroundColor.B;
			OverflowHidden = true;
		}

		public override void LeftClick(UIMouseEvent evt)
		{
			base.LeftClick(evt);
			this.Focused = true;
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			if (!AdjustedForLabel && Label != "")
			{
				if (WidthIncludesLabel)
				{
					Left.Pixels += FontAssets.MouseText.Value.MeasureString(Label).X;
				}

				AdjustedForLabel = true;
			}
		}

		public override void Update(GameTime gameTime)
		{
			// Do un-focus code
			base.Update(gameTime);

			if (TotalSeconds == 0)
				TotalSeconds = gameTime.TotalGameTime.TotalSeconds;

			if(gameTime.TotalGameTime.TotalSeconds - TotalSeconds > .5)
			{
				if (cursor == ' ')
					cursor = '|';
				else if (cursor == '|')
					cursor = ' ';

				TotalSeconds = gameTime.TotalGameTime.TotalSeconds;
			}

			var TextMeasure = FontAssets.MouseText.Value.MeasureString(Text);
			if (TextMeasure.X > Width.Pixels - PaddingLeft - PaddingRight)
			{
				OffsetX = TextMeasure.X - (Width.Pixels - PaddingLeft - PaddingRight);
			}
			else
			{
				OffsetX = 0;
			}

			if(Main.mouseLeft 
				&& (Main.mouseX < Left.Pixels || Main.mouseX > Left.Pixels + Width.Pixels 
					|| Main.mouseY < Top.Pixels || Main.mouseY > Top.Pixels + Height.Pixels))
			{
				Focused = false;
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Main.keyState.IsKeyDown(Keys.Escape))
				Focused = false;

			if (Focused)
			{
				PlayerInput.WritingText = true;
				Main.instance.HandleIME();
				Text = Main.GetInputText(Text);
				BackgroundColor.R = (byte)(baseR - 15);
				BackgroundColor.G = (byte)(baseG - 15);
				BackgroundColor.B = (byte)(baseB - 15);
			}
			else
			{
				BackgroundColor.R = baseR;
				BackgroundColor.G = baseG;
				BackgroundColor.B = baseB;
			}

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

			if (Focused)
			{
				Utils.DrawBorderString(spriteBatch, Text + cursor, Pos - new Vector2(OffsetX, 0) + GetCenterOffsetY(Height.Pixels, 18f), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor));
			}
			else
			{
				Utils.DrawBorderString(spriteBatch, Text, Pos - new Vector2(OffsetX, 0) + GetCenterOffsetY(Height.Pixels, 18f), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor));
			}

			if (Label != "")
			{
				Pos.X -= FontAssets.MouseText.Value.MeasureString(Label).X + PaddingLeft;
				Utils.DrawBorderString(spriteBatch, Label, Pos + GetCenterOffsetY(Height.Pixels, 18f), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor));
			}
		}

		private Vector2 GetCenterOffsetY(float Compare1Height, float Compare2Height)
		{
			Compare1Height -= Compare2Height;
			Compare1Height /= 2;
			return new Vector2(0, Compare1Height);
		}
	}
}
