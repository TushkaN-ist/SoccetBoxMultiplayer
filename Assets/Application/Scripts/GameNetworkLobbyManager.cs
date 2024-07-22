using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Soccer;
using UnityEngine.SceneManagement;
using System;

public class GameNetworkLobbyManager : NetworkManager
{
	[SerializeField,Range(1,4)]
	int _minPlayers = 1;

	[Scene,SerializeField]
	string GameScene;

	static Dictionary<NetworkConnectionToClient, MessagePlayerData> _playersInfo = new Dictionary<NetworkConnectionToClient, MessagePlayerData>();

	int _readyCount = 0;

	public Action<bool> OnPlayerReadysChange;

	public static MessagePlayerData GetPlayerInfo(NetworkConnectionToClient client)
	{
		return _playersInfo[client];
	}

	public override void OnStartHost()
	{
		ServerSetup();
		base.OnStartHost();
		OnPlayerReadysChange?.Invoke(false);
	}

	public override void OnStartServer()
	{
		ServerSetup();
		base.OnStartServer();
		OnPlayerReadysChange?.Invoke(false);
	}

	void ServerSetup()
	{
		NetworkServer.RegisterHandler<MessageReady>(ReadyState);
	}
	public override void OnStartClient()
	{
		base.OnStartClient();
	}

	void ReadyState(NetworkConnectionToClient client, MessageReady playerData)
	{
		if (playerData.isReady)
			_readyCount += 1;
		else
			_readyCount -= 1;
		OnPlayerReadysChange?.Invoke(_readyCount == NetworkServer.connections.Count);
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		if (offlineScene == SceneManager.GetActiveScene().path)
			base.OnServerAddPlayer(conn);
	}

	[Server]
	public void RunGame()
	{
		if (NetworkServer.active && NetworkServer.connections.Count >= _minPlayers && _readyCount == NetworkServer.connections.Count)
		{
			_playersInfo.Clear();
			foreach (var connect in NetworkServer.connections)
			{
				SoccerLobbyPlayer lobbyPlayer = SoccerLobbyPlayer.Players[connect.Value];

				_playersInfo.Add(connect.Value, new MessagePlayerData()
				{
					name = lobbyPlayer.Name,
					skinID = lobbyPlayer.SkinId,
					color = lobbyPlayer.Color
				});
			}
			maxConnections = NetworkServer.connections.Count;
			SoccerLobbyPlayer.Players.Clear();
			ServerChangeScene(GameScene);
		}
	}
}
