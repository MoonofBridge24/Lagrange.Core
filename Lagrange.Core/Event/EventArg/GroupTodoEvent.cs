namespace Lagrange.Core.Event.EventArg;

public class GroupTodoEvent : EventBase
{
    public ulong GroupUin { get; }

    public uint OperatorUin { get; }

    public GroupTodoEvent(ulong groupUin, uint operatorUin)
    {
        GroupUin = groupUin;
        OperatorUin = operatorUin;

        EventMessage = $"{nameof(GroupPokeEvent)}:  GroupUin: {GroupUin} | OperatorUin: {OperatorUin}";
    }
}