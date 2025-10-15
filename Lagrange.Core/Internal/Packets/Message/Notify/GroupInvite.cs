using ProtoBuf;

namespace Lagrange.Core.Internal.Packets.Message.Notify;

#pragma warning disable CS8618

[ProtoContract]
internal class GroupInvite
{
    [ProtoMember(1)] public ulong GroupUin { get; set; }
    
    [ProtoMember(2)] public ulong Field2 { get; set; } // 1
    
    [ProtoMember(3)] public ulong Field3 { get; set; } // 4
    
    [ProtoMember(4)] public ulong Field4 { get; set; } // 0
    
    [ProtoMember(5)] public string InvitorUid { get; set; }
    
    [ProtoMember(6)] public byte[] Hashes { get; set; }
}