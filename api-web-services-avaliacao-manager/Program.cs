using api_web_services_avaliacao_manager.Controllers;
using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o servi�o TMDBService para consumir a API externa
builder.Services.AddHttpClient<TMDBService>();  // Configura o HttpClient para TMDBService
builder.Services.AddScoped<TMDBService>();  // Registra TMDBService como um servi�o de escopo

// Configura��o de Controllers (n�o h� mais necessidade do DbContext)
builder.Services.AddControllers();

// Configura��o de Swagger para documenta��o da API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura��o do pipeline de requisi��o HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Habilita Swagger na vers�o de desenvolvimento
    app.UseSwaggerUI();  // UI do Swagger para visualiza��o da API
}

app.UseHttpsRedirection();  // Redirecionamento para HTTPS

app.UseAuthorization();  // Habilita a autoriza��o, caso necess�rio

app.MapControllers();  // Mapeia os controllers da API

app.Run();  // Inicia o servidor
