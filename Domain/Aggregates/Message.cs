using Domain.Shared;

namespace Domain.Aggregates;

public class Message : IBaseEntity
{
    public Guid Id { get; set; }
    public string MessageTextContent { get; set; }
}