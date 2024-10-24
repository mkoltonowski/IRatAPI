using Domain.Aggregates.Account;
using Domain.Aggregates.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class ClientUserConfiguration : IEntityTypeConfiguration<ClientUser>
{
    public void Configure(EntityTypeBuilder<ClientUser> builder)
    {
        
        builder.HasKey( clientUser  => clientUser.Id );
        builder.HasOne( clientUser => clientUser.User )
            .WithOne( user => user.ClientUser)
        .HasForeignKey<ClientUser>( user => user.UserId );
    }
}