namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysIncreaseEvent : ProtocolEvent
{
    public ulong GroupUin { get; }
    
    public string MemberUid { get; }
    
    public string? InvitorUid { get; }
    
    public ulong Type { get; }
    
    private GroupSysIncreaseEvent(ulong groupUin, string memberUid, string? invitorUid, ulong type) : base(0)
    {
        GroupUin = groupUin;
        MemberUid = memberUid;
        InvitorUid = invitorUid;
        Type = type;
    }

    public static GroupSysIncreaseEvent Result(ulong groupUin, string uid, string? invitorUid, ulong type) =>
        new(groupUin, uid, invitorUid, type);
}