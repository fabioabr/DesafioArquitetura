using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinancialServices.Api
{
    public interface IEndpoint
    {
        RouteHandlerBuilder Map(IEndpointRouteBuilder routeBuilder);

    }
}
