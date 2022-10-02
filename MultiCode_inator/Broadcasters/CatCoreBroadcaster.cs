using System;
using CatCore;
using CatCore.Services.Multiplexer;
using MultiCode_inator.Managers;
using SiraUtil.Logging;

namespace MultiCode_inator.Broadcasters
{
    internal class CatCoreBroadcaster : AbstractBroadcaster
    {
        private object? _chatServiceMultiplexer;
        
        private readonly CatCoreInstance _catCoreInstance;

        public CatCoreBroadcaster(SiraLog siraLog, BroadcastManager broadcastManager) : base(siraLog, broadcastManager)
        {
            _catCoreInstance = CatCoreInstance.Create();
        }
        
        private ChatServiceMultiplexer? CastedChatServiceMultiplexer
        {
            get
            {
                if (_chatServiceMultiplexer == null)
                {
                    return null;
                }

                return (ChatServiceMultiplexer) _chatServiceMultiplexer;
            }
            set => _chatServiceMultiplexer = value;
        }
        
        public override void Initialize()
        {
            CastedChatServiceMultiplexer = _catCoreInstance.RunAllServices();
            CastedChatServiceMultiplexer!.OnTextMessageReceived += CatService_OnTextMessageReceived;
            
            base.Initialize();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (CastedChatServiceMultiplexer != null)
            {
                CastedChatServiceMultiplexer!.OnTextMessageReceived -= CatService_OnTextMessageReceived;
                CastedChatServiceMultiplexer = null;
            }
            _catCoreInstance.StopAllServices();
        }

        private void CatService_OnTextMessageReceived(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            BroadcastManager.ReceivedMessage(message.Channel, message.Message, message.Sender.UserName);
        }

        protected override void BroadcastResponseMessage(object channel, string message)
        {
            try
            {
                var castedChannel = (MultiplexedChannel) channel;
                castedChannel.SendMessage(message);
                SiraLog.Info("Successfully sent message");
            }
            catch (Exception e)
            {
                SiraLog.Error(e);
            }
        }

        protected override void BroadcastMessageToAllChannels(string message)
        {
            try
            {
                if (CastedChatServiceMultiplexer != null)
                {
                    foreach (var channel in CastedChatServiceMultiplexer.GetTwitchPlatformService().GetChannelManagementService().GetAllActiveChannels())
                    {
                        channel.SendMessage(message);
                    }
                }

                SiraLog.Info("Successfully sent messages");
            }
            catch (Exception e)
            {
                SiraLog.Error(e);
            }
        }
    }
}