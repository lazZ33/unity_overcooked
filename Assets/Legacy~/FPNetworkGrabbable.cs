// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;

// public class FPNetworkGrabbable : NetworkBehaviour{

//     private Rigidbody _rigidbody = null;
//     private Transform _targetTransform = null;
//     private NetworkVariable<bool> isGrabbed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);
//     private float lerpSpeed;

//     public void Awake(){
//         _rigidbody = GetComponent<Rigidbody>();
//     }

//     public void grab(Transform grabPositionTransform){
//         if (isGrabbed.Value) return;
//         this._targetTransform = grabPositionTransform;
//         this._rigidbody.useGravity = false;
//     }

//     public void drop(){
//         if (this._targetTransform == null) return;
//         this._targetTransform = null;
//         this._rigidbody.useGravity = true;
//     }

//     public void FixedUpdate(){
//         if (!(this._targetTransform == null)) {
//             _rigidbody.MovePosition(Vector3.Lerp(this.transform.position, _targetTransform.position, Time.deltaTime * lerpSpeed));
//             _rigidbody.MoveRotation(Quaternion.Lerp(this.transform.rotation, _targetTransform.rotation, Time.deltaTime * lerpSpeed));
//         }
//     }

// }