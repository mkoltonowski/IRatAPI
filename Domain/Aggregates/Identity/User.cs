using Domain.Aggregates.User;
using Domain.Shared;
using Microsoft.AspNetCore.Identity;

namespace Domain.Aggregates.Identity;

public class User : IdentityUser, IBaseEntity
{
    public Guid Id { get; set; }
    public bool IsEnabled { get; set; }
    public virtual ClientUser ClientUser { get; set; }
}