using System.ComponentModel.DataAnnotations;
using Domain.Aggregates.Identity;
using Domain.Shared;

namespace Domain.Aggregates.Account;

public class ClientUser : IBaseAuditableEntity
{
    public Guid Id { get; set; }
    
    [MaxLength(30), MinLength(2)]
    public string FirstName { get; set; }
    
    [MaxLength(16), MinLength(3)]
    public string UserName { get; set; }
    
    [MaxLength(30), MinLength(2)]
    public string LastName { get; set; }
    
    public string UserId { get; set; }
    public virtual User User { get; set; }
    
    public bool IsBanned { get; set; }
    public DateTimeOffset BannedUntil { get; set; }
    
    public DateTimeOffset Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}