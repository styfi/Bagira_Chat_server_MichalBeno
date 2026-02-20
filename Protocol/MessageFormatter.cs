namespace Protocol
{
    public class MessageFormatter
    {
        public static string Format(string message)
        {
            return $"{DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}, {message}";
        }
    }
}
