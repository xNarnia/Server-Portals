using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPortals.Tiles
{
	public interface IServerPortal
	{
		string IP { get; set; }
		int Port { get; set; }
		string Name { get; set; }
		string Description { get; set; }
	}

	public struct Server : IServerPortal
	{
		public string IP { get; set; }
		public int Port { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
