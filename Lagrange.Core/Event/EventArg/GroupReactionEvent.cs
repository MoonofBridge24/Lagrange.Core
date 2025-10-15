namespace Lagrange.Core.Event.EventArg;

public class GroupReactionEvent : EventBase
{
    public ulong TargetGroupUin { get; }

    public ulong TargetSequence { get; }

    public ulong OperatorUin { get; }

    public bool IsAdd { get; }

    public string Code { get; }

    public ulong Count { get; }

    public GroupReactionEvent(ulong targetGroupUin, ulong targetSequence, ulong operatorUin, bool isAdd, string code, ulong count)
    {
        TargetGroupUin = targetGroupUin;
        TargetSequence = targetSequence;
        OperatorUin = operatorUin;
        IsAdd = isAdd;
        Code = code;
        Count = count;

        EventMessage = $"{nameof(GroupReactionEvent)}:  TargetGroupUin: {TargetGroupUin} | TargetSequence: {TargetSequence} | OperatorUin: {OperatorUin} | IsAdd: {IsAdd} | Code: {Code} | Count: {Count}";
    }
}