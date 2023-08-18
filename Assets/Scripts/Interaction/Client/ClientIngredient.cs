using Unity;
using System;

public class ClientIngredient: ClientInteractable, IClientCombinable
{
	private new ServerIngredient _server => (ServerIngredient)base._server;
	private new IngredientSO Info => (IngredientSO)base._info;
	private new IngredientSO _info => (IngredientSO)base._info;


	ICombinableSO IClientCombinable.Info => (ICombinableSO)base._info;
}