namespace Access.Auth.Service.Domain.Configuration
{
    public interface IAuthConfiguration
    {
        string IdentityServerBaseAddress { get; }
        string ExternalValidationClientId { get; }
        string ExternalValidationSecret { get; }
        string ExternalValidationScope { get; }
        string UserManagementClientId { get; }
        string UserManagementScope { get; }
        string DatabaseName { get; }
        string DatabaseConnectionString { get; }
    }
}
