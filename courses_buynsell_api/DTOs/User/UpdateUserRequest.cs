using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace courses_buynsell_api.DTOs.User
{
    [JsonObject(MemberSerialization.OptIn)]
    public class UpdateUserRequest : IValidatableObject
    {
        [JsonProperty("fullName")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }

        [JsonProperty("email")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email must be less than 100 characters.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "Invalid email format (e.g. name@example.com).")]
        public string? Email { get; set; }

        // ✅ Custom validation logic:
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FullName) && string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult(
                    "At least one of FullName or Email must be provided.",
                    new[] { nameof(FullName), nameof(Email) }
                );
            }
        }
    }
}
