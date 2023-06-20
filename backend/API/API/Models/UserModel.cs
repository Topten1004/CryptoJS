using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("User")]
    public class UserModel
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Column("UserName")]

        public string UserName { get; set; } = string.Empty;

        [Column("Password")]
        public string Password { get; set; } = string.Empty;

        [Column("LastModifiedDate")]
        public DateTime Lastmodifieddate { get; set; }
    }
}
