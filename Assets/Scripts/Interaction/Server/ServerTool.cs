using UnityEngine;
using Unity.Netcode;

public class ServerTool : ServerGrabbable, IUsable
{
    void IUsable.OnUseServerInternal(){
        print("OnUseServerInternal");
        return;
    }

    void IUsable.OnUnuseServerInternal(){
        print("OnUnuseServerInternal");
        return;
    }
}