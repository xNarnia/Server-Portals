using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace ServerPortals
{
	public class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		[Header("General")]
		[DefaultValue(true)]
		[ReloadRequired]
		public bool LoadIncludedPortals { get; set; }
	}
}
