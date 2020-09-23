using System.Threading.Tasks;
using Access.Auth.Service.Domain.Request;
using Access.Auth.Service.Domain.UserManagement;

namespace Access.Auth.Service.Business.UserManagement
{
    public interface IUserManagementHandler
    {
        Task AddAsync(User user);
        Task ChangePasswordAsync(PasswordChangeRequest request, string userId);
        Task ChangeNicknameAsync(NicknameChangeRequest request, string userId);
        Task ChangeNameAsync(NameChangeRequest request);
        Task ChangeEmailAsync(EmailChangeRequest request);
        Task AddCnpjAsync(CnpjUpdateRequest request);
        Task RemoveCnpjAsync(CnpjUpdateRequest request);
        Task AddProductAsync(ProductUpdateRequest request);
        Task RemoveProductAsync(ProductUpdateRequest request);
        Task AddRoleAsync(RoleUpdateRequest request);
        Task RemoveRoleAsync(RoleUpdateRequest request);
        Task OldPortalChangePasswordAsync(OldPortalPasswordChangeRequest request);
        Task OldPortalChangeNicknameAsync(OldPortalNicknameChangeRequest request);
    }  
}
