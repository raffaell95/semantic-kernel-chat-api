using ApiAi.Data;
using ApiAi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat API", Version = "v1" });
});

builder.Services.AddKernel();
builder.Services.AddOllamaChatCompletion(
    modelId: "llama3:8b-instruct-q4_0",
    endpoint: new Uri("http://localhost:11434")
);


builder.Services.Decorate<IChatCompletionService, LoggingChatCompletionService>();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddDbContext<ChatDbContext>(opt => 
    opt.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        new MySqlServerVersion(new Version(8, 0, 32))
    ));

builder.Services.AddScoped<ChatService>();


var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
    });
}

app.Run();