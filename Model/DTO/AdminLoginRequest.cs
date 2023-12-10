using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LiveWaitlistServer.Model.DTO
{
    public class AdminLoginRequest
    {
        [Required]
        [MaxLength(6)]
        public string WaitlistCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}