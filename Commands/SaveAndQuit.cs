using Terraria.ModLoader;
using Terraria;

namespace ServerPortals.Commands
{
	public class CoinCommand : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "quit";

		public override string Description
			=> "Quit the game";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			WorldGen.SaveAndQuit(() =>
			{
				Netplay.ServerIP = new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 });
				Netplay.ListenPort = 7778;

				Main.menuMode = 10;
				Netplay.StartTcpClient();
			});
		}
	}
}