namespace Common.Services;

// Парсер для работы с сообщениями в формате "ТИП|ДАННЫЕ"
// Например: "JOIN|{"nickname":"Player1"}"
public static class MessageParser
{
    // Разбирает строку на тип сообщения и данные
    public static NetworkMessage Parse(string raw)
    {
        // разделяем по первому символу '|'
        var parts = raw.Split('|', 2);

        return new NetworkMessage
        {
            Type = Enum.Parse<MessageType>(parts[0]),
            Payload = parts.Length > 1 ? parts[1] : string.Empty
        };
    }

    // Собирает сообщение обратно в строку для отправки
    public static string Serialize(NetworkMessage message)
    {
        return $"{message.Type}|{message.Payload}";
    }
}
