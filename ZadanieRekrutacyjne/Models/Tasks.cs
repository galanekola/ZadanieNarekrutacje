namespace ZadanieRekrutacyjne.Models;

public class Tasks
    {
        public virtual Guid Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual int Progress { get; set; }
        public virtual string Tag { get; set; }
        public virtual DateTime Deadline { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string Details { get; set; }
    }
