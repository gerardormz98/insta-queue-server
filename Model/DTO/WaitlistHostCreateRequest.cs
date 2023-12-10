using System.ComponentModel.DataAnnotations;
using LiveWaitlistServer.Model.DTO.Validators;

namespace LiveWaitlistServer.Model.DTO
{
    public class WaitlistHostCreateRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string HostName { get; set; } = "";

        [Required]
        [MinLength(20)]
        [MaxLength(10000)]
        public string HostDescription { get; set; } = "";

        [Required]
        [MinLength(6)]
        [MaxLength(100)]
        [PasswordStrengthValidation]
        public string Password { get; set; } = "";
    }
}