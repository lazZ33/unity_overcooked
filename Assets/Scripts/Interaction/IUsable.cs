using Unity;
using System;

public interface IUsable{

    // should return animation/data that should be known by the grabbing control but dependent on the IUsable(?)
    internal void OnUseServerInternal(ServerPlayerGrabbingControl grabbingControl);
    internal void OnUnuseServerInternal(ServerPlayerGrabbingControl grabbingControl);

    internal void OnUseHoldServerInternal(ServerPlayerGrabbingControl grabbingControl);
    internal void OnUseUnholdServerInternal(ServerPlayerGrabbingControl grabbingControl);

}