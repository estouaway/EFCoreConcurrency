namespace EFCoreConcurrency.Models
{
    public class ConcurrentWithRowVersion : Article
    {
        public byte[] Timestamp { get; set; }
    }
}
