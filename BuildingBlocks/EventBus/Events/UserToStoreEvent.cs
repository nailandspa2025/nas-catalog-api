namespace BuildingBlocks.EventBus.Events;

public class UserToStoreEvent
{
    public string UserId { get; set; } = null!;

    public List<long> StoreIds { get; set; }
}

