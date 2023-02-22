using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public float walkingSpeed = .5f;
    public float runningSpeed = 10f;
    public Rigidbody rb;
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        if (Input.GetKey(KeyCode.T))
        {
            print("Forward");
            rb.velocity = transform.forward * walkingSpeed;
        }

        else if (Input.GetKey(KeyCode.G))
        {
            print("Backwards");
            rb.velocity = -transform.forward * walkingSpeed;
        }
        else if (Input.GetKey(KeyCode.F))
        {
            print("Left");
            rb.velocity = -transform.right * walkingSpeed;
            
        }
        else if (Input.GetKey(KeyCode.H))
        {
            print("Right");
            rb.velocity = transform.right * walkingSpeed;
        }
    }
}
