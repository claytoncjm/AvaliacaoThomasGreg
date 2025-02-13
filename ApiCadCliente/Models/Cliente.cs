using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCadCliente.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? LogotipoBytes { get; set; }

        [StringLength(100)]
        public string? LogotipoContentType { get; set; }

        public virtual ICollection<Logradouro> Logradouros { get; set; } = new List<Logradouro>();
    }
}
