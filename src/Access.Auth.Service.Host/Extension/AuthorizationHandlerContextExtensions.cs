using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Access.Auth.Service.Host.Extension
{
    public static class AuthorizationHandlerContextExtensions
    {
        public static async Task<T> GetResourceBodyAs<T>(this AuthorizationHandlerContext context)
        {
            var body = await context.GetResourceBody();
            return JsonConvert.DeserializeObject<T>(body);
        }

        public static async Task<string> GetResourceBody(this AuthorizationHandlerContext context)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            authFilterContext.HttpContext.Request.EnableRewind();

            using (var stream = new MemoryStream())
            {
                await authFilterContext.HttpContext.Request.Body.CopyToAsync(stream);

                authFilterContext.HttpContext.Request.Body.Position = 0;

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
