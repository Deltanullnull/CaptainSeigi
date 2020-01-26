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

    public delegate void OnDestroyedDelegate(GameObject enemy);
    public OnDestroyedDelegate OnKilled;

    void Start()
    {
        playerCharacter = GameObject.FindGameObjectWithTag("Player");

        if (playerCharacter == null)
            Debug.Log("Player not found");
    }

    private float cooldown = 1f;
    private float attackAnimationDuration = 0.3f;

    // Update is called once per frame
    void Update()
    {
        var collider2d = GetComponent<BoxCollider>();
        var rigidbody2D = GetComponent<Rigidbody>();
        var animator = GetComponent<Animator>();

        if (wasHit)
            return;

        if (!playerCharacter.GetComponent<PlayerMovement>().playerAlive)
        {
            moveX = moveY = 0;
            animator.SetBool("IsRunning", false);
            return;
        }
        

        if (closeToPlayer && cooldown >= 1f)
        {
            animator.SetTrigger("Punched");
            Attack();

            cooldown = 0f;

            return;
        }

        if (cooldown < 1f)
        {
            cooldown += Time.deltaTime;
        }

        

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

        if (cooldown < attackAnimationDuration)
        {
            moveX = 0;
            moveY = 0;
        }

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

    private void Attack()
    {
        // spawn collider in front of player
        GameObject attackRange = Instantiate(Resources.Load("Prefabs/AttackRangeEnemy") as GameObject, this.transform.position + new Vector3(0.4f * (faceDirectionX / Mathf.Abs(faceDirectionX)), -0.514f, 0), Quaternion.identity);
        GameObject.Destroy(attackRange, 0.2f);
    }

    void FixedUpdate()
    {

        var collider2d = GetComponent<BoxCollider>();
        var rigidBody = GetComponent<Rigidbody>();


        bool isGrounded = IsGrounded();



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
    public bool closeToPlayer { get; set;}

    void OnTriggerEnter(Collider collider)
    {
        if (wasHit)
            return;

        if (collider.gameObject.layer == LayerMask.NameToLayer("Attack"))
        {
            float x = collider.transform.position.x - transform.position.x;

            wasHit = true;

            var animator = GetComponent<Animator>();
            animator.SetBool("Hit", true);

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

            Destroy(this.gameObject, 2f);
        }

    }

    
    void OnDestroy()
    {
        OnKilled.Invoke(this.gameObject);
    }

    public void Land()
    {
        var animator = GetComponent<Animator>();

        animator.SetBool("IsRunning", false);
    }

    void OnCollisionExit(Collision c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (wasHit)
            {
                var animator = GetComponent<Animator>();
                animator.SetBool("Dead", true);
            }
        }
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
