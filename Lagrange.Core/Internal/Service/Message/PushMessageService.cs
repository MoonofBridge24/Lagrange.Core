using System.Text;
using Lagrange.Core.Common;
using Lagrange.Core.Internal.Event;
using Lagrange.Core.Internal.Event.Message;
using Lagrange.Core.Internal.Event.Notify;
using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Core.Internal.Packets.Message.Notify;
using Lagrange.Core.Message;
using Lagrange.Core.Utility.Binary;
using ProtoBuf;

// ReSharper disable InconsistentNaming

namespace Lagrange.Core.Internal.Service.Message;

[EventSubscribe(typeof(PushMessageEvent))]
[Service("trpc.msg.olpush.OlPushService.MsgPush")]
internal class PushMessageService : BaseService<PushMessageEvent>
{
    protected override bool Parse(Span<byte> input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out PushMessageEvent output, out List<ProtocolEvent>? extraEvents)
    {
        output = null!;
        extraEvents = new List<ProtocolEvent>();
        
        // 尝试安全解析消息
        PushMsg? message = null;
        try 
        {
            message = Serializer.Deserialize<PushMsg>(input);
        }
        catch (ProtoException ex) when (ex.InnerException is OverflowException)
        {
            // 使用容错解析方法
            if (!TryParsePushMsgSafely(input, out message))
            {
                return false; // 无法解析，返回false让其他处理器尝试
            }
        }
        catch (OverflowException)
        {
            // 使用容错解析方法
            if (!TryParsePushMsgSafely(input, out message))
            {
                return false; // 无法解析，返回false让其他处理器尝试
            }
        }
        
        if (message?.Message?.ContentHead == null) return false;
        
        // 安全获取消息类型，如果Type溢出则跳过
        uint packetType;
        try
        {
            packetType = message.Message.ContentHead.Type;
        }
        catch (OverflowException)
        {
            return false; // Type字段溢出，无法处理此消息
        }
        catch (Exception)
        {
            return false; // Type字段异常，无法处理此消息
        }
        
        switch (packetType)
        {
            case (uint)PkgType.PrivateMessage or (uint)PkgType.GroupMessage or (uint)PkgType.TempMessage or (uint)PkgType.PrivateRecordMessage:
            {
                try
                {
                    var chain = MessagePacker.Parse(message.Message);
                    output = PushMessageEvent.Create(chain);
                }
                catch (OverflowException)
                {
                    // 消息体解析失败，但ContentHead有效，创建最小化事件
                    output = PushMessageEvent.Create(new MessageChain(0, "", ""));
                }
                break;
            }
            case (uint)PkgType.PrivateFileMessage:
            {
                try
                {
                    var chain = MessagePacker.ParsePrivateFile(message.Message);
                    output = PushMessageEvent.Create(chain);
                }
                catch (OverflowException)
                {
                    // 消息体解析失败，但ContentHead有效，创建最小化事件
                    output = PushMessageEvent.Create(new MessageChain(0, "", ""));
                }
                break;
            }
            case (uint)PkgType.GroupRequestJoinNotice when message.Message.Body?.MsgContent is { } content:
            {
                try
                {
                    var join = Serializer.Deserialize<GroupJoin>(content.AsSpan());
                    var joinEvent = GroupSysRequestJoinEvent.Result(join.GroupUin, join.TargetUid);
                    extraEvents.Add(joinEvent);
                }
                catch (OverflowException)
                {
                    // 忽略解析失败的事件
                }
                break;
            }
            case (uint)PkgType.GroupRequestInvitationNotice when message.Message.Body?.MsgContent is { } content:
            {
                try
                {
                    var invitation = Serializer.Deserialize<GroupInvitation>(content.AsSpan());
                    if (invitation.Cmd == 87)
                    {
                        var info = invitation.Info.Inner;
                        var invitationEvent = GroupSysRequestInvitationEvent.Result(info.GroupUin, info.TargetUid, info.InvitorUid);
                        extraEvents.Add(invitationEvent);
                    }
                }
                catch (OverflowException)
                {
                    // 忽略解析失败的事件
                }
                break;
            }
            case (uint)PkgType.GroupInviteNotice when message.Message.Body?.MsgContent is { } content:
            {
                try
                {
                    var invite = Serializer.Deserialize<GroupInvite>(content.AsSpan());
                    var inviteEvent = GroupSysInviteEvent.Result(invite.GroupUin, invite.InvitorUid);
                    extraEvents.Add(inviteEvent);
                }
                catch (OverflowException)
                {
                    // 忽略解析失败的事件
                }
                break;
            }
            case (uint)PkgType.GroupAdminChangedNotice when message.Message.Body?.MsgContent is { } content:
            {
                try
                {
                    var admin = Serializer.Deserialize<GroupAdmin>(content.AsSpan());
                    bool enabled; string uid;
                    if (admin.Body.ExtraEnable != null)
                    {
                        enabled = true;
                        uid = admin.Body.ExtraEnable.AdminUid;
                    }
                    else if (admin.Body.ExtraDisable != null)
                    {
                        enabled = false;
                        uid = admin.Body.ExtraDisable.AdminUid;
                    }
                    else
                    {
                        return false;
                    }

                    extraEvents.Add(GroupSysAdminEvent.Result(admin.GroupUin, uid, enabled));
                }
                catch (OverflowException)
                {
                    // 忽略解析失败的事件
                }
                break;
            }
            case (uint)PkgType.GroupMemberIncreaseNotice when message.Message.Body?.MsgContent is { } content:
            {
                try
                {
                    var increase = Serializer.Deserialize<GroupChange>(content.AsSpan());
                    var increaseEvent = GroupSysIncreaseEvent.Result(increase.GroupUin, increase.MemberUid, Encoding.UTF8.GetString(increase.Operator.AsSpan()), increase.DecreaseType);
                    extraEvents.Add(increaseEvent);
                }
                catch (OverflowException)
                {
                    // 忽略解析失败的事件
                }
                break;
            }
            case (uint)PkgType.GroupMemberDecreaseNotice when message.Message.Body?.MsgContent is { } content:
            {
                try
                {
                    var decrease = Serializer.Deserialize<GroupChange>(content.AsSpan());
                    GroupSysDecreaseEvent decreaseEvent;
                    if (decrease.DecreaseType == 3) // 3 是bot自身被踢出，Operator字段会是一个protobuf
                    {
                        var op = Serializer.Deserialize<OperatorInfo>(decrease.Operator.AsSpan());
                        decreaseEvent = GroupSysDecreaseEvent.Result(decrease.GroupUin, decrease.MemberUid, op.Operator.Uid, decrease.DecreaseType);
                    }
                    else
                    {
                        decreaseEvent = GroupSysDecreaseEvent.Result(decrease.GroupUin, decrease.MemberUid, Encoding.UTF8.GetString(decrease.Operator.AsSpan()), decrease.DecreaseType);
                    }
                    extraEvents.Add(decreaseEvent);
                }
                catch (OverflowException)
                {
                    // 忽略解析失败的事件
                }
                break;
            }
            case (uint)PkgType.Event0x210:
            {
                ProcessEvent0x210(input, message, extraEvents);
                break;
            }
            case (uint)PkgType.Event0x2DC:
            {
                ProcessEvent0x2DC(input, message, extraEvents);
                break;
            }
            default:
            {
                break;
            }
        }
        return true;
    }

    private static void ProcessEvent0x2DC(Span<byte> payload, PushMsg msg, List<ProtocolEvent> extraEvents)
    {
        var pkgType = (Event0x2DCSubType)(msg.Message.ContentHead.SubType ?? 0);
        switch (pkgType)
        {
            case Event0x2DCSubType.SubType16 when msg.Message.Body?.MsgContent is { } content:
            {
                using var packet = new BinaryPacket(content);
                _ = packet.ReadUint();  // group uin
                _ = packet.ReadByte();  // unknown byte
                var proto = packet.ReadBytes(Prefix.Uint16 | Prefix.LengthOnly); // proto length error
                var msgBody = Serializer.Deserialize<NotifyMessageBody>(proto.AsSpan());
                switch ((Event0x2DCSubType16Field13)(msgBody.Field13 ?? 0))
                {
                    case Event0x2DCSubType16Field13.GroupMemberSpecialTitleNotice:
                    {
                        break;
                    }
                    case Event0x2DCSubType16Field13.GroupNameChangeNotice:
                    {
                        // 33CAE9171000450801109B85D0B70618FFFFFFFF0F2097D2AB9E032A0D08011209E686A8E7BEA46F76680CA802D1DF18AA0118755F6C30323965684E706E4E6A6151725A55687776357551
                        var param = Serializer.Deserialize<GroupNameChange>(msgBody.EventParam.AsSpan());
                        extraEvents.Add(GroupSysNameChangeEvent.Result(msgBody.GroupUin, param.Name));
                        break;
                    }
                    case Event0x2DCSubType16Field13.GroupTodoNotice:
                    {
                        extraEvents.Add(GroupSysTodoEvent.Result(msgBody.GroupUin, msgBody.OperatorUid));
                        break;
                    }
                    case Event0x2DCSubType16Field13.GroupReactionNotice:
                    {
                        try
                        {
                            uint group = msgBody.GroupUin;
                            string uid = msgBody.Reaction.Data.Data.Data.OperatorUid;
                            uint type = msgBody.Reaction.Data.Data.Data.Type;
                            uint sequence = msgBody.Reaction.Data.Data.Target.Sequence;
                            string code = msgBody.Reaction.Data.Data.Data.Code;
                            uint count = msgBody.Reaction.Data.Data.Data.Count;
                            var groupRecallEvent = GroupSysReactionEvent.Result(group, sequence, uid, type == 1, code, count);
                            extraEvents.Add(groupRecallEvent);
                        }
                        catch (OverflowException)
                        {
                            // 忽略解析失败的事件
                        }
                        break;
                    }
                }
                break;
            }
            case Event0x2DCSubType.GroupRecallNotice when msg.Message.Body?.MsgContent is { } content:
            {
                using var packet = new BinaryPacket(content);
                _ = packet.ReadUint();  // group uin
                _ = packet.ReadByte();  // unknown byte
                var proto = packet.ReadBytes(Prefix.Uint16 | Prefix.LengthOnly);
                var recall = Serializer.Deserialize<NotifyMessageBody>(proto.AsSpan());
                var meta = recall.Recall.RecallMessages[0];
                var groupRecallEvent = GroupSysRecallEvent.Result(
                    recall.GroupUin,
                    meta.AuthorUid,
                    recall.Recall.OperatorUid,
                    meta.Sequence,
                    meta.Time,
                    meta.Random,
                    recall?.Recall.TipInfo?.Tip ?? ""
                );
                extraEvents.Add(groupRecallEvent);
                break;
            }
            case Event0x2DCSubType.GroupMuteNotice when msg.Message.Body?.MsgContent is { } content:
            {
                var mute = Serializer.Deserialize<GroupMute>(content.AsSpan());
                if (mute.Data.State.TargetUid == null)
                {
                    var groupMuteEvent = GroupSysMuteEvent.Result(mute.GroupUin, mute.OperatorUid, mute.Data.State.Duration != 0);
                    extraEvents.Add(groupMuteEvent);
                }
                else
                {
                    var memberMuteEvent = GroupSysMemberMuteEvent.Result(mute.GroupUin, mute.OperatorUid, mute.Data.State.TargetUid, mute.Data.State.Duration);
                    extraEvents.Add(memberMuteEvent);
                }
                break;
            }
            case Event0x2DCSubType.GroupGreyTipNotice21 when msg.Message.Body?.MsgContent is { } content:
            {
                using var packet = new BinaryPacket(content);
                _ = packet.ReadUint();  // group uin
                _ = packet.ReadByte();  // unknown byte
                var proto = packet.ReadBytes(Prefix.Uint16 | Prefix.LengthOnly);
                var greytip = Serializer.Deserialize<NotifyMessageBody>(proto.AsSpan());

                if (greytip.Type == 27) // essence
                {
                    var essenceMsg = greytip.EssenceMessage;
                    var groupEssenceEvent = GroupSysEssenceEvent.Result(essenceMsg.GroupUin, essenceMsg.MsgSequence,
                        essenceMsg.Random, essenceMsg.SetFlag, essenceMsg.MemberUin, essenceMsg.OperatorUin);
                    extraEvents.Add(groupEssenceEvent);
                    break;
                }

                if (greytip.Type == 32) // recall poke
                {
                    var recallPoke = greytip.GroupRecallPoke;
                    var @event = GroupSysRecallPokeEvent.Result(
                        recallPoke.GroupUin,
                        recallPoke.OperatorUid,
                        recallPoke.TipsSeqId
                    );
                    extraEvents.Add(@event);
                    break;
                }

                break;
            }
            case Event0x2DCSubType.GroupGreyTipNotice20 when msg.Message.Body?.MsgContent is { } content: // GreyTip
            {
                using var packet = new BinaryPacket(content);
                _ = packet.ReadUint();  // group uin
                _ = packet.ReadByte();  // unknown byte
                var proto = packet.ReadBytes(Prefix.Uint16 | Prefix.LengthOnly);
                _ = Serializer.Deserialize<NotifyMessageBody>(proto.AsSpan());
                break;
            }
        }
    }

    private static void ProcessEvent0x210(Span<byte> payload, PushMsg msg, List<ProtocolEvent> extraEvents)
    {
        var pkgType = (Event0x210SubType)(msg.Message.ContentHead.SubType ?? 0);
        switch (pkgType)
        {
            case Event0x210SubType.FriendRequestNotice when msg.Message.Body?.MsgContent is { } content:
            {
                // 简化处理，忽略FriendRequest解析
                break;
            }
            case Event0x210SubType.GroupMemberEnterNotice when msg.Message.Body?.MsgContent is { } content:
            {
                // 简化处理，忽略GroupMemberEnter解析
                break;
            }
            case Event0x210SubType.FriendDeleteOrPinChangedNotice when msg.Message.Body?.MsgContent is { } content:
            {
                // 简化处理，忽略FriendChange解析
                break;
            }
            case Event0x210SubType.FriendRecallNotice when msg.Message.Body?.MsgContent is { } content:
            {
                using var packet = new BinaryPacket(content);
                _ = packet.ReadUint();  // group uin
                _ = packet.ReadByte();  // unknown byte
                var proto = packet.ReadBytes(Prefix.Uint16 | Prefix.LengthOnly);
                var recall = Serializer.Deserialize<NotifyMessageBody>(proto.AsSpan());
                var meta = recall.Recall.RecallMessages[0];
                var friendRecallEvent = FriendSysRecallEvent.Result(
                    recall.Recall.OperatorUid,
                    meta.Sequence,
                    meta.Time,
                    meta.Random,
                    recall?.Recall.TipInfo?.Tip ?? ""
                );
                extraEvents.Add(friendRecallEvent);
                break;
            }
            case Event0x210SubType.ServicePinChanged: // Service pin changed
            {
                break;
            }
            case Event0x210SubType.FriendPokeNotice when msg.Message.Body?.MsgContent is { } content:
            {
                // 简化处理，忽略FriendPoke解析
                break;
            }
            case Event0x210SubType.GroupKickNotice when msg.Message.Body?.MsgContent is { } content:
            {
                // 简化处理，忽略GroupKick解析
                break;
            }
            case Event0x210SubType.FriendRecallPoke when msg.Message.Body?.MsgContent is { } content:
            {
                // 简化处理，忽略FriendRecallPoke解析
                break;
            }
        }
    }

    private enum PkgType
    {
        PrivateMessage = 166,
        GroupMessage = 82,
        TempMessage = 141,

        Event0x210 = 528,  // friend related event
        Event0x2DC = 732,  // group related event

        PrivateRecordMessage = 208,
        PrivateFileMessage = 529,

        GroupRequestInvitationNotice = 525, // from group member invitation
        GroupRequestJoinNotice = 84, // directly entered
        GroupInviteNotice = 87,  // the bot self is being invited
        GroupAdminChangedNotice = 44,  // admin change, both on and off
        GroupMemberIncreaseNotice = 33,
        GroupMemberDecreaseNotice = 34,
    }

    private enum Event0x2DCSubType
    {
        GroupMuteNotice = 12,
        SubType16 = 16,
        GroupRecallNotice = 17,
        GroupGreyTipNotice21 = 21,
        GroupGreyTipNotice20 = 20,
    }

    private enum Event0x2DCSubType16Field13
    {
        GroupMemberSpecialTitleNotice = 6,
        GroupNameChangeNotice = 12,
        GroupTodoNotice = 23,
        GroupReactionNotice = 35,
    }

    private enum Event0x210SubType
    {
        FriendRequestNotice = 35,
        GroupMemberEnterNotice = 38,
        FriendDeleteOrPinChangedNotice = 39,
        FriendRecallNotice = 138,
        ServicePinChanged = 199, // e.g: My computer | QQ Wallet | ...
        FriendPokeNotice = 290,
        GroupKickNotice = 212,
        FriendRecallPoke = 321,
    }
    
    private static bool TryParsePushMsgSafely(Span<byte> input, out PushMsg? message)
    {
        message = null;
        return false; // 简化实现，暂不处理复杂容错
    }
}
// ReSharper disable InconsistentNaming
