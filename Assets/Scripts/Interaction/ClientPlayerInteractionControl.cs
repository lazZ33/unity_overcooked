using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientPlayerInteractionControl : ClientInteractable, IClientHolder
{
    private new ServerPlayerInteractionControl _server => (ServerPlayerInteractionControl)base._server;


	private ClientInteractable _targetInteractable;


	IHolderSO IClientHolder.Info => (IHolderSO)base._info;


	public event EventHandler<HoldTakeEventArgs> OnHold;
	public event EventHandler<HoldTakeEventArgs> OnTake;


	public override void OnNetworkSpawn(){
        if (!this.IsClient) this.enabled = false;
    }

    void HighlightExpectedTargetInteractable(){
        if (!this.IsLocalPlayer) return;

        Transform curInteractableTransform = this._server.GetExpectedTargetInteractableTransform();
        if (curInteractableTransform == null) return;
        ClientInteractable curInteractable = curInteractableTransform.GetComponent<ClientInteractable>();
        if (this._targetInteractable == curInteractableTransform) return;

        // TODO: unhighlight previous _targetInteractable

        this._targetInteractable = curInteractable;

        // TODO: highlighting current _targetInteractable
        // Renderer renderer = cur_grabbable.GetComponent<Renderer>();
        // renderer.material.color = this._highlightingColor;
        // this._targetInteractableRenderer = renderer;
        return;
    }

    void Update()
    {
        if (!this.IsOwner) return;

        this.HighlightExpectedTargetInteractable();

        if (Input.GetButtonDown("Grab")) this._server.GrabDropActionServerRpc();
        //if (Input.GetButtonDown("Use")) this._server.UseActionServerRpc();
    }
}
