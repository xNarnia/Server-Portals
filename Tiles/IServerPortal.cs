using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerPortals.Tiles
{
	public interface IServerPortal
	{
		string ServerIP { get; set; }
		int ServerPort { get; set; }
		string ServerName { get; set; }
		string ServerDescription { get; set; }
	}
}
