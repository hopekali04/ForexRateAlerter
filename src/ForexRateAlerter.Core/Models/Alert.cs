using System.ComponentModel.DataAnnotations;

namespace ForexRateAlerter.Core.Models
{
    public class Alert
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        [MaxLength(3)]
        public string BaseCurrency { get; set; } = string.Empty; // e.g., "USD"
        
        [Required]
        [MaxLength(3)]
        public string TargetCurrency { get; set; } = string.Empty; // e.g., "MWK"
        
        public AlertCondition Condition { get; set; }
        
        [Range(0.001, double.MaxValue)]
        public decimal TargetRate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastTriggeredAt { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual ICollection<AlertLog> AlertLogs { get; set; } = new List<AlertLog>();
    }

    public enum AlertCondition
    {
        GreaterThan = 1,
        LessThan = 2,
        EqualTo = 3
    }
}