namespace SeminarHub.Data
{
    public class DataConstants
    {
        public const int SeminarTopicMinLength = 3;
        public const int SeminarTopicMaxLength = 100;

        public const int LecturerNameMinLength = 5;
        public const int LecturerNameMaxLength = 60;

        public const int SeminarDetailsMinLength = 10;
        public const int SeminarDetailsMaxLength = 500;

        public const string DateFormat = "dd/MM/yyyy HH:mm";

        public const int SeminarDurationMinValue = 30;
        public const int SeminarDurationMaxValue = 180;

        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 50;

        public const string RequiredErrorMessage = "Field {0} is required";
        public const string InvalidLengthErrorMessage = "Field {0} must be between {2} and {1} characters";
    }
}
