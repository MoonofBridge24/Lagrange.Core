using Lagrange.Core.Internal.Event;

namespace Lagrange.Core.Internal.Event.Notify;

internal class GroupSysRequestInvitationEvent : ProtocolEvent
{
    public ulong GroupUin { get; }
    
    public string TargetUid { get; }
    
    public string InvitorUid { get; }

    private GroupSysRequestInvitationEvent(ulong groupUin, string targetUid, string invitorUid) : base(0)
    {
        GroupUin = groupUin;
        TargetUid = targetUid;
        InvitorUid = invitorUid;
    }

    public static GroupSysRequestInvitationEvent Result(ulong groupUin, string targetUid, string invitorUid) 
        => new(groupUin, targetUid, invitorUid);
}