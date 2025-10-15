namespace Lagrange.Core.Event.EventArg;

public class GroupInvitationRequestEvent : EventBase
{
    internal GroupInvitationRequestEvent(ulong groupUin, ulong targetUin, ulong invitorUin)
    {
        GroupUin = groupUin;
        TargetUin = targetUin;
        InvitorUin = invitorUin;
        EventMessage = $"[{nameof(GroupInvitationRequestEvent)}] {TargetUin} from {InvitorUin} at {GroupUin}";
    }
    
    public ulong GroupUin { get; }
    
    public ulong TargetUin { get; }
    
    public ulong InvitorUin { get; }
}