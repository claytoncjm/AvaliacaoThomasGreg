using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CadCliente.Models
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Logo")]
        public IFormFile? LogotipoFile { get; set; }

        public byte[]? LogotipoBytes { get; set; }
        
        public string? LogotipoContentType { get; set; }

        public string? LogotipoBase64 
        { 
            get
            {
                if (LogotipoBytes == null || LogotipoContentType == null) return null;
                return $"data:{LogotipoContentType};base64,{Convert.ToBase64String(LogotipoBytes)}";
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    LogotipoBytes = null;
                    LogotipoContentType = null;
                }
                else
                {
                    try
                    {
                        var parts = value.Split(',');
                        if (parts.Length != 2) throw new Exception("Formato base64 inválido");

                        var header = parts[0];
                        var base64 = parts[1];

                        if (!header.StartsWith("data:") || !header.Contains(";base64"))
                            throw new Exception("Cabeçalho base64 inválido");

                        LogotipoContentType = header.Replace("data:", "").Replace(";base64", "");
                        LogotipoBytes = Convert.FromBase64String(base64);
                    }
                    catch
                    {
                        LogotipoBytes = null;
                        LogotipoContentType = null;
                    }
                }
            }
        }

        public List<LogradouroViewModel> Logradouros { get; set; } = new();
    }

    public class LogradouroViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório")]
        [StringLength(200, ErrorMessage = "O endereço deve ter no máximo 200 caracteres")]
        public string Endereco { get; set; } = string.Empty;
    }
}
