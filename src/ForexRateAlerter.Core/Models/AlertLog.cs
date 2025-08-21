namespace ForexRateAlerter.Core.Models
{
    public class AlertLog
    {
        public int Id { get; set; }
        
        public int AlertId { get; set; }
        
        public decimal TriggeredRate { get; set; }
        
        public decimal TargetRate { get; set; }
        
        public AlertCondition Condition { get; set; }
        
        public DateTime TriggeredAt { get; set; } = DateTime.UtcNow;
        
        public bool EmailSent { get; set; }
        
        public string? EmailError { get; set; }
        
        // Navigation properties
        public virtual Alert Alert { get; set; } = null!;
    }
}
