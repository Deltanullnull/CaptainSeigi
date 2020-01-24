using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 1f;

    bool isMoving;

    private float z = 0;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal"))
            isMoving = true;
        else if (Input.GetButtonUp("Horizontal"))
            isMoving = false;



        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Debug.Log(x);

        if (!isMoving)
            GetComponent<Animator>().SetBool("IsRunning", false);
        else
        {
            //GetComponent<Animator>().SetFloat("Blend", x > 0 ? 1 : 0);
            GetComponent<Animator>().SetBool("IsRunning", true);

            var position = this.transform.position;

            var horizontalMovement = x > 0 ? Mathf.Ceil(x) : Mathf.Floor(x);
            var verticalMovement = y > 0 ? Mathf.Ceil(y) : Mathf.Floor(y);

            GetComponent<Rigidbody2D>().MovePosition(position + new Vector3(horizontalMovement, verticalMovement)  * Speed * Time.deltaTime);
        }
        

    }
}
