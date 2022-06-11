using System;
using BeatSaberPlus.SDK.Chat;
using BeatSaberPlus.SDK.Chat.Interfaces;
using MultiCode_inator.Configuration;
using SiraUtil.Logging;
using Zenject;

namespace MultiCode_inator.Utils
{
	internal class BspBroadcaster : IInitializable, IDisposable
	{
		private readonly SiraLog _siraLog;
		private readonly PluginConfig _pluginConfig;

		public BspBroadcaster(SiraLog siraLog, PluginConfig pluginConfig)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
		}

		public void Initialize()
		{
			Service.Acquire();
			Service.Multiplexer.OnTextMessageReceived += MultiplexerOnOnTextMessageReceived;
		}

		public void Dispose()
		{
			Service.Multiplexer.OnTextMessageReceived -= MultiplexerOnOnTextMessageReceived;
			Service.Release();
		}

		private void MultiplexerOnOnTextMessageReceived(IChatService chatService, IChatMessage chatMessage)
		{
			if (chatMessage.Message.ToLower() == "!mc" || chatMessage.Message.ToLower() == "!multicode")
			{
				_siraLog.Info("Received MultiCode command");
				try
				{
					if (StaticFields.RoomCode != null && _pluginConfig.CommandEnabled)
					{
						chatService.SendTextMessage(chatMessage.Channel, $"! {chatMessage.Sender.UserName}, The current multiplayer lobby code is {StaticFields.RoomCode}");
					}
					else
					{
						chatService.SendTextMessage(chatMessage.Channel, $"! {chatMessage.Sender.UserName}, The streamer has MultiCode disabled or they aren't playing multiplayer");
					}
					_siraLog.Info("Successfully responded");
				}
				catch (Exception e)
				{
					_siraLog.Critical(e);
				}
			}
		}
	}
}