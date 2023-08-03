using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWebRazor_Temp.Models
{
	public class Category
	{
        [Key]
        public int CategoryId { get; set; }
        [Required, DisplayName("Category Name")]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required, DisplayName("Display Order"), Range(1, 100)]
        public int DisplayOrder { get; set; }
    }
}

