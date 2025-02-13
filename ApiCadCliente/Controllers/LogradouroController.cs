using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiCadCliente.Data;
using ApiCadCliente.Models;
using ApiCadCliente.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ApiCadCliente.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LogradouroController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LogradouroController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LogradouroDTO>> CreateLogradouro(LogradouroDTO logradouroDto)
        {
            var logradouro = new Logradouro
            {
                Endereco = logradouroDto.Endereco,
                ClienteId = logradouroDto.ClienteId
            };

            _context.Logradouros.Add(logradouro);
            await _context.SaveChangesAsync();

            logradouroDto.Id = logradouro.Id;
            return CreatedAtAction(nameof(GetLogradouro), new { id = logradouro.Id }, logradouroDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LogradouroDTO>> GetLogradouro(int id)
        {
            var logradouro = await _context.Logradouros.FindAsync(id);

            if (logradouro == null)
            {
                return NotFound();
            }

            var logradouroDto = new LogradouroDTO
            {
                Id = logradouro.Id,
                Endereco = logradouro.Endereco ?? string.Empty,
                ClienteId = logradouro.ClienteId
            };

            return logradouroDto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogradouroDTO>>> GetLogradouros()
        {
            return await _context.Logradouros
                .Select(l => new LogradouroDTO
                {
                    Id = l.Id,
                    Endereco = l.Endereco ?? string.Empty,
                    ClienteId = l.ClienteId
                })
                .ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLogradouro(int id, LogradouroDTO logradouroDto)
        {
            if (id != logradouroDto.Id)
            {
                return BadRequest();
            }

            var logradouro = await _context.Logradouros.FindAsync(id);
            if (logradouro == null)
            {
                return NotFound();
            }

            logradouro.Endereco = logradouroDto.Endereco;
            logradouro.ClienteId = logradouroDto.ClienteId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogradouroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogradouro(int id)
        {
            var logradouro = await _context.Logradouros.FindAsync(id);
            if (logradouro == null)
            {
                return NotFound();
            }

            _context.Logradouros.Remove(logradouro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogradouroExists(int id)
        {
            return _context.Logradouros.Any(e => e.Id == id);
        }
    }
}
