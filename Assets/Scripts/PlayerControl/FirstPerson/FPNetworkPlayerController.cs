// using System.Collections.Generic;
// using Unity.Netcode;
// using UnityEngine;

// public class FPNetworkPlayerController : NetworkBehaviour
// {
//     [SerializeField] private float moveSpeed = 2;
//     [SerializeField] private float jumpForce = 4;
//     [SerializeField] private Animator animator = null;
//     [SerializeField] private Rigidbody rigidBody = null;
//     private Camera _camera = null;

//     private readonly float sensX = 100f;
//     private readonly float sensY = 100f;
//     private readonly float walkScale = 2.0f;

//     private float xRotation = 0f;
//     private float yRotation = 0f;
//     private Vector3 cameraOffset;
//     private float jumpTimeStamp = 0;
//     private float minJumpInterval = 0.25f;
//     private bool jumpInput = false;
    
//     private bool wasGrounded = true;
//     private bool isGrounded = true;

//     private List<Collider> collisions = new List<Collider>();

//     private void Awake()
//     {
//         if (!this.animator) { this.animator = gameObject.GetComponent<Animator>(); }
//         if (!this.rigidBody) { this.rigidBody = gameObject.GetComponent<Rigidbody>(); }
//         if (!this._camera) { this._camera = Camera.main; }
        
//         this.cameraOffset = new Vector3(0, 2.5f, 0);
//         // Cursor.lockState = CursorLockMode.Locked;
//     }
    
//     private void Update()
//     {
//         if (!this.jumpInput && Input.GetKey(KeyCode.Space))
//         {
//             this.jumpInput = true;
//         }

//         // update camera position
//         this._camera.transform.position = this.transform.position + this.cameraOffset;
//     }

//     private void FixedUpdate()
//     {
//         if (!IsOwner) return;

//         this.animator.SetBool("Grounded", this.isGrounded);

//         FirstPersonUpdate();

//         this.wasGrounded = this.isGrounded;
//         this.jumpInput = false;
//     }

//     private void FirstPersonUpdate()
//     {
//         // update camera rotation
//         float mouse_x_rotation_input = Input.GetAxisRaw("Mouse X") * Time.deltaTime * this.sensX;
//         float mouse_y_rotation_input = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * this.sensY;

//         this.xRotation -= mouse_y_rotation_input;
//         this.xRotation = Mathf.Clamp(this.xRotation, -90f, 90f);
//         this.yRotation = this.yRotation + mouse_x_rotation_input;
//         this._camera.transform.rotation = Quaternion.Euler(this.xRotation, this.yRotation, 0);

//         // follow camera y rotation
//         this.transform.rotation = Quaternion.Euler(0, this.yRotation, 0);


//         if (!this.isGrounded) return;
//         // update displacement
//         float key_z_position_input = Input.GetAxisRaw("Vertical");
//         float key_x_position_input = Input.GetAxisRaw("Horizontal");

//         print(this.transform.forward + " " + key_z_position_input + " " + this.transform.right + " " + key_x_position_input);
//         Vector3 force = this.transform.forward * key_z_position_input + this.transform.right * key_x_position_input;

//         if (force != Vector3.zero)
//         {
//             force = force.normalized * this.moveSpeed;
//             if (Input.GetKey(KeyCode.LeftShift)) {force *= this.walkScale;}

//             // force = Vector3.Lerp(this.rigidBody.velocity, force, Time.deltaTime);
//             force.y = 0;
//             rigidBody.AddForce(force, ForceMode.Force);

//         }
//         this.animator.SetFloat("MoveSpeed", force.magnitude);

//         JumpingAndLanding();
//     }

//     private void JumpingAndLanding()
//     {
//         bool jumpCooldownOver = (Time.time - this.jumpTimeStamp) >= this.minJumpInterval;

//         if (jumpCooldownOver && this.isGrounded && this.jumpInput)
//         {
//             this.jumpTimeStamp = Time.time;
//             this.rigidBody.AddForce(Vector3.up * this.jumpForce, ForceMode.Impulse);
//         }

//         if (!this.wasGrounded && this.isGrounded)
//         {
//             // Jump not resetting due to problem of triggering sequence, need fix 
//             // https://answers.unity.com/questions/981044/animator-trigger-not-reseting-bug.html
//             this.animator.ResetTrigger("Jump"); 
//         }

//         if (!this.isGrounded && this.wasGrounded)
//         {
//             this.animator.SetTrigger("Jump");
//         }
//     }

//     // private void OnCollisionEnter(Collision collision)
//     // {
//     //     ContactPoint[] contactPoints = collision.contacts;
//     //     for (int i = 0; i < contactPoints.Length; i++)
//     //     {
//     //         if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
//     //         {
//     //             if (!this.collisions.Contains(collision.collider))
//     //             {
//     //                 this.collisions.Add(collision.collider);
//     //             }
//     //             this.isGrounded = true;
//     //         }
//     //     }
//     // }

//     // private void OnCollisionStay(Collision collision)
//     // {
//     //     ContactPoint[] contactPoints = collision.contacts;
//     //     bool validSurfaceNormal = false;
//     //     for (int i = 0; i < contactPoints.Length; i++)
//     //     {
//     //         if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
//     //         {
//     //             validSurfaceNormal = true; break;
//     //         }
//     //     }

//     //     if (validSurfaceNormal)
//     //     {
//     //         this.isGrounded = true;
//     //         if (!this.collisions.Contains(collision.collider))
//     //         {
//     //             this.collisions.Add(collision.collider);
//     //         }
//     //     }
//     //     else
//     //     {
//     //         if (this.collisions.Contains(collision.collider))
//     //         {
//     //             this.collisions.Remove(collision.collider);
//     //         }
//     //         if (this.collisions.Count == 0) { this.isGrounded = false; }
//     //     }
//     // }

//     // private void OnCollisionExit(Collision collision)
//     // {
//     //     if (this.collisions.Contains(collision.collider))
//     //     {
//     //         this.collisions.Remove(collision.collider);
//     //     }
//     //     if (this.collisions.Count == 0) { this.isGrounded = false; }
//     // }


//     // private void TankUpdate()
//     // {
//     //     float v = Input.GetAxis("Vertical");
//     //     float h = Input.GetAxis("Horizontal");

//     //     bool walk = Input.GetKey(KeyCode.LeftShift);

//     //     if (v < 0)
//     //     {
//     //         if (walk) { v *= this.backwardsWalkScale; }
//     //         else { v *= this.backwardRunScale; }
//     //     }
//     //     else if (walk)
//     //     {
//     //         v *= this.walkScale;
//     //     }

//     //     this.currentV = Mathf.Lerp(this.currentV, v, Time.deltaTime * this.interpolation);
//     //     this.currentH = Mathf.Lerp(this.currentH, h, Time.deltaTime * this.interpolation);

//     //     transform.position += transform.forward * this.currentV * this.moveSpeed * Time.deltaTime;
//     //     transform.Rotate(0, this.currentH * this.turnSpeed * Time.deltaTime, 0);

//     //     this.animator.SetFloat("MoveSpeed", this.currentV);

//     //     JumpingAndLanding();
//     // }
// }
