namespace Lib
{
    [Serializable]
    public record Message
    {
        public int Id { get; set; }
        public string? Text { get; set; }
        public MessageType MessageType { get; set; }
    }

    public enum MessageType
    {
        Warning,
        Error,
        Information,
    }
}
