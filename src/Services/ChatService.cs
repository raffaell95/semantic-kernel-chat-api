using ApiAi.Data;
using ApiAi.Dto;
using ApiAi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ApiAi.Services;

public class ChatService
{
    private readonly ChatDbContext _db;
    private readonly IChatCompletionService _chat;
    private readonly ILogger<ChatService> _logger;

    public ChatService(ChatDbContext db, Kernel kernel, ILogger<ChatService> logger)
    {
        _db = db;
        _logger = logger;
        _chat = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<ChatResponse> CreateChatAsync(string userMessage)
    {
        var chat = new Chat
        {
            Description = await GenerateDescription(userMessage)
        };

        _db.Chats.Add(chat);
        await _db.SaveChangesAsync();

        _db.Messages.Add(new ChatMessage
        {
            ChatId = chat.Id,
            Sender = "USER",
            Content = userMessage
        });

        await _db.SaveChangesAsync();

        return new ChatResponse(
            chat.Id,
            chat.Description,
            chat.CreatedAt
        );
    }

    public async Task<ChatMessageResponse> ChatAsync(Guid chatId, string message)
    {
        var chat = await _db.Chats.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == chatId);
        if (chat == null) throw new Exception("Chat não encontrado");

        var history = new ChatHistory();

        history.AddSystemMessage(
            "Você é um assistente útil e objetivo."
        );

        var lastMessages = chat.Messages
            .OrderByDescending(m => m.Timestamp)
            .Take(10)
            .OrderBy(m => m.Timestamp);

        foreach (var msg in lastMessages)
        {
            if (msg.Sender == "USER")
                history.AddUserMessage(msg.Content);
            else
                history.AddAssistantMessage(msg.Content);
        }

        history.AddUserMessage(message);

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["chatId"] = chatId
        }))
        {
            _logger.LogInformation(
                "Chamando LLM com {Count} mensagens no histórico",
                history.Count
            );


            var settings = new OllamaPromptExecutionSettings
            {
                NumPredict = 300,
                Temperature = 0.7f
            };

            var response = await _chat.GetChatMessageContentAsync(history, settings);

            var answer = response.Content ?? string.Empty;

            _db.Messages.Add(new ChatMessage
            {
                ChatId = chatId,
                Sender = "USER",
                Content = message
            });

            var assistantMessage = new ChatMessage
            {
                ChatId = chatId,
                Sender = "ASSISTANT",
                Content = answer
            };

            _db.Messages.Add(assistantMessage);
            await _db.SaveChangesAsync();

            return new ChatMessageResponse(
                assistantMessage.Id,
                assistantMessage.Sender,
                assistantMessage.Content,
                assistantMessage.Timestamp
            );
        }
    }

    public async Task<List<Chat>> ListAllChats() =>
        await _db.Chats.OrderByDescending(c => c.CreatedAt).ToListAsync();

    public async Task<List<ChatMessageResponse>> GetMessages(Guid chatId) =>
        await _db.Messages
        .Where(m => m.ChatId == chatId)
        .OrderBy(m => m.Timestamp)
        .Select(m => new ChatMessageResponse(
            m.Id,
            m.Sender,
            m.Content,
            m.Timestamp
        ))
        .ToListAsync();

    private async Task<string> GenerateDescription(string userMessage)
    {
        var history = new ChatHistory();
        history.AddSystemMessage("Gere uma descrição curta para esta conversa.");
        history.AddUserMessage(userMessage);

        var response = await _chat.GetChatMessageContentAsync(history);
        return response.Content ?? "Nova conversa";
    }

}
