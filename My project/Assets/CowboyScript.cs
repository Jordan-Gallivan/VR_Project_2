using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CowboyScript : MonoBehaviour
{
    private GameObject tgt;
    private float startTime;
    private float durration = 6f;
    public float xSpeed = 0;
    public float ySpeed = 0;
    public float zSpeed = 0;
    public Vector3 speed;

    public float minDist = 10;
    public float dist;
    private GameObject playerObj;

    public GameObject deathSmoke;
    // Start is called before the first frame update
    void Start()
    {
        this.startTime = Time.time;
        this.tgt = this.gameObject;
        this.speed = new Vector3(xSpeed, ySpeed, zSpeed);
        
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.tgt.transform.Translate(this.speed);
        if (Time.time - startTime > durration)
        {
            xSpeed *= -1;
            ySpeed *= -1;
            zSpeed *= -1;
            startTime = Time.time;
            this.speed = new Vector3(xSpeed, ySpeed, zSpeed);
        }
        distanceCall();
    }

    void distanceCall()
    {
        dist = Vector3.Distance(playerObj.transform.position, transform.position);
        if (dist < minDist)
        {
            print("Distance acheived");
        }
        
    }
}
