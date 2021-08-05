using System;

namespace EFCoreConcurrency.Models
{
    public abstract class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long Version { get; set; }
    }
}
