using Microsoft.AspNetCore.Http;
using FinancialServices.Api.Attributes;
using FinancialServices.Api.Contract;

namespace FinancialServices.Api.Endpoints.V2.Transaction
{
    [RouteBuilderConfiguration(Version = 2, Path = "/transaction", RouteGroup = "Transactions", Name = "Create Transaction Enpdoint")]
    public class PostTransactionEndpoint_v2 : IEndpoint
    {
        public RouteHandlerBuilder Map(IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder.MapPost("/", (HttpContext httpContext) =>
            {
                return Results.Ok("PostTransactionEndpoint v2");
            })
                .RequireAuthorization("PremiumUsers")
                .Produces(StatusCodes.Status200OK)
                ;
                 
        }

    }

}
