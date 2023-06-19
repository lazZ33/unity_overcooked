using System;
using UnityEngine;
using Unity.Netcode;

[Serializable]
[CreateAssetMenu(fileName = "Utensil", menuName = "ScriptableObject/Utensil")]
public class UtensilSO : GrabbableSO, IHolderSO
{
    [SerializeField] private UsableHolderSO _bindingUsableHolder;
    internal UsableHolderSO BindingUsableHolder => this._bindingUsableHolder;

    public static new UtensilSO GetSO(string strKey) => (UtensilSO)InteractableSO.GetSO(strKey);
    public static new UtensilSO TryGetSO(string strKey) => (UtensilSO)InteractableSO.TryGetSO(strKey);


}
