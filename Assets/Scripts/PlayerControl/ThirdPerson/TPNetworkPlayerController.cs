using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TPNetworkPlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float BoostForce = 4;
    [SerializeField] private float minBoostInterval = 0.25f;
    [SerializeField] private Animator animator = null;
    [SerializeField] private Rigidbody rigidBody = null;
    [SerializeField] private LayerMask groundMask;
    private float targetAngle = 0;
    private float BoostTimeStamp = 0;

    private List<Collider> collisions = new List<Collider>();

    private void Awake()
    {
        if (!this.animator) { this.animator = gameObject.GetComponent<Animator>(); }
        if (!this.rigidBody) { this.rigidBody = gameObject.GetComponent<Rigidbody>(); }
        // Cursor.lockState = CursorLockMode.Locked;
    }
    
    // private void Update()
    // {
    //     grounded = Physics.Raycast(this.transform.position, Vector3.down, 0.5f + 0.2f, groundMask);
    //     this.rigidBody.drag = groundDrag;
    // }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        ThridPersonUpdate();
    }

    private void ThridPersonUpdate()
    {
        // get input
        float key_z_position_input = Input.GetAxisRaw("Vertical");
        float key_x_position_input = Input.GetAxisRaw("Horizontal");
        Vector3 force = Vector3.forward * key_z_position_input + Vector3.right * key_x_position_input;

        if (force != Vector3.zero)
        {
            // update rotation
            force = force.normalized;
            this.targetAngle = Vector3.SignedAngle(Vector3.forward, force, Vector3.up);
            if (Input.GetButtonDown("Boost") && (Time.time - this.minBoostInterval > this.BoostTimeStamp))
            {
                this.rigidBody.AddForce(force * BoostForce * 10f, ForceMode.VelocityChange);
                this.BoostTimeStamp = Time.time;
            } 
            else{
                // update position
                force *= this.moveSpeed;
                force.y = 0;
                this.rigidBody.AddForce(force * 100f, ForceMode.Force);
            }
        }
        // print(this.targetAngle + " " + Quaternion.Euler(0, this.targetAngle, 0) + " " + this.transform.rotation + " " + Time.deltaTime * 10f);
        this.rigidBody.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, this.targetAngle, 0), Time.deltaTime * 10f); 
        // this.animator.SetFloat("MoveSpeed", force.magnitude);
    }
}
