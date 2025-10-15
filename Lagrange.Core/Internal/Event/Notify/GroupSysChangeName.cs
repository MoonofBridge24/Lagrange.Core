namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysNameChangeEvent : ProtocolEvent
{
    public ulong GroupUin { get; }

    public string Name { get; }

    private GroupSysNameChangeEvent(ulong groupUin, string name) : base(0)
    {
        GroupUin = groupUin;
        Name = name;
    }

    public static GroupSysNameChangeEvent Result(ulong groupUin, string name)
        => new(groupUin, name);
}