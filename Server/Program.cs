using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Common;
using Common.DTO;
using Server;

var server = new GameServer();
await server.Start();
