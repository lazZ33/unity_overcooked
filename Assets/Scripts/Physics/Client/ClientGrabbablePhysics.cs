using Unity;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;
using System;

public class ClientGrabbablePhysics: ClientInteractablePhysics{
    [SerializeField] private Rigidbody _rigidBody;
    protected ClientGrabbable _grabbableControl => (ClientGrabbable)base._interactableControl;

    public override void OnNetworkSpawn(){
        if (this.IsClient && !this.IsHost) this._meshCollider.enabled = false;
    }
}