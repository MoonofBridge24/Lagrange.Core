namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysReactionEvent : ProtocolEvent
{
    public ulong TargetGroupUin { get; }

    public ulong TargetSequence { get; }

    public string OperatorUid { get; }

    public bool IsAdd { get; }

    public string Code { get; }

    public ulong Count { get; }

    private GroupSysReactionEvent(ulong targetGroupUin, ulong targetSequence, string operatorUid, bool isAdd, string code, ulong count) : base(0)
    {
        TargetGroupUin = targetGroupUin;
        TargetSequence = targetSequence;
        OperatorUid = operatorUid;
        IsAdd = isAdd;
        Code = code;
        Count = count;
    }

    public static GroupSysReactionEvent Result(ulong groupUin, ulong targetSequence, string operatorUid, bool isAdd, string code, ulong count)
        => new(groupUin, targetSequence, operatorUid, isAdd, code, count);
}