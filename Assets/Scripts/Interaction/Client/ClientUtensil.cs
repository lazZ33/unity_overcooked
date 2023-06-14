using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ClientUtensil: ClientGrabbable{

    public new UtensilSO Info => (UtensilSO)base._info;
    private new UtensilSO _info { get{ return (UtensilSO)base._info; } set{ base._info = value; } }
    private new ServerUtensil _server => (ServerUtensil)base._server;

    
}