﻿using Microsoft.AspNetCore.Http;
using FinancialServices.Api.Attributes;
using System;

namespace FinancialServices.Api.Endpoints.V1.Transaction
{
    [RouteBuilderConfiguration(Version = 1, Path = "/transaction", RouteGroup = "Transactions", Name = "Create Transaction Enpdoint")]
    public class PostTransactionEndpoint_v1 : IEndpoint
    {
        public RouteHandlerBuilder Map(IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder.MapPost("/", async (HttpContext httpContext) =>
            {
                return Results.Ok("PostTransactionEndpoint v1");
            })
                .Produces(StatusCodes.Status200OK);
               

        }

       
    }
}
