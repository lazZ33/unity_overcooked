// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Unity.Netcode;

// public class FPNetworkGrappingControl : NetworkBehaviour
// {
//     [SerializeField] private float maxDistance = 2f;
//     [SerializeField] private LayerMask m_layerMask;

//     private Camera _camera = null;
//     private FPNetworkGrabbable grabbable = null;
//     void Start()
//     {
//         this._camera = Camera.main;
//     }
//     void Update()
//     {
//         if (!IsOwner) return;

//         if (Input.GetButton("Grab")){
//             if(!Physics.Raycast(this._camera.transform.position, this._camera.transform.forward, out RaycastHit raycastHit, this.maxDistance, m_layerMask)) return;
//             if (!raycastHit.transform.TryGetComponent(out grabbable)) return;
//             this.grabbable.grab(this.transform);
//         }
//         else if (grabbable != null) {
//             this.grabbable.drop();
//             this.grabbable = null;
//         }
//     }
// }
