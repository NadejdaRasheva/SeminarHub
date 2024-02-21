using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SeminarHub.Data.DataConstants;

namespace SeminarHub.Data
{
    [Authorize]
    public class Seminar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(SeminarTopicMaxLength)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [MaxLength(LecturerNameMaxLength)]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [MaxLength(SeminarDetailsMaxLength)]
        public string Details { get; set; } = string.Empty;

        [Required]
        public string OrganizerId { get; set; } = string.Empty;

        [Required]
        public IdentityUser Organizer { get; set; } = null!;

        [Required]
        public DateTime DateAndTime { get; set; }

        [Range(SeminarDurationMinValue, SeminarDurationMaxValue)]
        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public IList<SeminarParticipant> SeminarsParticipants { get; set; } = new List<SeminarParticipant>();
    }
}
