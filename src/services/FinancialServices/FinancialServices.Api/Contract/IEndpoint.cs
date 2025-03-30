using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinancialServices.Api.Contract
{
    public interface IEndpoint
    {
        RouteHandlerBuilder Map(IEndpointRouteBuilder routeBuilder);

    }
}
