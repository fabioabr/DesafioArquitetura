using Microsoft.AspNetCore.Http;
using FinancialServices.Api.Attributes;
using System;
using FinancialServices.Api.Contract;
using Microsoft.AspNetCore.Mvc;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Model;

namespace FinancialServices.Api.Endpoints.V1.Transaction
{
    [RouteBuilderConfiguration(Version = 1, Path = "/transaction", RouteGroup = "Transactions", Name = "Create Transaction Enpdoint")]
    public class CreateTransactionEndpoint : IEndpoint
    {
        public RouteHandlerBuilder Map(IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder.MapPost("/", (HttpContext httpContext, [FromServices] ICreateTransactionUseCase createTransactionUseCase, [FromBody] TransactionModel transaction) =>
            {
                try
                {

                    createTransactionUseCase.CreateTransaction(transaction);                    
                    
                    return Results.Ok("Transaction Created");

                }
                catch(InvalidDataException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
                 
            })
                .RequireAuthorization("PremiumUsers")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status500InternalServerError)
                ;
               

        }

       
    }
}
