using Microsoft.AspNetCore.Http;
using FinancialServices.Api.Attributes;
using System;
using FinancialServices.Api.Contract;
using Microsoft.AspNetCore.Mvc;
using FinancialServices.Domain.Financial.Contract;
using FinancialServices.Domain.Financial.Model;

namespace FinancialServices.Api.Endpoints.V1.Transaction
{
    [RouteBuilderConfiguration(Version = 1, Path = "/daily-report", RouteGroup = "Reports", Name = "Get Daily Consolidated Report")]
    public class GetConsolidatedDataEndpoint : IEndpoint
    {
        public RouteHandlerBuilder Map(IEndpointRouteBuilder routeBuilder)
        {
            return routeBuilder.MapGet("/{date}", (HttpContext httpContext, [FromServices] IGetConsolidatedReportUseCase getConsolidatedReportUseCase, [FromRoute] DateTime date, [FromHeader(Name = "X-Timezone")] string? timezone) =>
            {
                try
                {

                    var tz = TimeZoneInfo.FindSystemTimeZoneById("UTC");
                    if (!string.IsNullOrEmpty(timezone))
                        TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out tz);

                    var utcDateParam = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
                    var r = getConsolidatedReportUseCase.GetConsolidatedReport(utcDateParam, tz);

                    if (r.Success == false)
                    {
                        if (r.Exception is KeyNotFoundException)
                            return Results.NotFound(r.Message);

                        if (r.Exception is InvalidDataException)
                            return Results.BadRequest(r.Message);

                        return Results.Problem(r.Message);
                    }

                    return Results.Ok(r.Data);

                }
                catch (InvalidDataException ex)
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
