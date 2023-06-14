using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

internal class ClientSpawnControl: ClientInteractControl{
    [SerializeField] private ServerSpawnControl _server;
}