namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysAdminEvent : ProtocolEvent
{
    public ulong GroupUin { get; }
    
    public string Uid { get; }
    
    public bool IsPromoted { get; }
    
    private GroupSysAdminEvent(ulong groupUin, string uid, bool isPromoted) : base(0)
    {
        GroupUin = groupUin;
        Uid = uid;
        IsPromoted = isPromoted;
    }
    
    public static GroupSysAdminEvent Result(ulong groupUin, string uid, bool isPromoted) => new(groupUin, uid, isPromoted);
}