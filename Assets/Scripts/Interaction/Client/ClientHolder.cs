using UnityEngine;
using Unity;
using Unity.Netcode;

public abstract class ClientHolder: ClientInteractable{
    [SerializeField] private ServerHolder _server;

    // public virtual void RequestPlace(ClientGrabbable targetGrabbable, PlayerGrabbingControl grabbingControl){
    //     if (this.IsHoldingGrabbable) return;
    //     print("baes Place");

    //     this._server.PlaceServerRpc(targetGrabbable.NetworkObjectReferenceBuf, grabbingControl.OwnerClientId);
    // }

    // public virtual void RequestTake(PlayerGrabbingControl grabbingControl){
    //     if (!this.IsHoldingGrabbable) return;
    //     print("base Take");

    //     this._server.TryTransferGrabbableControlTo(grabbingControl);
    // }
}