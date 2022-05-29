using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PS.Data
{
    public class BlackList
    {
        public int Id { get; set; }
        
        [Required]
        public string Url { get; set; }

        [NotMapped]
        public bool Saving { get; set; }
    }
}
