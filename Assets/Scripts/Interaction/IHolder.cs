using Unity;
using System;

public interface IHolder{

    internal void OnPlaceServerInternal(ServerGrabbable targetGrabbable);
    internal void OnTakeServerInternal(out ServerGrabbable targetGrabbable);

}