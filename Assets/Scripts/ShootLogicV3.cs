﻿using UnityEngine;
using System.Collections;

public class ShootLogicV3 : MonoBehaviour {

    public float maxDrag = 2.0f;
    public float lineWidth = 0.30f;
    public int acceleration = 2;

    private float maxDragSqr;

    private bool isClicking;
    private bool drawing;
    private bool isShot;

    private Vector2 prevVelocity;
    private Vector2 dragToMouse;

    private Vector3 ballStartPosition;

    private LineRenderer lineRenderer;

    private Ray rayToMouse;
    private Ray leftStartPointToBall;

    //
    void Awake()
    {
        ballStartPosition = transform.position; // Set the ball position
    }

    // Use this for initialization
    void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1, ballStartPosition);
        lineRenderer.SetWidth(lineWidth, lineWidth);

        rayToMouse = new Ray(ballStartPosition, Vector3.zero);
        leftStartPointToBall = new Ray(lineRenderer.transform.position, Vector3.zero);
        maxDragSqr = maxDrag * maxDrag;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (isClicking && !isShot)
        {
            Dragging();
        }
    }

    //
    void OnMouseDown()
    {
        drawing = true;
        isClicking = true;
    }

    //
    void OnMouseUp()
    {
        drawing = false;
        isClicking = false;
        GetComponent<Rigidbody2D>().isKinematic = false;

        if(!isShot)
        {
            Shoot(); // Shoot the ball if its not shot yet.
        }
    }

    //
    void Shoot()
    {
        Destroy(lineRenderer); // First remove the line.

        Vector3 mousePos = Input.mousePosition;
        Vector3 ballPosition = Camera.main.WorldToScreenPoint(ballStartPosition);
        Vector3 dir = (ballPosition - mousePos).normalized;
        
        float velocity = Vector3.Distance(ballPosition, mousePos) * 5f;

        // When its maximized for shooting, set the max velocity
        if (dragToMouse.sqrMagnitude > maxDragSqr)
        {
            velocity = 500f;
        }

        GetComponent<Rigidbody2D>().AddForce(dir * (velocity * acceleration)); // Shoot it using the velocity * acceleration;

        isShot = true;
    }

    //
    void Dragging()
    {
        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragToMouse = mouseWorldPoint - ballStartPosition;

        // Invert Line
        mouseWorldPoint.x = ballStartPosition.x - dragToMouse.x;
        mouseWorldPoint.y = ballStartPosition.y - dragToMouse.y;

        // Dont extend the line to space, Hold it at max drag.
        if (dragToMouse.sqrMagnitude > maxDragSqr)
        {
            // Invert line at max range.
            dragToMouse.x = mouseWorldPoint.x - ballStartPosition.x;
            dragToMouse.y = mouseWorldPoint.y - ballStartPosition.y;
            rayToMouse.direction = dragToMouse;
            mouseWorldPoint = rayToMouse.GetPoint(maxDrag);
        }
        
        mouseWorldPoint.z = 0f; // Set Z to 0 -> we dont need this in a 2D game.
        lineRenderer.SetPosition(1, mouseWorldPoint);
    }
}