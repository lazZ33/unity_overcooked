using Unity;
using System;

public interface IUsable{

    // should return animation/data that should be known by the grabbing control but dependent on the IUsable(?)
    internal void OnUseServerInternal();
    internal void OnUnuseServerInternal();
}