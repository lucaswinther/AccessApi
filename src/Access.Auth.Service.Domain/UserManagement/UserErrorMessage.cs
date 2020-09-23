namespace Access.Auth.Service.Domain.UserManagement
{
    public static class UserErrorMessage
    {
        public const string SHORT_PASSWORD = "Invalid password: minimum 6 characters";
        public const string INVALID_ID = "Invalid id";
        public const string INVALID_CNPJ = "Invalid CNPJ";
        public const string INVALID_PRODUCT = "Invalid product";
        public const string INVALID_USERNAME = "Invalid username";
        public const string INVALID_NICKNAME = "Invalid nickname";
        public const string INVALID_EMAIL = "Invalid email";
        public const string INVALID_ROLE = "Invalid role";
        public const string INVALID_CURRENT_PASSWORD = "Invalid current password";
        public const string EXISTING_USER = "User already exists";
        public const string USER_ID_NOT_FOUND = "User ID not found";
        public const string PASSWORD_CONFIRMATION_FAILED = "Password confirmation failed";
        public const string USER_NOT_FOUND = "User not found";
        public const string NICKNAME_UNAVAILABLE = "Nickname unavailable";
    }
}
