using System;
using BeatSaberPlus.SDK.Chat;
using BeatSaberPlus.SDK.Chat.Interfaces;
using MultiCode_inator.AffinityPatches;
using MultiCode_inator.Configuration;
using SiraUtil.Logging;
using Zenject;

namespace MultiCode_inator.Utils
{
	internal class BspBroadcaster : IInitializable, IDisposable
	{
		private readonly SiraLog _siraLog;
		private readonly PluginConfig _pluginConfig;
		private readonly MultiplayerSettingsPanelControllerPatch _multiplayerSettingsPanelControllerPatch;

		public BspBroadcaster(SiraLog siraLog, PluginConfig pluginConfig, MultiplayerSettingsPanelControllerPatch multiplayerSettingsPanelControllerPatch)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_multiplayerSettingsPanelControllerPatch = multiplayerSettingsPanelControllerPatch;
		}

		public void Initialize()
		{
			Service.Acquire();
			Service.Multiplexer.OnTextMessageReceived += MultiplexerOnOnTextMessageReceived;
			
			_multiplayerSettingsPanelControllerPatch.CodeSetEvent += MultiplayerSettingsPanelControllerPatchOnCodeSetEvent;
		}

		public void Dispose()
		{
			_multiplayerSettingsPanelControllerPatch.CodeSetEvent -= MultiplayerSettingsPanelControllerPatchOnCodeSetEvent;
			
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

		private void MultiplayerSettingsPanelControllerPatchOnCodeSetEvent(string code)
		{
			if (_pluginConfig.PostCodeOnLobbyJoin)
			{
				_siraLog.Info($"Joined lobby with code: {code}");
				try
				{
					foreach (var channel in Service.Multiplexer.Channels)
					{
						Service.Multiplexer.SendTextMessage(channel.Item2, code);
					}

					_siraLog.Info("Successfully posted code");
				}
				catch (Exception e)
				{
					_siraLog.Critical(e);
				}
			}
		}
	}
}