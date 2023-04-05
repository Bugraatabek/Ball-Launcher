using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallHandler : MonoBehaviour
{
    
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot;
    [SerializeField] float respawnDelay = 2f;
    [SerializeField] float detachDelay = 0.5f;
    
    Rigidbody2D currentBallRigidBody;
    private SpringJoint2D currentBallSpringJoint;
    private bool isDragging = false;
    private Camera mainCamera;
    
    List<Rigidbody2D> constructs = new List<Rigidbody2D>();
    bool constructsDisabled = false;

    private void Start() 
    {
        BuildConstructs();
        SpawnNewBall();
        mainCamera = Camera.main;
    }

    private void BuildConstructs()
    {
        foreach (GameObject construct in GameObject.FindGameObjectsWithTag("Construct"))
        {
            constructs.Add(construct.GetComponent<Rigidbody2D>());
        }
    }

    private void DisableConstructs()
    {
        constructsDisabled = true;
        foreach (var construct in constructs)
        {
            construct.isKinematic = true;
        }
    }

    private void EnableConstructs()
    {
        constructsDisabled = false;
        foreach (var construct in constructs)
        {
            construct.isKinematic = false;
        }
    }

    private void Update() 
    {
        if(currentBallRigidBody == null) return;
        if(Touchscreen.current.primaryTouch.press.IsPressed()) 
        {
            if(!constructsDisabled)
            {
                DisableConstructs();
            }

            currentBallRigidBody.isKinematic = true;
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Vector3 touchWorldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
            currentBallRigidBody.position = touchWorldPosition;
            isDragging = true;
        }
        else
        {
            if(isDragging)
            {
                LaunchBall();
            }
            isDragging = false;
        }
    }

    private void LaunchBall()
    {
        if(constructsDisabled)
        {
            EnableConstructs();
        }
        
        currentBallRigidBody.isKinematic = false;
        currentBallRigidBody = null;

        Invoke("DetachBall", detachDelay);
        
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;
        Invoke(nameof(SpawnNewBall), respawnDelay);
    }

    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);
        currentBallRigidBody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();
        currentBallSpringJoint.connectedBody = pivot;
    }

    
    
}



