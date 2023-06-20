using UnityEngine;
using Unity.Netcode;

public class ServerTool : ServerGrabbable, IUsable
{
    void IUsable.OnUseServerInternal(){
        return;
    }

    void IUsable.OnUnuseServerInternal(){
        return;
    }
}