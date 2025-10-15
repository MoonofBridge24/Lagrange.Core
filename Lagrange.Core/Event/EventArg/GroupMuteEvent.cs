namespace Lagrange.Core.Event.EventArg;

public class GroupMuteEvent : EventBase
{
    public ulong GroupUin { get; }
    
    public uint? OperatorUin { get; }
    
    public bool IsMuted { get; }
    
    public GroupMuteEvent(ulong groupUin, uint? operatorUin, bool isMuted)
    {
        GroupUin = groupUin;
        OperatorUin = operatorUin;
        IsMuted = isMuted;
        
        EventMessage = $"{nameof(GroupMuteEvent)}: {GroupUin} | {OperatorUin} | IsMuted: {IsMuted}";
    }
}