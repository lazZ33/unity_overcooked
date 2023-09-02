using System;
using Unity;
using Unity.Netcode;
using UnityEngine;

public interface IServerGrabbable: IServerInteractable
{
	public new IGrabbableSO Info { get; }

	public event EventHandler<ServerGrabDropEventArgs> OnGrab;
	public event EventHandler<ServerGrabDropEventArgs> OnDrop;

	public bool IsGrabbedByPlayer { get; }
	public bool IsGrabbedByLocal { get; }
	public bool CanPlaceOn(IServerHolder targetHolder);

	internal void OnGrabServerInternal(IServerHolder targetHolder);
	internal void OnDropServerInternal();
}