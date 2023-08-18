using System;
using Unity;
using Unity.Netcode;
using UnityEngine;

public interface IClientGrabbable : IClientInteractable
{
	public new IGrabbableSO Info { get; }

	public event EventHandler<GrabDropEventArgs> OnGrab;
	public event EventHandler<GrabDropEventArgs> OnDrop;

	public bool IsGrabbedByPlayer { get; }
	public bool IsGrabbedByLocal { get; }
	public bool CanPlaceOn(IServerHolder targetHolder);
}