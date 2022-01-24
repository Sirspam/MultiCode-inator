using System;
using ChatCore;
using ChatCore.Interfaces;
using SiraUtil.Logging;
using TheMultiCode_inator.Configuration;
using Zenject;

namespace TheMultiCode_inator.Utils
{
    internal class CodeBroadcaster : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly PluginConfig _pluginConfig;
        private readonly ChatCoreInstance _chatCoreInstance = ChatCoreInstance.Create();
        private IChatService _chatService;

        public CodeBroadcaster(SiraLog siraLog, PluginConfig pluginConfig)
        {
            _siraLog = siraLog;
            _pluginConfig = pluginConfig;
        }
        
        public void Initialize()
        {
            _chatService = _chatCoreInstance.RunAllServices();
            _chatService.OnTextMessageReceived += ChatService_OnTextMessageReceived;
        }

        public void Dispose()
        {
            if (_chatService != null) _chatService.OnTextMessageReceived -= ChatService_OnTextMessageReceived;
            _chatCoreInstance.StopAllServices();
        }

        private void ChatService_OnTextMessageReceived(IChatService service, IChatMessage msg)
        {
            if (msg.Message.ToLower() == "!mc" || msg.Message.ToLower() == "!multicode")
            {
                _siraLog.Info("Received multicode command");
                try
                {
                    // I wanted to have this automatically enable / disable the command if the user has server browser installed and if their lobby was on there
                    // Couldn't think of a decent way of doing it without having to make the mod a dependency, which I don't want to do
                    // It's mainly just a QoL thing so MultiCode_inator will be fine without it
                    if (CodeManager.RoomCode != null && _pluginConfig.CommandEnabled)
                        service.SendTextMessage($"!{msg.Sender.UserName}, The current multiplayer lobby code is {CodeManager.RoomCode}", msg.Channel);
                    else
                        service.SendTextMessage($"!{msg.Sender.UserName}, The player isn't in a multiplayer lobby or they have MultiCode disabled", msg.Channel);
                }
                catch (Exception e)
                {
                    _siraLog.Critical(e);
                }
            }
        }
    }
}