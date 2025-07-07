using NodaTime;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace portal_agile.Models
{
    public class Timesheet
    {
        [Key]
        public Guid TimesheetId { get; set; } = Guid.NewGuid();

        [Required]
        public required int TenantId { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required int Year { get; set; } = DateTime.UtcNow.Year;

        [Required]
        [Range(1, 12)]
        public required int Month { get; set; }

        [Required]
        [Range(1, 31)]
        public int Day { get; set; }

        [Required]
        public DateTime TimeIn { get; set; }

        public DateTime? TimeOut { get; set; }

        [Required]
        [StringLength(50)]
        public string TimeZone { get; set; } = "Asia/Manila";

        [StringLength(500)]
        public string? WorkDone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        // Computed properties
        [NotMapped]
        public DateTime WorkDate
        {
            get => new DateTime(Year, Month, Day);
        }

        // Hours worked as decimal (2.5 = 2 hrs 30 mins)
        [NotMapped]
        public decimal? TotalHours
        {
            get
            {
                if (!TimeOut.HasValue) return null;
                var timeSpan = TimeOut.Value - TimeIn;
                return (decimal)timeSpan.TotalHours;
            }
        }

        // Formatted display (e.g., "2.5 hours")
        [NotMapped]
        public string? TotalHoursDisplay
        {
            get
            {
                if (!TotalHours.HasValue) return null;
                return $"{TotalHours.Value:F1} hours";
            }
        }

        // Display time in user's timezone
        [NotMapped]
        public DateTime DisplayTimeIn
        {
            get
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
                return TimeZoneInfo.ConvertTimeFromUtc(TimeIn, timeZone);
            }
        }

        [NotMapped]
        public DateTime? DisplayTimeOut
        {
            get
            {
                if (!TimeOut.HasValue) return null;
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
                return TimeZoneInfo.ConvertTimeFromUtc(TimeOut.Value, timeZone);
            }
        }
    }
}
