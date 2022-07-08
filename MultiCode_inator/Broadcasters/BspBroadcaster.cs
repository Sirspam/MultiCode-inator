using System;
using BeatSaberPlus.SDK.Chat;
using BeatSaberPlus.SDK.Chat.Interfaces;
using MultiCode_inator.Managers;
using SiraUtil.Logging;

namespace MultiCode_inator.Broadcasters
{
	internal class BspBroadcaster : AbstractBroadcaster
	{
		public BspBroadcaster(SiraLog siraLog, BroadcastManager broadcastManager) : base(siraLog, broadcastManager)
		{
		}

		public override void Initialize()
		{
			Service.Acquire();
			Service.Multiplexer.OnTextMessageReceived += MultiplexerOnOnTextMessageReceived;
			
			base.Initialize();
		}

		private void MultiplexerOnOnTextMessageReceived(IChatService chatService, IChatMessage chatMessage)
		{
			BroadcastManager.ReceivedMessage(chatMessage, chatMessage.Message, chatMessage.Sender.UserName);
		}

		public override void Dispose()
		{
			base.Dispose();
			
			Service.Multiplexer.OnTextMessageReceived -= MultiplexerOnOnTextMessageReceived;
			Service.Release();
		}

		protected override void BroadcastResponseMessage(object channel, string message)
		{
			try
			{
				var castedChannel = (IChatMessage) channel;
				Service.Multiplexer.SendTextMessage(castedChannel.Channel, message);
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
				foreach (var channel in Service.Multiplexer.Channels)
				{
					Service.Multiplexer.SendTextMessage(channel.Item2, message);
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