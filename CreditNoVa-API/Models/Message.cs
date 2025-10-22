using EFund_API.WebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFund_API.Models
{
    public class Message : Entity<Guid>
    {

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required, MaxLength(20)]
        public string SenderType { get; set; }

        [Required]
        public string Content { get; set; }

        public bool IsRead { get; set; } = false;

        public Guid? SessionId { get; set; }

        public User User { get; set; }
    }
}
