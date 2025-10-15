namespace Lagrange.Core.Event.EventArg;

public class GroupInvitationEvent : EventBase
{
    public ulong GroupUin { get; }

    public ulong InvitorUin { get; }

    public ulong? Sequence { get; }

    internal GroupInvitationEvent(ulong groupUin, ulong invitorUin)
    {
        GroupUin = groupUin;
        InvitorUin = invitorUin;
        Sequence = null;
        EventMessage = $"[{nameof(GroupInvitationEvent)}]: {GroupUin} from {InvitorUin}";
    }

    internal GroupInvitationEvent(ulong groupUin, ulong invitorUin, ulong? sequence) : this(groupUin, invitorUin)
    {
        Sequence = sequence;
    }
}