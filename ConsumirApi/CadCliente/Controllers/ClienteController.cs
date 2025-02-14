using Microsoft.AspNetCore.Mvc;
using CadCliente.Models;
using CadCliente.Services;
using Microsoft.AspNetCore.Authorization;

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
                _logger.LogInformation("Iniciando busca de clientes");
                var clientes = await _apiService.GetClientesAsync();
                _logger.LogInformation("Clientes obtidos com sucesso. Total: {Count}", clientes?.Count() ?? 0);

                if (!clientes.Any())
                {
                    ViewBag.Message = "Nenhum cliente encontrado.";
                }
                return View(clientes);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Usuário não está autenticado. Redirecionando para login");
                return RedirectToAction("Login", "Auth");
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
                    _logger.LogInformation("Iniciando criação do cliente: {Nome}", cliente.Nome);
                    await _apiService.CreateClienteAsync(cliente);
                    _logger.LogInformation("Cliente criado com sucesso: {Nome}", cliente.Nome);
                    
                    TempData["SuccessMessage"] = "Cliente criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Usuário não está autenticado. Redirecionando para login");
                return RedirectToAction("Login", "Auth");
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
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Usuário não está autenticado. Redirecionando para login");
                return RedirectToAction("Login", "Auth");
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
                    _logger.LogInformation("Iniciando atualização do cliente: {Id}", id);
                    cliente.Id = id; // Garantir que o ID está correto
                    await _apiService.UpdateClienteAsync(id, cliente);
                    _logger.LogInformation("Cliente atualizado com sucesso: {Id}", id);
                    
                    TempData["SuccessMessage"] = "Cliente atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Usuário não está autenticado. Redirecionando para login");
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente: {Id}", id);
                ModelState.AddModelError("", $"Erro ao atualizar cliente: {ex.Message}");
            }

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando exclusão do cliente: {Id}", id);
                await _apiService.DeleteClienteAsync(id);
                _logger.LogInformation("Cliente excluído com sucesso: {Id}", id);
                
                TempData["SuccessMessage"] = "Cliente excluído com sucesso!";
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Usuário não está autenticado. Redirecionando para login");
                return RedirectToAction("Login", "Auth");
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
