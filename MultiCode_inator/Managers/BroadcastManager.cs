using System;
using MultiCode_inator.Configuration;
using MultiCode_inator.Utils;
using SiraUtil.Logging;

namespace MultiCode_inator.Managers
{
	internal class BroadcastManager
	{
		public Action<string> RequestBroadcastMessageToAllChannelsEvent = null!;
		public Action<object, string> RequestBroadcastResponseMessageEvent = null!;

		private string? _playerUsername;
		
		private readonly SiraLog _siraLog;
		private readonly PluginConfig _pluginConfig;
		private readonly IPlatformUserModel _platformUserModel;

		public BroadcastManager(SiraLog siraLog, PluginConfig pluginConfig, IPlatformUserModel platformUserModel)
		{
			_siraLog = siraLog;
			_pluginConfig = pluginConfig;
			_platformUserModel = platformUserModel;
		}

		private string PlayerUsername => _playerUsername ??= _platformUserModel.GetUserInfo().Result.userName;

		public void ReceivedMessage(object channel, string message, string senderUsername)
		{
			if (message.ToLower() == "!mc" || message.ToLower() == "!multicode")
			{
				_siraLog.Info("Received MultiCode command");
				if (_pluginConfig.CommandEnabled && StaticFields.RoomCode != null)
				{
					RequestBroadcastResponseMessageEvent.Invoke(channel, $"! {senderUsername}, The current multiplayer lobby code is {StaticFields.RoomCode}");
				}
				else
				{
					RequestBroadcastResponseMessageEvent.Invoke(channel, $"! {senderUsername}, The MultiCode command is currently disabled!");
				}
			}
		}

		public void LobbyCodeUpdated(string code)
		{
			if (_pluginConfig.PostCodeOnLobbyJoin)
			{
				_siraLog.Info($"Joined lobby with code: {code}");
				RequestBroadcastMessageToAllChannelsEvent.Invoke($"{PlayerUsername} has joined lobby {code}");	
			}
		}
	}
}