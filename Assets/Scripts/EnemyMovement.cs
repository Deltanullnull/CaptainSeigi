using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    private bool jumped = false;
    private bool pressedJump = false;

    private bool isFalling = false;

    private float faceDirectionX = 0;
    private float faceDirectionY = 0;
    private float moveX = 0;
    private float moveY = 0;

    public float runSpeed;

    bool freshlySpawned = true;

    public bool IsHit { get; set; }

    GameObject playerCharacter;

    void Start()
    {
        playerCharacter = GameObject.FindGameObjectWithTag("Player");

        if (playerCharacter == null)
            Debug.Log("Player not found");
    }

    // Update is called once per frame
    void Update()
    {
        if (wasHit)
            return;

        var collider2d = GetComponent<BoxCollider>();
        var rigidbody2D = GetComponent<Rigidbody>();
        var animator = GetComponent<Animator>();

        float lastMoveDirection = faceDirectionX;

        bool isGrounded = IsGrounded();

        if (isGrounded && freshlySpawned)
            freshlySpawned = false;

        Vector3 vectorToPlayer = (playerCharacter.transform.position - transform.position).normalized;

        moveX = vectorToPlayer.x;
        moveY = vectorToPlayer.z;

        if (moveX != 0)
            faceDirectionX = moveX;

        if (moveY != 0)
            faceDirectionY = moveY;

        if (moveY != 0 && isGrounded)
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

        var collider2d = GetComponent<BoxCollider>();
        var rigidBody = GetComponent<Rigidbody>();


        bool isGrounded = IsGrounded();

        if (wasHit && isGrounded && rigidBody.velocity.y < 0)
        {
            Debug.Log("Landed");
            wasHit = false;
        }

        if (wasHit)
            return;

        if (TouchingWall(moveY))
            moveY = 0;

        if (moveX != 0 || moveY != 0)
        {
            rigidBody.velocity = new Vector3(moveX, 0, moveY * 2) * Time.deltaTime * runSpeed + new Vector3(0, rigidBody.velocity.y, 0);
        }
        else
        {
            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
        }
    }

    bool wasHit = false;

    void OnTriggerEnter(Collider collider)
    {
        if (wasHit)
            return;

        if (collider.gameObject.layer == LayerMask.NameToLayer("Attack"))
        {
            Debug.Log("I'm hit");
        }

        float x = collider.transform.position.x - transform.position.x;

        wasHit = true;

        GameObject blood = Instantiate(Resources.Load("Prefabs/Blood") as GameObject, collider.ClosestPointOnBounds(this.transform.position), Quaternion.identity);

        var velOverLifetime = blood.GetComponent<ParticleSystem>().velocityOverLifetime;

        if (x < 0)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(3, 5, 0), ForceMode.Impulse);
            velOverLifetime.x = 2f;
        }
        else
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(-3, 5, 0), ForceMode.Impulse);
            velOverLifetime.x = -2f;
        }
        
        
    }

    public void Land()
    {
        var animator = GetComponent<Animator>();

        animator.SetBool("IsRunning", false);
    }

    private bool IsGrounded()
    {
        if (!freshlySpawned && !jumped && !wasHit)
            return true;

        var collider2d = GetComponent<BoxCollider>();

        RaycastHit hitInfo;
        Ray ray = new Ray(transform.position, -Vector3.up);

        if (Physics.Raycast(ray, out hitInfo, 10f))
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

        if (directionY > 0 && Physics.Raycast(ray, out hitInfo, 0.2f))
        {
            //Debug.Log("Touching wall (far) " + hitInfo.distance);
            return hitInfo.distance < 0.1f && hitInfo.distance > 0;
        }

        ray = new Ray(transform.position, transform.position - Vector3.forward);

        if (directionY < 0 && Physics.Raycast(ray, out hitInfo, 0.2f))
        {
            //Debug.Log("Touching wall (near)" + hitInfo.distance);
            return hitInfo.distance < 0.1f && hitInfo.distance > 0;
        }

        return false;
    }

    
}
