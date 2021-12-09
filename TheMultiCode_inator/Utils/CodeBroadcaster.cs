using ChatCore;
using ChatCore.Interfaces;
using System;
using TheMultiCode_inator.Configuration;
using Zenject;

namespace TheMultiCode_inator.Utils
{
    // We do an excessive amount of copying Auros
    internal class CodeBroadcaster : IInitializable, IDisposable
    {
        private readonly ChatCoreInstance chatCoreInstance = ChatCoreInstance.Create();
        private IChatService chatService;

        public void Initialize()
        {
            chatService = chatCoreInstance.RunAllServices();
            chatService.OnTextMessageReceived += ChatService_OnTextMessageReceived;
        }

        public void Dispose()
        {
            if (chatService != null) chatService.OnTextMessageReceived -= ChatService_OnTextMessageReceived;
            chatCoreInstance.StopAllServices();
        }

        private void ChatService_OnTextMessageReceived(IChatService service, IChatMessage msg)
        {
            if (msg.Message.ToLower() == "!mc" || msg.Message.ToLower() == "!multicode")
            {
                try
                {
                    // I wanted to have this automatically enable / disable the command if the user has server browser installed and if their lobby was on there
                    // Couldn't think of a decent way of doing it without having to make the mod a dependency, which I don't want to do
                    // It's mainly just a QoL thing so MultiCode-inator will be fine without it
                    if (CodeManager.RoomCode != null && PluginConfig.Instance.CommandEnabled)
                    {
                        service.SendTextMessage($"! {msg.Sender.UserName}, The current multiplayer lobby code is {CodeManager.RoomCode}", msg.Channel);
                    }
                    else
                    {
                        service.SendTextMessage($"! {msg.Sender.UserName}, The player either isn't in a multiplayer lobby or they have !multicode disabled", msg.Channel);
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.Critical(e);
                }
            }
        }
    }
}