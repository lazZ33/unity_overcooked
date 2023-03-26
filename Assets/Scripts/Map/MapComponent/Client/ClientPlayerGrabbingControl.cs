using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientPlayerGrabbingControl : NetworkBehaviour
{

    [SerializeField] private ServerPlayerGrabbingControl _server;
    private ClientInteractable _targetInteractable;
    // private Color32 _highlightingColor = new Color32((byte)0, (byte)255, (byte)255, (byte)0);

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

        switch(curInteractable){
            case ClientGrabbable curGrabbable:
                // print("found grabbable");
                this._targetInteractable = curGrabbable;
                break;
            case ClientSpawnable curSpawnable:
                // print("found spawnable");
                this._targetInteractable = curSpawnable;
                break;
            case ClientHolder curHolder:
                // print("found holder");
                this._targetInteractable = curHolder;
                break;
            default:
                break;
        }
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
    }
}
