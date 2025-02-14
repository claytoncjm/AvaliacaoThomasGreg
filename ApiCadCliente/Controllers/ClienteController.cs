using ApiCadCliente.Data;
using ApiCadCliente.Models;
using ApiCadCliente.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCadCliente.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Logradouros)
                .Select(c => new ClienteDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Email = c.Email,
                    LogotipoBytes = c.LogotipoBytes,
                    LogotipoContentType = c.LogotipoContentType,
                    Logradouros = c.Logradouros.Select(l => new LogradouroDTO
                    {
                        Id = l.Id,
                        Endereco = l.Endereco,
                        ClienteId = l.ClienteId
                    }).ToList()
                })
                .ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Logradouros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var clienteDto = new ClienteDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                LogotipoBytes = cliente.LogotipoBytes,
                LogotipoContentType = cliente.LogotipoContentType,
                Logradouros = cliente.Logradouros.Select(l => new LogradouroDTO
                {
                    Id = l.Id,
                    Endereco = l.Endereco,
                    ClienteId = l.ClienteId
                }).ToList()
            };

            return Ok(clienteDto);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> CreateCliente([FromForm] ClienteCreateDTO clienteDto)
        {
            try
            {
                if (await _context.Clientes.AnyAsync(c => c.Email == clienteDto.Email))
                {
                    return BadRequest($"Já existe um cliente cadastrado com o email {clienteDto.Email}");
                }

                var cliente = new Cliente
                {
                    Nome = clienteDto.Nome,
                    Email = clienteDto.Email
                };

                if (clienteDto.LogotipoFile != null)
                {
                    using var ms = new MemoryStream();
                    await clienteDto.LogotipoFile.CopyToAsync(ms);
                    cliente.LogotipoBytes = ms.ToArray();
                    cliente.LogotipoContentType = clienteDto.LogotipoFile.ContentType;
                }

                // Adicionar os endereços
                if (clienteDto.Logradouros != null)
                {
                    foreach (var logradouroDto in clienteDto.Logradouros)
                    {
                        cliente.Logradouros.Add(new Logradouro
                        {
                            Endereco = logradouroDto.Endereco
                        });
                    }
                }

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                var createdClienteDto = new ClienteDTO
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    Email = cliente.Email,
                    LogotipoBytes = cliente.LogotipoBytes,
                    LogotipoContentType = cliente.LogotipoContentType,
                    Logradouros = cliente.Logradouros.Select(l => new LogradouroDTO
                    {
                        Id = l.Id,
                        Endereco = l.Endereco,
                        ClienteId = l.ClienteId
                    }).ToList()
                };

                return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, createdClienteDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar cliente: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromForm] ClienteUpdateDTO clienteDto)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Logradouros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            if (await _context.Clientes.AnyAsync(c => c.Email == clienteDto.Email && c.Id != id))
            {
                return BadRequest($"Já existe outro cliente cadastrado com o email {clienteDto.Email}");
            }

            cliente.Nome = clienteDto.Nome;
            cliente.Email = clienteDto.Email;

            if (clienteDto.RemoverLogotipo)
            {
                cliente.LogotipoBytes = null;
                cliente.LogotipoContentType = null;
            }
            else if (clienteDto.LogotipoFile != null)
            {
                using var ms = new MemoryStream();
                await clienteDto.LogotipoFile.CopyToAsync(ms);
                cliente.LogotipoBytes = ms.ToArray();
                cliente.LogotipoContentType = clienteDto.LogotipoFile.ContentType;
            }

            // Atualizar os endereços
            if (clienteDto.Logradouros != null)
            {
                // Remover endereços que não estão mais na lista
                var logradourosParaRemover = cliente.Logradouros
                    .Where(l => !clienteDto.Logradouros.Any(dto => dto.Id == l.Id))
                    .ToList();

                foreach (var logradouro in logradourosParaRemover)
                {
                    cliente.Logradouros.Remove(logradouro);
                }

                // Atualizar ou adicionar novos endereços
                foreach (var logradouroDto in clienteDto.Logradouros)
                {
                    var logradouro = cliente.Logradouros.FirstOrDefault(l => l.Id == logradouroDto.Id);
                    if (logradouro != null)
                    {
                        // Atualizar endereço existente
                        logradouro.Endereco = logradouroDto.Endereco;
                    }
                    else
                    {
                        // Adicionar novo endereço
                        cliente.Logradouros.Add(new Logradouro
                        {
                            Endereco = logradouroDto.Endereco
                        });
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar cliente: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Logradouros)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
            {
                return NotFound();
            }

            _context.Logradouros.RemoveRange(cliente.Logradouros);
            _context.Clientes.Remove(cliente);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao excluir cliente: {ex.Message}");
            }
        }
    }
}
