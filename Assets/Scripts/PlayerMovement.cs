﻿using System.Collections;
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

    public int health = 100;

    public delegate void OnEnemyDelegate(GameObject enemy);
    public delegate void OnGameOverDelegate();

    public OnEnemyDelegate OnEnemyEntered;
    public OnEnemyDelegate OnEnemyExited;

    public OnGameOverDelegate OnGameOver;

    // Update is called once per frame
    bool wasHit = false;
    public bool playerAlive { get; set; }

    public UnityEngine.UI.Slider healthSlider;

    void Start()
    {
        lastY = GetComponent<Rigidbody>().position.y;
        lastZ = GetComponent<Rigidbody>().position.z;

        healthSlider.value = health;
        playerAlive = true;

        StartCoroutine(DropHealth());
    }

    private void Attack()
    {
        // spawn collider in front of player
        GameObject attackRange = Instantiate(Resources.Load("Prefabs/AttackRange") as GameObject, this.transform.position + new Vector3(0.4f * (faceDirectionX/Mathf.Abs(faceDirectionX)), -0.514f, 0), Quaternion.identity);
        GameObject.Destroy(attackRange, 0.2f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            OnEnemyEntered.Invoke(collider.gameObject);
        }
        else if (collider.gameObject.layer == LayerMask.NameToLayer("AttackEnemy"))
        {
            //if (wasHit)
              //  return;

            float x = collider.transform.position.x - transform.position.x;

            wasHit = true;

            health -= 10;

            

            GameObject blood = Instantiate(Resources.Load("Prefabs/Blood") as GameObject, collider.ClosestPointOnBounds(this.transform.position), Quaternion.identity);

            var velOverLifetime = blood.GetComponent<ParticleSystem>().velocityOverLifetime;

            float forceMultiplier = 1f;

            if (health <= 0)
            {
                var animator = GetComponent<Animator>();
                animator.SetBool("Hit", true);

                playerAlive = false;

                moveX = moveY = 0f;

                forceMultiplier = 2f;
                
                OnGameOver.Invoke();
            }

            if (x < 0)
            {
                GetComponent<Rigidbody>().AddForce(new Vector3(1, 1, 0) * forceMultiplier, ForceMode.Impulse);
                velOverLifetime.x = 2f;
            }
            else
            {
                GetComponent<Rigidbody>().AddForce(new Vector3(-1, 1, 0) * forceMultiplier, ForceMode.Impulse);
                velOverLifetime.x = -2f;
            }

        }

    }

    private IEnumerator DropHealth()
    {
        float dropSpeed = 10f;

        while (true)
        {

            healthSlider.value = Mathf.Lerp(healthSlider.value, health, Time.deltaTime * dropSpeed);

            yield return null;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (!playerAlive)
            {
                var animator = GetComponent<Animator>();
                animator.SetBool("Dead", true);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            OnEnemyExited.Invoke(collider.gameObject);
        }

    }

    void Update()
    {
        

        if (!playerAlive)
            return;

        Ray ray = new Ray(transform.position, -Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction * 10);

        var collider2d = GetComponent<BoxCollider>();
        var rigidbody2D= GetComponent<Rigidbody>();
        var animator = GetComponent<Animator>();

        float lastMoveDirection = faceDirectionX;

        bool isGrounded = IsGrounded();

        if (isGrounded && freshlySpawned)
            freshlySpawned = false;

        if (Input.GetKeyDown(KeyCode.Space))
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
            /*if (!jumped && Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody2D.drag = 0f;
                animator.SetTrigger("Jumped");
                rigidbody2D.AddForce(Vector3.up * 6, ForceMode.Impulse);
                jumped = true;

            }
            else*/
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
        if (!playerAlive)
            return;

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

        if (directionY >= 0 && Physics.Raycast(ray, out hitInfo, 0.5f, 1 << LayerMask.NameToLayer("Wall")) )
        {
            return hitInfo.distance < 0.5f && hitInfo.distance > 0;
        }

        ray = new Ray(transform.position, transform.position - Vector3.forward); // Foreground
        if (directionY <= 0 && Physics.Raycast(ray, out hitInfo, 0.5f, 1 << LayerMask.NameToLayer("Wall")))
        {
            return hitInfo.distance < 0.5f && hitInfo.distance > 0;
        }

        return false;
    }
}
