using Domain.Aggregates.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Providers.Identity;

public class IdentityUserManager : UserManager<User>
{
    private readonly IAuthorizationService _authorizationService;
    public IdentityUserManager(
        IUserStore<User> store, 
        IOptions<IdentityOptions> optionsAccessor, 
        IPasswordHasher<User> passwordHasher, 
        IEnumerable<IUserValidator<User>> userValidators, 
        IEnumerable<IPasswordValidator<User>> passwordValidators, 
        ILookupNormalizer keyNormalizer, 
        IdentityErrorDescriber errors, 
        IServiceProvider services, 
        ILogger<UserManager<User>> logger,
        IAuthorizationService authorizationService
        ) : base( store, 
            optionsAccessor, 
            passwordHasher, 
            userValidators, 
            passwordValidators, 
            keyNormalizer, 
            errors, 
            services, 
            logger
        )
    {
        _authorizationService = authorizationService;
    }

    public static bool IsBanned(User user)
    {
        return user.ClientUser.IsBanned;
    }
    
    public static bool IsEnabled(User user)
    {
        return !IsBanned(user) && user.IsEnabled;
    }
}