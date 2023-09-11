namespace Lib
{
    [Serializable]
    public class FactoryMessage
    {
        public int MessageNumber { get; set; }
        public int Duration { get; set; }
    
        public FactoryMessage(int messageNumber, int duration)
        {
            this.MessageNumber = messageNumber;
            this.Duration = duration;
        }
    }
}