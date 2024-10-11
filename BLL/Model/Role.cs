using System.ComponentModel.DataAnnotations;

namespace JwtImplementation.BLL.Model
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; } = "User";
    }
}
