using UnityEngine;
using Unity.Netcode;
using System;

public abstract class ServerInteractControl: NetworkBehaviour
{
    public NetworkObjectReference NetworkObjectReferenceBuf { get; private set; }
    public NetworkObject NetworkObjectBuf { get; private set; }


    // shared dependencies to be injected in child class
    protected IServerInteractable _parentInstance { get; private set; }
	protected internal delegate IInteractableSO GetInfoFunc();
    //protected internal delegate void SetInfoFunc(IInteractableSO info);
    private GetInfoFunc _getInfo { get; set; } = null;
    //private SetInfoFunc _setInfo { get; set; } = null;


    protected bool IsDepsInitialized = false;
	protected IInteractableSO _info { get { return (IInteractableSO)this._getInfo(); } }


    // builder DI
    internal abstract class InteractControlInitArgs
    {
        internal ServerInteractable ParentInstance;
        internal GetInfoFunc GetInfoFunc;
        //internal SetInfoFunc SetInfoFunc;
        internal void AddParentInstance(ServerInteractable parentInstance) => this.ParentInstance = parentInstance;
        internal void AddGetInfoFunc(GetInfoFunc getInfoFunc) => this.GetInfoFunc = getInfoFunc;
        //internal void AddSetInfoFunc(SetInfoFunc setInfoFunc) => this.SetInfoFunc = setInfoFunc;
	}
    internal virtual void DepsInit(InteractControlInitArgs args)
    {
		this.IsDepsInitialized = true;

		this._getInfo = args.GetInfoFunc;
        //this._setInfo = args.SetInfoFunc;
        this._parentInstance = args.ParentInstance;
    }


	protected virtual void Awake(){
        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
    }

	protected virtual void Start()
	{
        if (this._getInfo == null)
            throw new MissingReferenceException(String.Format("Info not initialized before Start(), parent instance: {0}", this._parentInstance));

		if (this._parentInstance == null)
            throw new MissingReferenceException("ParentInstance not initialized before Start()");
        if (!this.IsDepsInitialized)
            throw new MissingReferenceException(String.Format("Dependencies not initialized before Start(), parent instance: {0}", this._parentInstance));
	}

	public override void OnNetworkSpawn(){
        if (!IsServer) this.enabled = false;

        if (this.NetworkObjectBuf == null) this.NetworkObjectBuf = this.NetworkObject;
        this.NetworkObjectReferenceBuf = new NetworkObjectReference(this.NetworkObjectBuf);
    }


}