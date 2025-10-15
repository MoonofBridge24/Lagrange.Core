namespace Lagrange.Core.Event.EventArg;

public class GroupRecallPokeEvent : EventBase
{
    public ulong GroupUin { get; }

    public uint OperatorUin { get; }

    public ulong TipsSeqId { get; set; }

    public GroupRecallPokeEvent(ulong groupUin, uint operatorUin, ulong tipsSeqId)
    {
        GroupUin = groupUin;
        OperatorUin = operatorUin;
        TipsSeqId = tipsSeqId;

        EventMessage = $"[GroupRecallPoke] Group: {GroupUin} | Operator: {OperatorUin} | TipsSeqId: {TipsSeqId}";
    }
}