namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysRequestJoinEvent : ProtocolEvent
{
    public string TargetUid { get; }
    
    public ulong GroupUin { get; }

    private GroupSysRequestJoinEvent(ulong groupUin, string targetUid) : base(0)
    {
        TargetUid = targetUid;
        GroupUin = groupUin;
    }

    public static GroupSysRequestJoinEvent Result(ulong groupUin, string targetUid) => new(groupUin, targetUid);
}