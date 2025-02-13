using Microsoft.AspNetCore.Mvc;
using CadCliente.Models;
using CadCliente.Services;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace CadCliente.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(ApiService apiService, ILogger<ClienteController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var clientes = await _apiService.GetClientesAsync();
                if (!clientes.Any())
                {
                    ViewBag.Message = "Carregando clientes..."; 
                }
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes");
                TempData["Error"] = $"Erro ao buscar clientes: {ex.Message}";
                return View(new List<ClienteViewModel>());
            }
        }

        public IActionResult Create()
        {
            return View(new ClienteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Ponto de breakpoint #1 - Antes de processar
                    _logger.LogInformation("Iniciando criação do cliente: {Nome}", cliente.Nome);
                    
                    await _apiService.CreateClienteAsync(cliente);
                    
                    // Ponto de breakpoint #2 - Após processar
                    _logger.LogInformation("Cliente criado com sucesso: {Nome}", cliente.Nome);
                    
                    TempData["SuccessMessage"] = "Cliente criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente: {Nome}", cliente.Nome);
                ModelState.AddModelError("", $"Erro ao criar cliente: {ex.Message}");
            }

            return View(cliente);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var cliente = await _apiService.GetClienteByIdAsync(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente para edição: {Id}", id);
                TempData["Error"] = $"Erro ao buscar cliente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Ponto de breakpoint #3 - Antes de atualizar
                    _logger.LogInformation("Iniciando atualização do cliente: {Id}", id);
                    
                    // Log do tipo e tamanho da imagem
                    if (cliente.LogotipoFile != null)
                    {
                        _logger.LogInformation(
                            "Dados da imagem - Nome: {FileName}, Tipo: {ContentType}, Tamanho: {Length} bytes",
                            cliente.LogotipoFile.FileName,
                            cliente.LogotipoFile.ContentType,
                            cliente.LogotipoFile.Length
                        );
                    }

                    await _apiService.UpdateClienteAsync(id, cliente);
                    
                    // Ponto de breakpoint #4 - Após atualizar
                    _logger.LogInformation("Cliente atualizado com sucesso: {Id}", id);
                    
                    TempData["SuccessMessage"] = "Cliente atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente: {Id}", id);
                ModelState.AddModelError("", $"Erro ao atualizar cliente: {ex.Message}");
            }

            return View(cliente);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var cliente = await _apiService.GetClienteByIdAsync(id);
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente para detalhes: {Id}", id);
                TempData["Error"] = $"Erro ao buscar cliente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var cliente = await _apiService.GetClienteByIdAsync(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente para exclusão: {Id}", id);
                TempData["Error"] = $"Erro ao buscar cliente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiService.DeleteClienteAsync(id);
                TempData["Success"] = "Cliente excluído com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente: {Id}", id);
                TempData["Error"] = $"Erro ao excluir cliente: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
