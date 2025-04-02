using Microsoft.AspNetCore.Http;
using FinancialServices.Api.Attributes;
using FinancialServices.Api.Contract;

namespace FinancialServices.Api.Endpoints.V2.Transaction
{
    [RouteBuilderConfiguration(Version = 2, Path = "/transaction", RouteGroup = "Transactions", Name = "Create Transaction Enpdoint")]
    public class CreateTransactionEndpoint : IEndpoint
    {
        public RouteHandlerBuilder Map(IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder.MapPost("/", (HttpContext httpContext) =>
            {
                return Results.Ok("CreateTransactionEndpoint v2");
            })
                .RequireAuthorization("PremiumUsers")
                .Produces(StatusCodes.Status200OK)
                ;
                 
        }

    }

}
