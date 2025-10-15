namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysTodoEvent : ProtocolEvent
{
    public ulong GroupUin { get; }

    public string OperatorUid { get; }

    private GroupSysTodoEvent(ulong groupUin, string operatorUid) : base(0)
    {
        GroupUin = groupUin;
        OperatorUid = operatorUid;
    }

    public static GroupSysTodoEvent Result(ulong groupUin, string operatorUid)
        => new(groupUin, operatorUid);
}