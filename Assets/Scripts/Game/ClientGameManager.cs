using System.Collections;
using System.Collections.Generic;
using System;
using Unity;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class ClientGameManager: NetworkBehaviour {

	[SerializeField] private ServerGameManager _server;

	//[ClientRpc]
	//internal void GameStateChangeCallbackClientRpc(string strKey, int previousScore, int currentScore, GameStateChangeCallbackID id)
	//{
	//    ICombinableSO newTargetDish;

	//    switch (id)
	//    {

	//    }

	//}

}