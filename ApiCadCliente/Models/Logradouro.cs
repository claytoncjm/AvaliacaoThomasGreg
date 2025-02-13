using System.ComponentModel.DataAnnotations;

namespace ApiCadCliente.Models
{
    public class Logradouro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; } = string.Empty;

        public int ClienteId { get; set; }
    }
}
