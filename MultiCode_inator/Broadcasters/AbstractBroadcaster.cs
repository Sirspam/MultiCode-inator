using System;
using MultiCode_inator.Managers;
using SiraUtil.Logging;
using Zenject;

namespace MultiCode_inator.Broadcasters
{
	// Lord save me if I ever have to implement a chat service called "abstract"
	internal abstract class AbstractBroadcaster : IInitializable, IDisposable
	{
		protected readonly SiraLog SiraLog;
		protected readonly BroadcastManager BroadcastManager;

		protected AbstractBroadcaster(SiraLog siraLog, BroadcastManager broadcastManager)
		{
			SiraLog = siraLog;
			BroadcastManager = broadcastManager;
		}

		public virtual void Initialize()
		{
			BroadcastManager.RequestBroadcastResponseMessageEvent += BroadcastResponseMessage;
			BroadcastManager.RequestBroadcastMessageToAllChannelsEvent += BroadcastMessageToAllChannels;
		}

		public virtual void Dispose()
		{ 
			// Why compiler throw warning? :(
#pragma warning disable CS8601
			BroadcastManager.RequestBroadcastResponseMessageEvent -= BroadcastResponseMessage;
			BroadcastManager.RequestBroadcastMessageToAllChannelsEvent -= BroadcastMessageToAllChannels;
#pragma warning restore CS8601
		}

		protected abstract void BroadcastResponseMessage(object channel, string message);

		protected abstract void BroadcastMessageToAllChannels(string message);
	}
}