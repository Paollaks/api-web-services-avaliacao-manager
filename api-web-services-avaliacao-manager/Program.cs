using api_web_services_avaliacao_manager.Controllers;
using api_web_services_avaliacao_manager.Models;
using api_web_services_avaliacao_manager.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
{

}

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(connectionString));


builder.Services.AddHttpClient<TMDBService>();  
builder.Services.AddScoped<TMDBService>();  
builder.Services.AddDbContext<AppDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  
    app.UseSwaggerUI();  
}

app.UseHttpsRedirection();  

app.UseAuthorization();  

app.MapControllers();  

app.Run();  
