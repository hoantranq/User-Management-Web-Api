namespace UserManagement_Backend.Helpers
{
    public class Authorization
    {
        public enum Roles
        {
            Administrator,
            Moderator,
            User
        }

        public const string DEFAULT_USERNAME = "user";
        public const string DEFAULT_EMAIL = "user@gmail.com";
        public const string DEFAULT_PASSWORD = "Password@123";
        public const Roles DEFAULT_ROLE = Roles.User;
    }
}
