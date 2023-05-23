using UnityEngine;
using Unity;
using Unity.Netcode;

public class ClientTable: ClientHolder{
    // TODO: dynamically change _holdHolder according to received map config
    // private ClientHolder _holdHolder = null;

    // public override void RequestPlace(ClientGrabbable targetGrabbable, PlayerGrabbingControl grabbingControl){
    //     print("table Place");
    //     if ( this._holdHolder != null ){
    //         this._holdHolder.RequestPlace(targetGrabbable, grabbingControl);
    //     }

    //     base.RequestPlace(targetGrabbable, grabbingControl);
    // }

    // public override void RequestTake(PlayerGrabbingControl grabbingControl){
    //     print("table Take");
    //     if (this._holdHolder != null ){
    //         this._holdHolder.RequestTake(grabbingControl);
    //     }

    //     base.RequestTake(grabbingControl);
    // }

}