using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie_Project.Models
{
    public class Favorite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }
        public string Movie_Name { get; set; }

        public int? User_Id { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

    }
}
