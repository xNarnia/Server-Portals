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
using Terraria.GameContent;

namespace ServerPortals.UI
{
	public class ServerTransferCreationMenu : UIState
	{
		public float Width = 500f;
		public float Height = 248f;
		public bool Visible = true;
		public bool InputFocused = false;

		public InputBox InputServerIP;
		public InputBox InputServerPort;
		public InputBox InputServerName;
		public InputBox InputServerDescription;


		UIPanel Panel;
		UIPanel HeaderBorderPanel;
		TextButton ClearButton;
		TextButton SaveButton;

		List<InputBox> Inputs;

		public override void OnInitialize()
		{
			Panel = new UIPanel();
			Panel.Left.Set(Main.screenWidth / 2 - Width / 2, 0f);
			Panel.Top.Set(Main.screenHeight / 2 - Height / 2, 0f);
			Panel.Width.Set(Width, 0f);
			Panel.Height.Set(Height, 0f);
			Panel.BackgroundColor.R -= 25;
			Panel.BackgroundColor.G -= 25;
			Panel.BackgroundColor.B -= 25;

			HeaderBorderPanel = new UIPanel();
			HeaderBorderPanel.Left.Set(Panel.Width.Pixels / 2 - 166, 0f);
			HeaderBorderPanel.Top.Set(-32, 0f);
			HeaderBorderPanel.Width.Set(312, 0f);
			HeaderBorderPanel.Height.Set(48, 0f);

			Panel.Append(HeaderBorderPanel);

			Inputs = new List<InputBox>();

			InputServerIP = new InputBox("Server IP ");
			InputServerPort = new InputBox("Server Port ");
			InputServerName = new InputBox("Server Name ");
			InputServerDescription = new InputBox("Server Desc ");

			Inputs.Add(InputServerIP);
			Inputs.Add(InputServerPort);
			Inputs.Add(InputServerName);
			Inputs.Add(InputServerDescription);

			var i = 1;
			foreach (var input in Inputs)
			{
				input.WidthIncludesLabel = false;
				input.Width.Set(Panel.Width.Pixels - Panel.PaddingLeft - Panel.PaddingRight - 114, 0f);
				input.Height.Set(36, 0f);
				input.Left.Set(114, 0f);
				input.Top.Set(32 * i + 8 * (i - 1) - 7, 0f);
				Panel.Append(input);
				i++;
			}

			ClearButton = new TextButton("Clear");
			ClearButton.Width.Set(72, 0f);
			ClearButton.Height.Set(36, 0f);
			ClearButton.Left.Set(0, 0f);
			ClearButton.Top.Set(CalculateButtonHeight(ClearButton), 0f);
			ClearButton.OnLeftClick += new MouseEvent(ClearButtonClicked);
			Panel.Append(ClearButton);

			SaveButton = new TextButton("Save"); 
			SaveButton.Width.Set(72, 0f);
			SaveButton.Height.Set(36, 0f);
			SaveButton.Left.Set(CalculateButtonWidth(SaveButton), 0f);
			SaveButton.Top.Set(CalculateButtonHeight(SaveButton), 0f);
			SaveButton.OnLeftClick += new MouseEvent(SaveButtonClicked);
			Panel.Append(SaveButton);

			Append(Panel);
		}

		public static Server GetData()
		{
			var output = new Server();

			output.IP = ServerPortals.ServerTransferMenu.InputServerIP.Text;
			int.TryParse(ServerPortals.ServerTransferMenu.InputServerPort.Text, out int OutPort);
			output.Port = OutPort;
			output.Name = ServerPortals.ServerTransferMenu.InputServerName.Text;
			output.Description = ServerPortals.ServerTransferMenu.InputServerDescription.Text;

			return output;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			InputFocused = false;
			foreach(var input in Inputs)
			{
				if (input.Focused)
					InputFocused = true;
			}

			if (InputFocused)
				Main.blockInput = true;
			else
				Main.blockInput = false;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Utils.DrawBorderStringBig(
				spriteBatch, 
				"Server Transfer Portal", 
				new Vector2(Panel.Left.Pixels + Panel.PaddingLeft + Panel.Width.Pixels / 2 - FontAssets.MouseText.Value.MeasureString("Server Transfer Portal").X + 32, Panel.Top.Pixels + Panel.PaddingTop - 24), 
				new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor),
				0.6f);
		}

		private void ClearButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			InputServerIP.Text = "";
			InputServerPort.Text = "";
			InputServerName.Text = "";
			InputServerDescription.Text = "";
		}

		private void SaveButtonClicked(UIMouseEvent evt, UIElement listeningElement)
		{
			ServerPortals.CloseMenu();
		}
		
		private float CalculateButtonWidth(UIPanel button)
		{
			return Panel.Width.Pixels - Panel.PaddingLeft - Panel.PaddingRight - button.Width.Pixels;
		}

		private float CalculateButtonHeight(UIPanel button)
		{
			return Panel.Height.Pixels - Panel.PaddingTop - Panel.PaddingBottom - button.Height.Pixels;
		}
	}
}
