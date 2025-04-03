using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace L02P02_2022PM650_2022SG650.Models
{
    public class autores
    {
        [Key]
        public int id { get; set; }
        public string? autor { get; set; }
    }
}
