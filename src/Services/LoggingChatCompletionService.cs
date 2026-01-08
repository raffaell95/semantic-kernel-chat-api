using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace ApiAi.Services;

public sealed class LoggingChatCompletionService : IChatCompletionService
{
    private readonly IChatCompletionService _inner;
    private readonly ILogger<LoggingChatCompletionService> _logger;

    public LoggingChatCompletionService(
        IChatCompletionService inner,
        ILogger<LoggingChatCompletionService> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public IReadOnlyDictionary<string, object?> Attributes => throw new NotImplementedException();

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default)
    {
        LogRequest(chatHistory);

        var response = await _inner.GetChatMessageContentsAsync(
            chatHistory,
            executionSettings,
            kernel,
            cancellationToken
        );

        LogResponse(response);

        return response;
    }

    public async IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        LogRequest(chatHistory);

        await foreach (var chunk in _inner.GetStreamingChatMessageContentsAsync(
            chatHistory,
            executionSettings,
            kernel,
            cancellationToken))
        {
            if (!string.IsNullOrEmpty(chunk.Content))
            {
                _logger.LogInformation("LLM_STREAM_CHUNK: {Chunk}", chunk.Content);
            }

            yield return chunk;
        }
    }

    private void LogRequest(ChatHistory history)
    {
        _logger.LogInformation("===== LLM REQUEST =====");

        foreach (var msg in history)
        {
            _logger.LogInformation(
                "[{Role}] {Content}",
                msg.Role.Label,
                msg.Content
            );
        }
    }

    private void LogResponse(IReadOnlyList<ChatMessageContent> messages)
    {
        _logger.LogInformation("===== OLLAMA LLM RESPONSE =====");

        foreach (var msg in messages)
        {
            _logger.LogInformation(
                "[{Role}] {Content}",
                msg.Role.Label,
                msg.Content
            );

            // Modelo usado (Ollama retorna isso)
            if (!string.IsNullOrWhiteSpace(msg.ModelId))
            {
                _logger.LogInformation("Model: {Model}", msg.ModelId);
            }

            // Tamanho da resposta (útil como métrica)
            if (!string.IsNullOrEmpty(msg.Content))
            {
                _logger.LogInformation(
                    "Response length: {Chars} chars",
                    msg.Content.Length
                );
            }
        }
    }

}
