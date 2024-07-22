using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soccer
{
	public struct MessagePlayerData : NetworkMessage {

		public string name;
		public int skinID;
		public Color color;
	}

	public struct MessageReady : NetworkMessage {
		public bool isReady;
	}
}
