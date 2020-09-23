using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Access.Auth.Service.Business.Validation
{
    public interface IExternalValidationHandler
    {
        Task<JObject> GetExternalValidationToken(string email);
    }
}
