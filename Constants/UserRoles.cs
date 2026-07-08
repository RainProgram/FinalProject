namespace EventRescue.Constants
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Client = "Client";
        public const string Provider = "Provider";
    }
}

// للإستخدام : 
// Role = UserRoles.Admin;
// او 
// if (user.Role == UserRoles.Provider)
// {
//     ...
// }
