using System.Collections.Generic;
using UnityEngine;

public class UtilitySO: HolderSO{
    
    // should be called before any GrabbableSO's RegisterObject()
    public override void RegisterObject(){
        InteractableSO.ExistingInteractable.Add(this);
    }
}