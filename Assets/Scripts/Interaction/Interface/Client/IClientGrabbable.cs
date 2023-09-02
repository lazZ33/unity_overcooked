using System;
using Unity;
using Unity.Netcode;
using UnityEngine;

public interface IClientGrabbable : IClientInteractable
{
	public new IGrabbableSO Info { get; }

	public event EventHandler<ClientGrabDropEventArgs> OnGrab;
	public event EventHandler<ClientGrabDropEventArgs> OnDrop;

	public bool CanPlaceOn(IServerHolder targetHolder);
}