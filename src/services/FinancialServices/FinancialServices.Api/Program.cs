using FinancialServices.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.AddCustomApiConfiguration();
var app = builder.Build();
app.UseCustomApiConfiguration();
 





app.Run();


// Existe um Seed que preenche o banco de dados com os usuarios de teste iniciais
// \src\docker-compose\scripts\mongodb\seed.js
