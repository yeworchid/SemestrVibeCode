using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Common;
using Common.DTO;

namespace Client;

public class GameClient
{
    private TcpClient? _client;
    private NetworkStream? _stream;
    private Task? _receiveTask;
    
    public event Action<NetworkMessage>? MessageReceived;
    public bool IsConnected => _client?.Connected ?? false;
    
    public async Task<bool> Connect(string host, int port)
    {
        try
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port);
            _stream = _client.GetStream();
            
            _receiveTask = Task.Run(ReceiveMessages);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task SendMessage(MessageType type, object? payload = null)
    {
        if (_stream == null) return;
        
        var message = new NetworkMessage
        {
            Type = type,
            Payload = payload != null ? JsonSerializer.Serialize(payload) : ""
        };
        
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await _stream.WriteAsync(bytes, 0, bytes.Length);
    }
    
    private async Task ReceiveMessages()
    {
        if (_stream == null) return;
        
        var buffer = new byte[8192];
        
        try
        {
            while (_client?.Connected == true)
            {
                var bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;
                
                var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var message = JsonSerializer.Deserialize<NetworkMessage>(json);
                
                if (message != null)
                {
                    MessageReceived?.Invoke(message);
                }
            }
        }
        catch
        {
            // Connection closed
        }
    }
    
    public void Disconnect()
    {
        _stream?.Close();
        _client?.Close();
    }
}
