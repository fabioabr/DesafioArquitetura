using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialServices.Infrastructure.Enums
{
    public enum EventBusTypeEnum
    {
        RabbitMQ,
        Aws_SQS,
        Gcp_PubSub,
    }
}
