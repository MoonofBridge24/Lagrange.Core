namespace Lagrange.Core.Event.EventArg;

public class GroupNameChangeEvent : EventBase
{
    public ulong GroupUin { get; }

    public string Name { get; }

    public GroupNameChangeEvent(ulong groupUin, string name)
    {
        GroupUin = groupUin;
        Name = name;

        EventMessage = $"{nameof(GroupNameChangeEvent)}:  GroupUin: {GroupUin} | Name: {Name}";
    }
}