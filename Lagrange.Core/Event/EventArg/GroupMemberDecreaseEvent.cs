namespace Lagrange.Core.Event.EventArg;

public class GroupMemberDecreaseEvent : EventBase
{
    public ulong GroupUin { get; }

    public ulong MemberUin { get; }

    public ulong? OperatorUin { get; }

    public EventType Type { get; }

    public GroupMemberDecreaseEvent(ulong groupUin, ulong memberUin, ulong? operatorUin, ulong type)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        OperatorUin = operatorUin;
        Type = (EventType)type;

        EventMessage = $"{nameof(GroupMemberDecreaseEvent)}: {GroupUin} | {MemberUin} | {OperatorUin} | {Type}";
    }

    public enum EventType : ulong
    {
        KickMe = 3,
        Disband = 129,
        Leave = 130,
        Kick = 131
    }
}