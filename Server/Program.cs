using Server;

Console.WriteLine("=== Сервер игры ===");
Console.WriteLine("Ожидание 4 игроков для автостарта");
Console.WriteLine("Или введите 'start' когда подключатся минимум 2 игрока");

var server = new GameServer();

Task.Run(() => server.Start(5000));

while (true)
{
    string? input = Console.ReadLine();
    if (input == "start")
    {
        server.ForceStart();
    }
}
