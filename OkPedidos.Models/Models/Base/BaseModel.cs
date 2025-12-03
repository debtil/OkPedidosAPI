using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OkPedidos.Models.Models.Base
{
    public abstract class BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyOrder(-99999)]
        public int Id { get; set; }

        [Required]
        [JsonPropertyOrder(99990)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyOrder(99992)]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyOrder(99994)]
        public DateTime? DeletedAt { get; set; }
    }
}
