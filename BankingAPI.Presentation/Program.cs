using BankingAPI.Infrastructure.Data;
using BankingAPI.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBankingServices<BankingApiDbContext>(builder.Configuration,
    builder.Configuration["MySerilog:FileName"]!);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();