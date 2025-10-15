namespace Lagrange.Core.Event.EventArg;

public class GroupAdminChangedEvent : EventBase
{
    public ulong GroupUin { get; }
    
    public ulong AdminUin { get; }
    
    public bool IsPromote { get; }
    
    public GroupAdminChangedEvent(ulong groupUin, ulong adminUin, bool isPromote)
    {
        GroupUin = groupUin;
        AdminUin = adminUin;
        IsPromote = isPromote;
        EventMessage = $"{nameof(GroupAdminChangedEvent)} | {GroupUin} | {AdminUin} | {IsPromote}";
    }
}