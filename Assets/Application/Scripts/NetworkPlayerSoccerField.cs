using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkPlayerSoccerField : NetworkBehaviour
{
	static Dictionary<int, int> _fields = new Dictionary<int, int>();

	static Dictionary<Collider, NetworkPlayerSoccerField> goalTriggers = new Dictionary<Collider, NetworkPlayerSoccerField>();

	public static bool GoalTrigged(Collider collider, NetworkPlayerSoccerField fromField)
	{
		if (goalTriggers.TryGetValue(collider, out NetworkPlayerSoccerField field) && fromField != field)
		{
			if (fromField != null)
				fromField._scores += 1;
			field._scores -= 1;
			return true;
		}
		return false;
	}
	[SyncVar(hook = nameof(HookColor))]
	Color _color;

	[SerializeField]
	int idPlayer;

	[SyncVar(hook = nameof(UpdateScore))]
	int _scores;

	[SerializeField]
	TMPro.TextMeshPro _scoreText;

	[SerializeField]
	SoccerTrajectory _soccerPrefab;

	[SerializeField]
	PlayerSoccerMovement _playerMovement;

	[SerializeField]
	Collider[] _triggers;

	[SerializeField]
	Renderer[] _renderer;

	[Command]
	public void Launch(Vector4 power)
	{
		SoccerTrajectory prefab = Instantiate(_soccerPrefab, _playerMovement.LaunchPoint.position, _playerMovement.LaunchPoint.rotation);
		prefab.SetTraectory(_playerMovement.LaunchPoint.forward * power.w, power);
		prefab.field = this;
		NetworkServer.Spawn(prefab.netIdentity.gameObject);
	}
	[Server]
	public void LaunchSrv(Vector4 power)
	{
		SoccerTrajectory prefab = Instantiate(_soccerPrefab, _playerMovement.LaunchPoint.position, _playerMovement.LaunchPoint.rotation);
		prefab.SetTraectory(_playerMovement.LaunchPoint.forward * power.w, power);
		prefab.field = this;
		NetworkServer.Spawn(prefab.netIdentity.gameObject);
	}

	void UpdateScore(int oldScore, int newScore)
	{
		if (_scoreText != null)
			_scoreText.text = newScore.ToString();
	}

	void LaunchCliented(Vector4 power)
	{
		if (isServer)
			LaunchSrv(power);
		else
			Launch(power);
	}
	private void Start()
	{
		_playerMovement.OnLaunch += LaunchCliented;
	}

	[TargetRpc]
	void ActivePlayer()
	{
		_playerMovement.enabled = true;
	}

	void HookColor(Color oldColor, Color newColor)
	{
		foreach (Renderer renderer in _renderer)
		{
			MaterialPropertyBlock property = new MaterialPropertyBlock();
			property.SetColor("_Color", newColor);
			renderer.SetPropertyBlock(property);
		}
	}

	public override void OnStartServer()
	{
		_scores = 100;
		foreach (Collider trigger in _triggers)
		{
			goalTriggers.Add(trigger, this);
		}
		_playerMovement.enabled = false;
		SetPlayer();
		NetworkServer.OnConnectedEvent += ConnectPlayer;
		NetworkServer.OnDisconnectedEvent -= DisconnectPlayer;
	}

	public override void OnStartClient()
	{
		if (!isOwned)
			_playerMovement.enabled = false;
		UpdateScore(0, _scores);
		HookColor(Color.white, _color);
		base.OnStartClient();
	}

	void SetPlayer()
	{
		int id = 0;
		foreach (var player in NetworkServer.connections)
		{
			if (id++ == idPlayer)
			{
				_fields[player.Value.connectionId] = idPlayer;
				Debug.Log(player.Value.connectionId+":"+idPlayer);
				player.Value.authenticationData = netIdentity.gameObject;
				NetworkServer.ReplacePlayerForConnection(player.Value,gameObject);
				_color = GameNetworkLobbyManager.GetPlayerInfo(connectionToClient).color;
				ActivePlayer();
				break;
			}
		}
	}

	void ConnectPlayer(NetworkConnectionToClient client)
	{
		if (netIdentity.connectionToClient == null && _fields.TryGetValue(client.connectionId, out int id))
		{
			_fields[client.connectionId] = idPlayer;
			NetworkServer.ReplacePlayerForConnection(client, gameObject);
			ActivePlayer();
		}
	}

	void DisconnectPlayer(NetworkConnectionToClient client)
	{
		if (netIdentity.connectionToClient == client)
		{
			netIdentity.RemoveClientAuthority();
		}
	}

	public override void OnStopAuthority()
	{
		if (netIdentity.connectionToClient!=null)
			_fields.Remove(netIdentity.connectionToClient.connectionId);
		netIdentity.RemoveClientAuthority();
		_playerMovement.enabled = false;
	}
}
