using System.ComponentModel.DataAnnotations;

namespace LiveWaitlistServer.Model.DTO
{
    public class WaitlistHostUpdateRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string HostName { get; set; } = "";

        [Required]
        [MinLength(20)]
        [MaxLength(10000)]
        public string HostDescription { get; set; } = "";
    }
}