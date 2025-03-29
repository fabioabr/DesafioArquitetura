using FinancialServices.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddCustomApplicationSettingsConfiguration()
    .AddCustomLoggerConfiguration()
    .AddCustomApiConfiguration();
    
// Falta Redoc

    

var app = builder.Build();

app.UseApplicationEndpoints();

app.UseCustomSwagger();
app.UseCustomRedoc();




app.UseHttpsRedirection();

 

app.Run();
