using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/**
 * ! ERROR Fixed: Need to put line 6: using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
 * ? Should I improve the background for the game and add more interesting asset?
 * * Implemented the multitouch using the unity touch 
 * adding in the OnEnabled and OnDisabled as well as getting the touchPositions and divide it to determine where the mid point is. 
 * TODO need to create the zoom with the Cinemachine for dynamic zoom. 
*/

public class Ball_Handler : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay;
    [SerializeField] private float respawnDelay;

    private Rigidbody2D currentBallRigidbody;
    private SpringJoint2D currentBallSpringJoint;

    private Camera mainCamera;
    private bool isDragging;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    void OnEnable(){

        EnhancedTouchSupport.Enable();

    }

    void OnDisable(){

        EnhancedTouchSupport.Disable();

    }

    // Update is called once per frame
    void Update()
    {
        if(!Touchscreen.current.primaryTouch.press.isPressed)
        if(currentBallRigidbody == null) {return;}

        if(Touch.activeTouches.Count == 0)
        {
            currentBallRigidbody.isKinematic = false;
            if(isDragging){

                LaunchBall();

            }
            isDragging = false;
            return;
        }

        isDragging = true;
        currentBallRigidbody.isKinematic = true;

        Vector2 touchPositions = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPositions += touch.screenPosition;
        }

        touchPositions /= Touch.activeTouches.Count;

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPositions);

        currentBallRigidbody.position = worldPosition;
    }
    private void SpawnNewBall()
    {
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSpringJoint = ballInstance.GetComponent<SpringJoint2D>();

        currentBallSpringJoint.connectedBody = pivot;
    }

    private void LaunchBall()
    {
        currentBallRigidbody.isKinematic = false;
        currentBallRigidbody = null;

        Invoke(nameof(DetachBall), detachDelay);
    }

    private void DetachBall()
    {
        currentBallSpringJoint.enabled = false;
        currentBallSpringJoint = null;

        Invoke(nameof(SpawnNewBall), respawnDelay);
    }

}
