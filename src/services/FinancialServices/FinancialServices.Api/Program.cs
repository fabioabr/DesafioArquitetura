using FinancialServices.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddCustomApplicationSettingsConfiguration()
    .AddCustomLoggerConfiguration()
    .AddCustomApiConfiguration();
  
var app = builder.Build();

app.UseApplicationEndpoints();

app.UseCustomSwagger();
//app.UseCustomRedoc(); 
app.UseHttpsRedirection();



app.Run();
