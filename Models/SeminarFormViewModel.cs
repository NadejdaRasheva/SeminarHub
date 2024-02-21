using SeminarHub.Data;
using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.DataConstants;

namespace SeminarHub.Models
{
    public class SeminarFormViewModel
    {
        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(SeminarTopicMaxLength, MinimumLength = SeminarTopicMinLength)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(LecturerNameMaxLength, MinimumLength = LecturerNameMinLength)]
        public string Lecturer { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        [StringLength(SeminarDetailsMaxLength, MinimumLength = SeminarDetailsMinLength)]
        public string Details { get; set; } = string.Empty;

        [Required(ErrorMessage = RequiredErrorMessage)]
        public string DateAndTime { get; set; } = string.Empty;

        [Range(SeminarDurationMinValue, SeminarDurationMaxValue)]
        public int Duration { get; set; }

        [Required(ErrorMessage = RequiredErrorMessage)]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
