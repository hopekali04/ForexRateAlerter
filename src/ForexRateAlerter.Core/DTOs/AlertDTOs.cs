using System.ComponentModel.DataAnnotations;
using ForexRateAlerter.Core.Models;

namespace ForexRateAlerter.Core.DTOs
{
    public class CreateAlertDto
    {
        [Required]
        [MaxLength(3)]
        [MinLength(3)]
        public string BaseCurrency { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(3)]
        [MinLength(3)]
        public string TargetCurrency { get; set; } = string.Empty;
        
        public AlertCondition Condition { get; set; }
        
        [Range(0.001, double.MaxValue)]
        public decimal TargetRate { get; set; }
    }

    public class UpdateAlertDto
    {
        public AlertCondition? Condition { get; set; }
        
        [Range(0.001, double.MaxValue)]
        public decimal? TargetRate { get; set; }
        
        public bool? IsActive { get; set; }
    }

    public class AlertResponseDto
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; } = string.Empty;
        public string TargetCurrency { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public decimal TargetRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
    }
}
