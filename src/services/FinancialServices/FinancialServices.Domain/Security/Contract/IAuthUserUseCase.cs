using FinancialServices.Domain.Security.Model;
using FinancialServices.Utils.Shared;

namespace FinancialServices.Domain.Security.Contract
{
    public interface IAuthUserUseCase
    {
        GenericResponse<UserModel?> Execute(string apiKey, string[] requiredRoles);

    }
}
