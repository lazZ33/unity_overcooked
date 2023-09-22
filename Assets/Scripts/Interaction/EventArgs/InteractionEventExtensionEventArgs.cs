

using System;

internal class InteractionEventExtensionEventArgs: EventArgs {
	internal InteractionEventExtensionEventArgs(InteractionCallbackID id, IInteractableSO info) { this.Id = id; this.Info = info; }
	internal InteractionCallbackID Id;
	internal IInteractableSO Info;
}