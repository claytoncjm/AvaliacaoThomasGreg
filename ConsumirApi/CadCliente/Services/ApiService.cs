using CadCliente.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CadCliente.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private string _token;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiService> _logger;

        public ApiService(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration.GetValue<string>("ApiSettings:BaseUrl");
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private void EnsureTokenIsSet()
        {
            if (string.IsNullOrEmpty(_token))
            {
                var token = _httpContextAccessor.HttpContext?.User?.FindFirst("JwtToken")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    SetAuthToken(token);
                }
                else
                {
                    throw new InvalidOperationException("Token não configurado.");
                }
            }
        }

        public void SetAuthToken(string token)
        {
            _token = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task ProcessLogoFile(ClienteViewModel cliente)
        {
            try
            {
                _logger.LogInformation("Iniciando processamento do logo para cliente {Nome}", cliente.Nome);

                if (cliente.LogotipoFile != null && cliente.LogotipoFile.Length > 0)
                {
                    _logger.LogInformation("Arquivo recebido - Nome: {FileName}, Tipo: {ContentType}, Tamanho: {Length} bytes",
                        cliente.LogotipoFile.FileName,
                        cliente.LogotipoFile.ContentType,
                        cliente.LogotipoFile.Length);

                    // Verificar o tipo do arquivo
                    var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp" };
                    var contentType = cliente.LogotipoFile.ContentType.ToLower();
                    
                    if (!allowedTypes.Contains(contentType))
                    {
                        var error = $"Tipo de arquivo não permitido: {contentType}. Tipos permitidos: {string.Join(", ", allowedTypes)}";
                        _logger.LogError(error);
                        throw new Exception(error);
                    }

                    // Verificar o tamanho (2MB)
                    if (cliente.LogotipoFile.Length > 2 * 1024 * 1024)
                    {
                        var error = $"Arquivo muito grande: {cliente.LogotipoFile.Length} bytes. Máximo permitido: {2 * 1024 * 1024} bytes";
                        _logger.LogError(error);
                        throw new Exception(error);
                    }

                    using var ms = new MemoryStream();
                    await cliente.LogotipoFile.CopyToAsync(ms);
                    cliente.LogotipoBytes = ms.ToArray();
                    cliente.LogotipoContentType = contentType;

                    _logger.LogInformation("Logo processado com sucesso. Tamanho em bytes: {Length}", cliente.LogotipoBytes.Length);
                }
                else
                {
                    _logger.LogInformation("Nenhum arquivo de logo novo foi enviado");
                    
                    // Se não houver novo arquivo, manter os bytes existentes
                    if (cliente.LogotipoBytes == null)
                    {
                        cliente.LogotipoBytes = null;
                        cliente.LogotipoContentType = null;
                        _logger.LogInformation("Logo removido");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar logo: {Message}", ex.Message);
                throw new Exception($"Erro ao processar o logo: {ex.Message}");
            }
        }

        public async Task<string> LoginAsync(LoginViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}Auth/login", model);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Resposta da API - StatusCode: {StatusCode}, Content: {Content}",
                    response.StatusCode,
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(content);
                }

                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                var token = result?["token"] ?? string.Empty;
                SetAuthToken(token);
                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login: {Message}", ex.Message);
                throw new Exception($"Erro ao fazer login: {ex.Message}");
            }
        }

        public async Task<string> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}Auth/register", model);
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Resposta da API - StatusCode: {StatusCode}, Content: {Content}",
                    response.StatusCode,
                    content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(content);
                }

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário: {Message}", ex.Message);
                throw new Exception($"Erro ao registrar usuário: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ClienteViewModel>> GetClientesAsync()
        {
            try
            {
                EnsureTokenIsSet();
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Cliente");
                
                _logger.LogInformation("Resposta da API - StatusCode: {StatusCode}",
                    response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar clientes: {errorContent}");
                }

                var clientes = await response.Content.ReadFromJsonAsync<IEnumerable<ClienteViewModel>>();
                return clientes ?? new List<ClienteViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar clientes: {Message}", ex.Message);
                throw new Exception($"Erro ao buscar clientes: {ex.Message}");
            }
        }

        public async Task<ClienteViewModel> GetClienteByIdAsync(int id)
        {
            try
            {
                EnsureTokenIsSet();
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}Cliente/{id}");
                
                _logger.LogInformation("Resposta da API - StatusCode: {StatusCode}",
                    response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao buscar cliente: {errorContent}");
                }

                var cliente = await response.Content.ReadFromJsonAsync<ClienteViewModel>();
                return cliente ?? throw new Exception("Cliente não encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cliente: {Message}", ex.Message);
                throw new Exception($"Erro ao buscar cliente: {ex.Message}");
            }
        }

        public async Task CreateClienteAsync(ClienteViewModel cliente)
        {
            try
            {
                EnsureTokenIsSet();
                await ProcessLogoFile(cliente);

                // Criar um formulário multipart
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(cliente.Nome), "Nome");
                form.Add(new StringContent(cliente.Email), "Email");

                // Adicionar o arquivo se existir
                if (cliente.LogotipoFile != null)
                {
                    var fileContent = new StreamContent(cliente.LogotipoFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(cliente.LogotipoFile.ContentType);
                    form.Add(fileContent, "LogotipoFile", cliente.LogotipoFile.FileName);
                }

                // Adicionar os endereços
                if (cliente.Logradouros != null)
                {
                    for (int i = 0; i < cliente.Logradouros.Count; i++)
                    {
                        form.Add(new StringContent(cliente.Logradouros[i].Endereco), $"Logradouros[{i}].Endereco");
                    }
                }

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}Cliente", form);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Resposta da API - StatusCode: {StatusCode}, Content: {Content}",
                    response.StatusCode,
                    responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    if (responseContent.Contains("duplicate key") && responseContent.Contains("IX_Clientes_Email"))
                    {
                        throw new Exception($"Já existe um cliente cadastrado com o email {cliente.Email}");
                    }
                    throw new Exception($"Erro ao criar cliente: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente: {Message}", ex.Message);
                throw new Exception($"Erro ao criar cliente: {ex.Message}");
            }
        }

        public async Task UpdateClienteAsync(int id, ClienteViewModel cliente)
        {
            try
            {
                _logger.LogInformation("Iniciando atualização do cliente {Id}", id);
                
                EnsureTokenIsSet();
                await ProcessLogoFile(cliente);

                // Criar um formulário multipart
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(cliente.Id.ToString()), "Id");
                form.Add(new StringContent(cliente.Nome), "Nome");
                form.Add(new StringContent(cliente.Email), "Email");

                // Adicionar o arquivo se existir
                if (cliente.LogotipoFile != null)
                {
                    var fileContent = new StreamContent(cliente.LogotipoFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(cliente.LogotipoFile.ContentType);
                    form.Add(fileContent, "LogotipoFile", cliente.LogotipoFile.FileName);
                }

                // Adicionar os endereços
                if (cliente.Logradouros != null)
                {
                    for (int i = 0; i < cliente.Logradouros.Count; i++)
                    {
                        if (cliente.Logradouros[i].Id > 0)
                            form.Add(new StringContent(cliente.Logradouros[i].Id.ToString()), $"Logradouros[{i}].Id");
                        form.Add(new StringContent(cliente.Logradouros[i].Endereco), $"Logradouros[{i}].Endereco");
                    }
                }

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}Cliente/{id}", form);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Resposta da API - StatusCode: {StatusCode}, Content: {Content}",
                    response.StatusCode,
                    responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    if (responseContent.Contains("duplicate key") && responseContent.Contains("IX_Clientes_Email"))
                    {
                        throw new Exception($"Já existe outro cliente cadastrado com o email {cliente.Email}");
                    }

                    // Tentar deserializar o erro da API
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                        if (errorResponse != null && errorResponse.ContainsKey("message"))
                        {
                            throw new Exception($"Erro da API: {errorResponse["message"]}");
                        }
                    }
                    catch
                    {
                        // Se não conseguir deserializar, usa a mensagem completa
                        throw new Exception($"Erro ao atualizar cliente. Resposta da API: {responseContent}");
                    }
                }

                _logger.LogInformation("Cliente {Id} atualizado com sucesso", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar cliente {Id}: {Message}", id, ex.Message);
                throw new Exception($"Erro ao atualizar cliente: {ex.Message}", ex);
            }
        }

        public async Task DeleteClienteAsync(int id)
        {
            try
            {
                EnsureTokenIsSet();
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}Cliente/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Resposta da API - StatusCode: {StatusCode}, Content: {Content}",
                    response.StatusCode,
                    responseContent);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro ao excluir cliente: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir cliente: {Message}", ex.Message);
                throw new Exception($"Erro ao excluir cliente: {ex.Message}");
            }
        }
    }
}
