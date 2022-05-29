using Microsoft.AspNetCore.Identity;

namespace PS
{
    public class Defaults
    {
        public static Action<IdentityOptions> IdentityOptions
        {
            get
            {
                return new Action<IdentityOptions>(options =>
                {
                    //SignIn
                    options.SignIn.RequireConfirmedAccount = true;

                    //Password
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredUniqueChars = 1;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                });
            }
        }
    }
}
