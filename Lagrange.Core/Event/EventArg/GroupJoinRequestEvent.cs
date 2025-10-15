namespace Lagrange.Core.Event.EventArg;

public class GroupJoinRequestEvent : EventBase
{
    internal GroupJoinRequestEvent(ulong groupUin, ulong targetUin)
    {
        TargetUin = targetUin;
        GroupUin = groupUin;
        EventMessage = $"[{nameof(GroupJoinRequestEvent)}] {TargetUin} at {GroupUin}";
    }
    
    public ulong TargetUin { get; }
    
    public ulong GroupUin { get; }
}