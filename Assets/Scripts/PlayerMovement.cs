using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private bool jumped = false;
    private bool pressedJump = false;

    private bool isFalling = false;

    private float faceDirectionX = 1f;
    private float faceDirectionY = 0;
    private float moveX = 0;
    private float moveY = 0;

    private float lastY = 0;
    private float lastZ = 0;

    public float runSpeed;

    bool freshlySpawned = true;

    // Update is called once per frame
    
    void Start()
    {
        lastY = GetComponent<Rigidbody>().position.y;
        lastZ = GetComponent<Rigidbody>().position.z;
    }

    private void Attack()
    {
        // spawn collider in front of player
        GameObject attackRange = Instantiate(Resources.Load("Prefabs/AttackRange") as GameObject, this.transform.position + new Vector3(0.125f * (faceDirectionX/Mathf.Abs(faceDirectionX)), -0.514f, 0), Quaternion.identity);
        GameObject.Destroy(attackRange, 0.2f);
    }
    

    void Update()
    {
        Ray ray = new Ray(transform.position, -Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction * 10);

        var collider2d = GetComponent<BoxCollider>();
        var rigidbody2D= GetComponent<Rigidbody>();
        var animator = GetComponent<Animator>();

        float lastMoveDirection = faceDirectionX;

        bool isGrounded = IsGrounded();

        if (isGrounded && freshlySpawned)
            freshlySpawned = false;

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Punched");
            Attack();
            return;
        }

        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");

        if (moveX != 0)
            faceDirectionX = moveX;

        if (moveY != 0)
            faceDirectionY = moveY;


        if (!isGrounded)
        {
            if (!isFalling)
            {
                if (rigidbody2D.velocity.y < 0)
                {
                    isFalling = true;
                    animator.SetTrigger("Falling");
                }
                    
            }

        }
        else
        {
            if (isFalling)
            {
                isFalling = false;
                animator.SetTrigger("Landed");
                jumped = false;
            }
        }

        

        if (isGrounded)
        {
            if (!jumped && Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody2D.drag = 0f;
                animator.SetTrigger("Jumped");
                rigidbody2D.AddForce(Vector3.up * 6, ForceMode.Impulse);
                jumped = true;

                rigidbody2D.useGravity = true;
            }
            else
            {
                

                if (moveX < 0.5 && moveX > -0.5 && moveY < 0.5 && moveY > -0.5)
                {
                    animator.SetBool("IsRunning", false);
                }
                else
                {
                    animator.SetBool("IsRunning", true);   
                }
            }

            
        }

        
        if (lastMoveDirection * faceDirectionX < 0 || lastMoveDirection == 0)
        {
            if (faceDirectionX >= 0)
            {
                animator.SetFloat("Direction", 1);
            }
            else
            {
                animator.SetFloat("Direction", 0);
            }
        }

        

        

    }

    void FixedUpdate()
    {
        bool isGrounded = IsGrounded();

        var collider2d = GetComponent<BoxCollider>();
        var rigidBody = GetComponent<Rigidbody>();

        if (TouchingWall(moveY))
        {
            moveY = 0f;
        }


        if (moveY == 0f)
        {
            var rPos = rigidBody.position;
            rPos.z = lastZ;

            rigidBody.position = rPos;
        }

        rigidBody.velocity = new Vector3(moveX, 0, moveY * 2) * Time.deltaTime * runSpeed + new Vector3(0f, rigidBody.velocity.y, 0f);

        lastY = rigidBody.position.y;
        lastZ = rigidBody.position.z;
        

    }



    public void Land()
    {
        var animator = GetComponent<Animator>();

        animator.SetBool("IsRunning", false);
    }

    private bool IsGrounded()
    {
        if (!freshlySpawned && !jumped)
            return true;

        var collider2d = GetComponent<BoxCollider>();

        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, - Vector3.up);

        if (Physics.Raycast(ray, out hitInfo, 10f, 1 << LayerMask.NameToLayer("Floor")))
        {
            return hitInfo.distance < 0.9f && hitInfo.distance > 0;
        }

        return false;
    }

    private bool TouchingWall(float directionY)
    {
        var collider2d = GetComponent<BoxCollider>();

        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, transform.position + Vector3.forward);

        if (directionY > 0 && Physics.Raycast(ray, out hitInfo, 0.2f, 1 << LayerMask.NameToLayer("Wall")) )
        {
            return hitInfo.distance < 0.1f && hitInfo.distance > 0;
        }

        ray = new Ray(transform.position, transform.position - Vector3.forward);

        if (directionY <= 0 && Physics.Raycast(ray, out hitInfo, 0.5f, 1 << LayerMask.NameToLayer("Wall")))
        {
            return hitInfo.distance < 0.5f && hitInfo.distance > 0;
        }

        return false;
    }
}
