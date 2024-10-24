using Domain.Aggregates.Account;
using Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace Domain.Aggregates.Identity;

public class User : IdentityUser, IBaseEntity
{
    public Guid Id { get; set; }
    public bool IsEnabled { get; set; }
    public virtual ClientUser ClientUser { get; set; }
    public string? AccessTokenId { get; set; }
    public string? RefreshToken { get; set; }
    
    public DateTime? TokenExpires { get; set; }
    
    public DateTime? TokenCreated { get; set; }
    
}