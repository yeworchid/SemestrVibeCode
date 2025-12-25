using System.Text;

namespace Common.Services;

// Класс для преобразования сообщений в байты и обратно
// Используется для передачи данных по TCP
// Включает XOR-шифрование по статичному ключу
public static class ByteConverter
{
    private static readonly Encoding Encoding = Encoding.UTF8;
    
    // Статичный ключ шифрования для игры "Бойцы хлопковых плантаций 2"
    private static readonly byte[] EncryptionKey = Encoding.UTF8.GetBytes("Happy_New_Year_2_0_2_6_!!!");

    // XOR-шифрование/дешифрование (симметричное)
    private static byte[] XorCrypt(byte[] data)
    {
        var result = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            result[i] = (byte)(data[i] ^ EncryptionKey[i % EncryptionKey.Length]);
        }
        return result;
    }

    // Преобразует строку в массив байтов для отправки по сети
    // Добавляет в начало 4 байта с длиной сообщения,
    // чтобы получатель знал, сколько байт нужно прочитать
    // Сообщение шифруется XOR
    public static byte[] StringToBytes(string message)
    {
        var messageBytes = Encoding.GetBytes(message);
        var encryptedBytes = XorCrypt(messageBytes);
        var lengthBytes = BitConverter.GetBytes(encryptedBytes.Length);

        // формат: [4 байта длины][зашифрованное сообщение]
        var result = new byte[4 + encryptedBytes.Length];
        Buffer.BlockCopy(lengthBytes, 0, result, 0, 4);
        Buffer.BlockCopy(encryptedBytes, 0, result, 4, encryptedBytes.Length);

        return result;
    }

    // Простое преобразование байтов в строку (с дешифровкой)
    public static string BytesToString(byte[] buffer, int offset, int length)
    {
        var encrypted = new byte[length];
        Buffer.BlockCopy(buffer, offset, encrypted, 0, length);
        var decrypted = XorCrypt(encrypted);
        return Encoding.GetString(decrypted);
    }

    // TCP может передавать данные частями,
    // поэтому сначала читаем длину сообщения,
    // а потом само сообщение
    // Метод возвращает false, если сообщение пришло не полностью
    public static bool TryReadMessage(byte[] buffer, int offset, int available, out string message, out int bytesRead)
    {
        message = null!;
        bytesRead = 0;

        // проверяем, что пришли хотя бы 4 байта с длиной
        if (available < 4)
            return false;

        // читаем длину сообщения из первых 4 байт
        int messageLength = BitConverter.ToInt32(buffer, offset);

        // проверяем, что всё сообщение уже пришло
        if (available < 4 + messageLength)
            return false;

        // всё на месте, дешифруем и читаем сообщение
        var encrypted = new byte[messageLength];
        Buffer.BlockCopy(buffer, offset + 4, encrypted, 0, messageLength);
        var decrypted = XorCrypt(encrypted);
        message = Encoding.GetString(decrypted);
        bytesRead = 4 + messageLength;

        return true;
    }
}
