using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ServerInteractableSharedData{
    public InteractableCompositionSO Info = null;
    internal ServerInteractableComposition HoldGrabbable = null;

    internal static readonly ulong GRABBED_CLIENT_DEFAULT = ulong.MaxValue;
    internal NetworkVariable<ulong> GrabbedClientId { get; } = new NetworkVariable<ulong>(ServerGrabDropControl.GRABBED_CLIENT_DEFAULT, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    internal static readonly FixedString128Bytes INFO_STR_KEY_DEFAULT = "";
    internal NetworkVariable<FixedString128Bytes> InfoStrKey = new NetworkVariable<FixedString128Bytes>(ServerInteractableSharedData.INFO_STR_KEY_DEFAULT, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
}