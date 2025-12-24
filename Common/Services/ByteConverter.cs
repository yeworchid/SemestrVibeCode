using System.Text;

namespace Common.Services;

// Класс для преобразования сообщений в байты и обратно
// Используется для передачи данных по TCP
public static class ByteConverter
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    // Преобразует строку в массив байтов для отправки по сети
    // Добавляет в начало 4 байта с длиной сообщения,
    // чтобы получатель знал, сколько байт нужно прочитать
    public static byte[] StringToBytes(string message)
    {
        var messageBytes = Encoding.GetBytes(message);
        var lengthBytes = BitConverter.GetBytes(messageBytes.Length);

        // формат: [4 байта длины][сообщение]
        var result = new byte[4 + messageBytes.Length];
        Buffer.BlockCopy(lengthBytes, 0, result, 0, 4);
        Buffer.BlockCopy(messageBytes, 0, result, 4, messageBytes.Length);

        return result;
    }

    // Простое преобразование байтов в строку
    public static string BytesToString(byte[] buffer, int offset, int length)
    {
        return Encoding.GetString(buffer, offset, length);
    }

    // TCP может передавать данные частями,
    // поэтому сначала читаем длину сообщения,
    // а потом само сообщение
    // Метод возвращает false, если сообщение пришло не полностью
    public static bool TryReadMessage(byte[] buffer, int offset, int available, out string message, out int bytesRead)
    {
        message = null;
        bytesRead = 0;

        // проверяем, что пришли хотя бы 4 байта с длиной
        if (available < 4)
            return false;

        // читаем длину сообщения из первых 4 байт
        int messageLength = BitConverter.ToInt32(buffer, offset);

        // проверяем, что всё сообщение уже пришло
        if (available < 4 + messageLength)
            return false;

        // всё на месте, читаем сообщение
        message = Encoding.GetString(buffer, offset + 4, messageLength);
        bytesRead = 4 + messageLength;

        return true;
    }
}
