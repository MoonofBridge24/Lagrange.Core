namespace Lagrange.Core.Event.EventArg;

public class GroupMemberIncreaseEvent : EventBase
{
    public ulong GroupUin { get; }
    
    public ulong MemberUin { get; }
    
    public ulong? InvitorUin { get; }
    
    public EventType Type { get; }
    
    public GroupMemberIncreaseEvent(ulong groupUin, ulong memberUin, ulong? invitorUin, ulong type)
    {
        GroupUin = groupUin;
        MemberUin = memberUin;
        InvitorUin = invitorUin;
        Type = (EventType)type;

        EventMessage = $"{nameof(GroupMemberIncreaseEvent)}: {GroupUin} | {MemberUin} | {InvitorUin} | {Type}";
    }

    public enum EventType : ulong
    {
        Approve = 130,
        Invite = 131
    }
}
