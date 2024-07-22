using Mirror;
using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	public static MainMenu singleton = null;

	public Transform clientsRoot;

	[SerializeField]
	ApplicationStruct.EventBool OnReadyChange;

	private void Awake()
	{
		if (singleton == null)
			singleton = this;
	}
	private void Start()
	{
		if (singleton == this)
		{
			if (NetworkManager.singleton is GameNetworkLobbyManager)
			{
				GameNetworkLobbyManager gameNetwork = NetworkManager.singleton as GameNetworkLobbyManager;
				gameNetwork.OnPlayerReadysChange += OnReadyChange.Invoke;
			}
		}
	}

	private void OnDestroy()
	{
		if (singleton == this)
		{
			singleton = null;
			if (NetworkManager.singleton is GameNetworkLobbyManager)
			{
				GameNetworkLobbyManager gameNetwork = NetworkManager.singleton as GameNetworkLobbyManager;
				gameNetwork.OnPlayerReadysChange -= OnReadyChange.Invoke;
			}
		}
	}

	Uri uri;

	public void SetAddress(string url)
	{
		uri = new Uri(url);
	}

	public void Connect()
	{
		if (uri!=null)
			NetworkManager.singleton.StartClient(uri);
		else
			NetworkManager.singleton.StartClient();
	}

	public void StartHost()
	{
		NetworkManager.singleton.StartHost();
	}

	public void StartGame()
	{
		if (NetworkManager.singleton is GameNetworkLobbyManager)
		{
			GameNetworkLobbyManager gameNetwork = NetworkManager.singleton as GameNetworkLobbyManager;
			gameNetwork.RunGame();
		}
	}
}
