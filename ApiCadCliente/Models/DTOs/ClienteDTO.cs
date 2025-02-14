using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ApiCadCliente.Models.DTOs
{
    public class ClienteDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[]? LogotipoBytes { get; set; }
        public string? LogotipoContentType { get; set; }

        public string? LogotipoBase64
        {
            get
            {
                if (LogotipoBytes == null || LogotipoContentType == null)
                    return null;

                return $"data:{LogotipoContentType};base64,{Convert.ToBase64String(LogotipoBytes)}";
            }
        }

        public List<LogradouroDTO> Logradouros { get; set; } = new();
    }

    public class ClienteCreateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        public IFormFile? LogotipoFile { get; set; }

        public List<LogradouroCreateDTO> Logradouros { get; set; } = new();
    }

    public class ClienteUpdateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        public IFormFile? LogotipoFile { get; set; }
        public bool RemoverLogotipo { get; set; }

        public List<LogradouroUpdateDTO> Logradouros { get; set; } = new();
    }

    public class LogradouroDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; } = string.Empty;
        public int ClienteId { get; set; }
    }

    public class LogradouroCreateDTO
    {
        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; } = string.Empty;
    }

    public class LogradouroUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; } = string.Empty;
    }
}
