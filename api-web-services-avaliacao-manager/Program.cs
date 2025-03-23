using api_web_services_avaliacao_manager.Controllers;
using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o serviço TMDBService para consumir a API externa
builder.Services.AddHttpClient<TMDBService>();  // Configura o HttpClient para TMDBService
builder.Services.AddScoped<TMDBService>();  // Registra TMDBService como um serviço de escopo

// Configuração de Controllers (não há mais necessidade do DbContext)
builder.Services.AddControllers();

// Configuração de Swagger para documentação da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do pipeline de requisição HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Habilita Swagger na versão de desenvolvimento
    app.UseSwaggerUI();  // UI do Swagger para visualização da API
}

app.UseHttpsRedirection();  // Redirecionamento para HTTPS

app.UseAuthorization();  // Habilita a autorização, caso necessário

app.MapControllers();  // Mapeia os controllers da API

app.Run();  // Inicia o servidor
