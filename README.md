# Cadastro de Clientes

Sistema de cadastro de clientes desenvolvido em ASP.NET Core MVC que consome uma API REST.

## Funcionalidades

- Autenticação de usuários (Login/Registro)
- Listagem de clientes com carregamento dinâmico
- Cadastro de novos clientes
- Upload de logo com preview e validações
- Edição de clientes existentes
- Exclusão de clientes
- Gerenciamento de endereços múltiplos por cliente

## Tecnologias Utilizadas

- ASP.NET Core MVC
- Entity Framework Core
- Bootstrap 5
- jQuery
- Bootstrap Icons

## Configuração e Uso

### API (ApiCadCliente)

1. Configure a string de conexão no `appsettings.json`
2. Execute as migrations do banco de dados
3. Execute a API
4. Para testar a API:
   - Primeiro crie um usuário usando o endpoint de registro
   - Faça login para obter o token JWT
   - Para todas as outras requisições, use o token no header:
     - Adicione o header `Authorization: Bearer {seu-token-aqui}`
     - Substitua `{seu-token-aqui}` pelo token obtido no login

### Aplicação Web (CadCliente)

1. Configure a URL da API no `appsettings.json`
2. Execute a aplicação
3. Na tela de login:
   - Se for primeiro acesso, clique em "Criar Conta"
   - Registre-se com email e senha
   - Faça login com as credenciais criadas

## Estrutura do Projeto

- `Controllers/`: Controladores da aplicação
  - `AuthController.cs`: Gerencia autenticação
  - `ClienteController.cs`: Gerencia operações de clientes
- `Models/`: Classes de modelo
- `Views/`: Views da aplicação
- `Services/`: Serviços da aplicação
  - `ApiService.cs`: Serviço para comunicação com a API

## Melhorias Recentes

1. Interface do Usuário:
   - Adicionado preview de imagem no upload de logo
   - Validações em tempo real para upload de arquivos
   - Mensagens de carregamento e sucesso
   - Ícones do Bootstrap para melhor UX

2. Segurança:
   - Validação de token JWT
   - Proteção contra uploads maliciosos
   - Validação de tamanho e tipo de arquivo

3. Usabilidade:
   - Página inicial redirecionada para login
   - Mensagens de feedback mais claras
   - Animações suaves nas transições

## Como Usar

1. Acesse a aplicação
2. Na tela de login:
   - Se já tem conta: faça login com email e senha
   - Se é novo usuário: clique em "Criar Conta" e registre-se
3. Na lista de clientes, você pode:
   - Ver todos os clientes cadastrados
   - Adicionar novo cliente
   - Editar cliente existente
   - Excluir cliente
   - Gerenciar endereços
   - Fazer upload de logo

## Observações

- O tamanho máximo para upload de logo é 2MB
- Formatos aceitos: JPG, PNG, GIF
- É necessário estar autenticado para acessar as funcionalidades
- Para testar a API diretamente, lembre-se de incluir o token JWT com o prefixo "Bearer" no header Authorization
