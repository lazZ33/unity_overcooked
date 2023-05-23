using UnityEngine;
using Unity.Netcode;

public class ServerTool : ServerGrabbable, IUsable
{
    void IUsable.OnUseServerInternal(ServerPlayerGrabbingControl grabbingControl)
    {
        return;
    }

    void IUsable.OnUnuseServerInternal(ServerPlayerGrabbingControl grabbingControl)
    {
        return;
    }

    void IUsable.OnUseHoldServerInternal(ServerPlayerGrabbingControl grabbingControl)
    {
        return;
    }

    void IUsable.OnUseUnholdServerInternal(ServerPlayerGrabbingControl grabbingControl)
    {
        return;
    }
}