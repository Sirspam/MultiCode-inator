using System;
using System.Threading;
using MultiCode_inator.Configuration;
using MultiCode_inator.Utils;
using SiraUtil.Logging;
using Zenject;

namespace MultiCode_inator.Managers
{
	internal class BroadcastManager : IInitializable, IDisposable
	{
		public event Action<string>? RequestBroadcastMessageToAllChannelsEvent;
		public event Action<object, string>? RequestBroadcastResponseMessageEvent;

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

		private string PlayerUsername => _playerUsername ??= _platformUserModel.GetUserInfo(CancellationToken.None).Result.userName;

		public void ReceivedMessage(object channel, string message, string senderUsername)
		{
			if (message.ToLower() != "!mc" && message.ToLower() != "!multicode") 
				return;
			
			_siraLog.Info("Received MultiCode command");
			if (_pluginConfig.CommandEnabled && MultiCodeFields.RoomCode != null)
			{
				RequestBroadcastResponseMessageEvent?.Invoke(channel, $"! {senderUsername}, The current multiplayer lobby code is {MultiCodeFields.RoomCode}");
			}
			else
			{
				RequestBroadcastResponseMessageEvent?.Invoke(channel, $"! {senderUsername}, The MultiCode command is currently disabled!");
			}
		}

		private void LobbyCodeUpdated(string? code)
		{
			if (_pluginConfig.PostCodeOnLobbyJoin && code != null)
			{
				_siraLog.Info($"Joined lobby with code: {code}");
				RequestBroadcastMessageToAllChannelsEvent?.Invoke($"! {PlayerUsername} has joined lobby {code}");	
			}
		}

		public void Initialize() => MultiCodeFields.LobbyCodeUpdated += LobbyCodeUpdated;

		public void Dispose() => MultiCodeFields.LobbyCodeUpdated -= LobbyCodeUpdated;
	}
}