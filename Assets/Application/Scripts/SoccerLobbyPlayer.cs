using Mirror;
using Soccer;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoccerLobbyPlayer : NetworkBehaviour
{

	public static Dictionary<NetworkConnectionToClient, SoccerLobbyPlayer> Players = new Dictionary<NetworkConnectionToClient, SoccerLobbyPlayer>();

	[SyncVar(hook = nameof(NameHook))]
    string _playerName;
    [SyncVar(hook = nameof(SkinHook))]
    int _skinId;
    [SyncVar(hook = nameof(ReadyHook))]
    bool _isReady;
	[SyncVar(hook = nameof(ColorHook))]
	Color _color;

	[SerializeField]
	Image _image, _mainColor;
    [SerializeField]
    Sprite[] _skinsImages;

    [SerializeField]
    ApplicationStruct.EventBool _OnIsOwned;
    [SerializeField]
    TMPro.TMP_InputField _nameField;
	[SerializeField]
	Color _ColorReady, _ColorNotReady;
	[SerializeField]
	ApplicationStruct.EventColor _OnEventColorState;

	Vector3 _currentHSV = Vector3.one;

	public string Name => _playerName;
	public int SkinId => _skinId;
	public Color Color => _color;


	private void Start()
	{
		if (MainMenu.singleton)
		{
			transform.SetParent(MainMenu.singleton.clientsRoot, false);
		}
	}

	public void ToggleReady()
    {
		_isReady = !_isReady;
		_OnEventColorState?.Invoke(_isReady ? _ColorReady : _ColorNotReady);
		NetworkClient.connection.Send(new MessageReady() { isReady = _isReady });
	}

    public void SetName(string newName)
    {
		_playerName = newName;
	}

    public void SetSkin(int id)
    {
		_skinId = (_skinsImages.Length + id) % _skinsImages.Length;
		_image.sprite = _skinsImages[_skinId];
	}
	public void SetNextSkin(int id)
	{
        _skinId = (_skinsImages.Length + _skinId + id) % _skinsImages.Length;
		_image.sprite = _skinsImages[_skinId];
	}

    void NameHook(string oldNam, string newName)
	{
		_nameField.text = newName;
	}
	void SkinHook(int oldSkin,	int newSkin)
	{
		_image.sprite = _skinsImages[newSkin];
	}
	void ReadyHook(bool oldReady, bool newReady)
	{
		_OnEventColorState?.Invoke(_isReady ? _ColorReady : _ColorNotReady);
	}
	void ColorHook(Color oldReady, Color newReady)
	{
		_mainColor.color = _color;
	}
	

	public void DragData(BaseEventData eventData)
	{
		switch (eventData)
		{
			case PointerEventData pointerEventData:

				_currentHSV.x += pointerEventData.delta.x / Screen.width;
				_currentHSV.y += pointerEventData.delta.y / Screen.width;
				_currentHSV.y = Mathf.Clamp01(_currentHSV.y);
				_color = Color.HSVToRGB((_currentHSV.x + 1f) % 1f, _currentHSV.y, _currentHSV.z);
				_mainColor.color = _color;
				break;
		}
	}

	public override void OnStartServer()
	{
		Players.Add(netIdentity.connectionToClient, this);
	}
	public override void OnStartClient()
	{
        _OnIsOwned?.Invoke(isOwned);
	}
	public override void OnStartAuthority()
	{
		if (isOwned)
		{
			_nameField.onEndEdit.AddListener(SetName);
			_playerName = Environment.UserName;
			_nameField.text = _playerName;
			_isReady = false;
			_mainColor.color = _color = Color.white;
		}
	}
}
