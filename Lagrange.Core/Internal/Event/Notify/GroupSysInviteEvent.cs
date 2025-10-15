namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysInviteEvent : ProtocolEvent
{
    public ulong GroupUin { get; set; }
    
    public string InvitorUid { get; set; }

    private GroupSysInviteEvent(ulong groupUin, string invitorUid) : base(0)
    {
        GroupUin = groupUin;
        InvitorUid = invitorUid;
    }
    
    public static GroupSysInviteEvent Result(ulong groupUin, string invitorUid) => new(groupUin, invitorUid);
}