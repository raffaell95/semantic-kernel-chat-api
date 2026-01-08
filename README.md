# Chat API com Semantic Kernel (.NET) + Ollama

Esta é uma API de chat construída com **.NET 10**, utilizando o **Semantic Kernel** para integração com LLMs. Inicialmente desenvolvida com OpenAI, agora adaptada para usar **Ollama** localmente via Docker.  

O projeto permite criar chats, enviar mensagens, recuperar histórico e gerar respostas com LLM, mantendo registro no banco de dados MySQL.

---

## Funcionalidades

- Criar uma nova conversa (`/chats/start`)  
- Enviar mensagem para um chat existente (`/chats/{chatId}`)  
- Listar todos os chats (`/chats`)  
- Recuperar histórico de mensagens de um chat (`/chats/{chatId}/messages`)  
- Integração LLM com logs detalhados (tokens, mensagens, roles)  

---

## Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| POST   | `/chats/start` | Cria um novo chat. Recebe `message` no body. Retorna `chatId` e descrição do chat. |
| POST   | `/chats/{chatId}` | Envia uma mensagem para o chat existente e recebe a resposta do assistente. |
| GET    | `/chats` | Lista todos os chats com suas descrições e timestamps. |
| GET    | `/chats/{chatId}/messages` | Retorna o histórico de mensagens do chat especificado. |

**Exemplo de request para iniciar chat:**  
```json
POST /chats/start
{
    "message": "Olá, quero iniciar uma conversa"
}
```

## Tecnologias utilizadas
- .NET Core 10
- Entity Framework Core para persistência 9
- Semantic Kernel (Microsoft)
- Ollama para LLM local
- MySQL (Docker)

## Setup do projeto

### 1. Subir containers
`docker compose up -d`

### 2. Carregar modelo desejado
`docker exec -it ollama ollama pull llama3:8b-instruct-q4_0`

### 3. Rodar migrations e atualizar banco
`dotnet ef migrations add InitialCreate`
`dotnet ef database update`

### 4. Executar a aplicação
`dotnet run`

### A aplicação deve rodar em uma porta `http://localhost:<porta>/swagger/index.html`
Exemplo: `http://localhost:5034/swagger/index.html`
