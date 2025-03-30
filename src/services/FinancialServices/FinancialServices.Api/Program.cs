using FinancialServices.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder

    .AddCustomApiConfiguration();
  
var app = builder.Build();



app.UseCustomApiConfiguration();

// Existe um Seed que preenche o banco de dados com os usuarios de teste iniciais
// /src/Database/Seeds/mongodb-seed-script.js

app.Run();
