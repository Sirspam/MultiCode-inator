using System;
using CatCore;
using CatCore.Services.Multiplexer;
using MultiCode_inator.Configuration;
using SiraUtil.Logging;
using Zenject;

namespace MultiCode_inator.Utils
{
    internal class CatCoreBroadcaster : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly PluginConfig _pluginConfig;
        private readonly CatCoreInstance _catCoreInstance;
        private ChatServiceMultiplexer? _chatServiceMultiplexer;

        public CatCoreBroadcaster(SiraLog siraLog, PluginConfig pluginConfig)
        {
            _siraLog = siraLog;
            _pluginConfig = pluginConfig;
            _catCoreInstance = CatCoreInstance.Create();
        }
        
        public void Initialize()
        {
            _chatServiceMultiplexer = _catCoreInstance.RunAllServices();
            _chatServiceMultiplexer!.OnTextMessageReceived += CatService_OnTextMessageReceived;
        }

        public void Dispose()
        {
            if (_chatServiceMultiplexer != null)
            {
                _chatServiceMultiplexer!.OnTextMessageReceived -= CatService_OnTextMessageReceived;
                _chatServiceMultiplexer = null;
            }
            _catCoreInstance.StopAllServices();
        }

        private void CatService_OnTextMessageReceived(MultiplexedPlatformService service, MultiplexedMessage message)
        {
            if (message.Message.ToLower() == "!mc" || message.Message.ToLower() == "!multicode")
            {
                _siraLog.Info("Received MultiCode command");
                try
                {
                    if (StaticFields.RoomCode != null && _pluginConfig.CommandEnabled)
                    {
                        message.Channel.SendMessage($"! {message.Sender.UserName}, The current multiplayer lobby code is {StaticFields.RoomCode}");
                    }
                    else
                    {
                        message.Channel.SendMessage($"! {message.Sender.UserName}, The streamer has MultiCode disabled or they aren't playing multiplayer");
                    }
                }
                catch (Exception e)
                {
                    _siraLog.Critical(e);
                }
            }
        }
    }
}