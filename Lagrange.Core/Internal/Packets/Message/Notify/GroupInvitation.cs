using ProtoBuf;

namespace Lagrange.Core.Internal.Packets.Message.Notify;

#pragma warning disable CS8618

[ProtoContract]
internal class GroupInvitation
{
    [ProtoMember(1)] public int Cmd { get; set; }
    
    [ProtoMember(2)] public InvitationInfo Info { get; set; }
}

[ProtoContract]
internal class InvitationInfo
{
    [ProtoMember(1)] public InvitationInner Inner { get; set; }
}


[ProtoContract]
internal class InvitationInner
{
    [ProtoMember(1)] public ulong GroupUin { get; set; }
    
    [ProtoMember(2)] public ulong Field2 { get; set; }
    
    [ProtoMember(3)] public ulong Field3 { get; set; }
    
    [ProtoMember(4)] public ulong Field4 { get; set; }
    
    [ProtoMember(5)] public string TargetUid { get; set; }
    
    [ProtoMember(6)] public string InvitorUid { get; set; }

    [ProtoMember(7)] public ulong Field7 { get; set; }
    
    [ProtoMember(9)] public ulong Field9 { get; set; }
    
    [ProtoMember(10)] public byte[] Field10 { get; set; }
    
    [ProtoMember(11)] public ulong Field11 { get; set; }
    
    [ProtoMember(12)] public string Field12 { get; set; }
}

