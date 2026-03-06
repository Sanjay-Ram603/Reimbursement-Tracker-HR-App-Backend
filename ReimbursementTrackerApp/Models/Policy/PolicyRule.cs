using ReimbursementTrackerApp.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ReimbursementTrackerApp.Models.Policy
{
    public class PolicyRule
    {
        [Key]
        public Guid PolicyRuleId { get; set; }

        [Required]
        public string RuleName { get; set; } = string.Empty;

        [Required]
        public PolicyRuleType RuleType { get; set; }

        public decimal? MaximumAmount { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
