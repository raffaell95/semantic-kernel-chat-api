using ApiAi.Dto;
using ApiAi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiAi.Controllers;

[ApiController]
[Route("api/chat-memory")]
public class ChatController : ControllerBase
{
    private readonly ChatService _chatService;

    public ChatController(ChatService chatService) => _chatService = chatService;


    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] ChatRequest request)
    {
        var chat = await _chatService.CreateChatAsync(request.Message);
        return Ok(chat);
    }

     [HttpPost("{chatId}")]
    public async Task<IActionResult> Chat(Guid chatId, [FromBody] ChatRequest req)
    {
        var msg = await _chatService.ChatAsync(chatId, req.Message);
        return Ok(msg);
    }

    [HttpGet]
    public async Task<IActionResult> List() =>
        Ok(await _chatService.ListAllChats());

    [HttpGet("{chatId}")]
    public async Task<IActionResult> History(Guid chatId) =>
        Ok(await _chatService.GetMessages(chatId));
}
