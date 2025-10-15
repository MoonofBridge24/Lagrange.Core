namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysDecreaseEvent : ProtocolEvent
{
    public ulong GroupUin { get; }
    
    public string MemberUid { get; }
    
    public string? OperatorUid { get; }
    
    public ulong Type { get; }
    
    private GroupSysDecreaseEvent(ulong groupUin, string memberUid, string? operatorUid, ulong type) : base(0)
    {
        GroupUin = groupUin;
        MemberUid = memberUid;
        OperatorUid = operatorUid;
        Type = type;
    }

    public static GroupSysDecreaseEvent Result(ulong groupUin, string uid, string? operatorUid, ulong type) =>
        new(groupUin, uid, operatorUid, type);
}