using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeThreshold = 50f; // Ajusta este valor según la sensibilidad deseada

    private PlayerManager playerManager;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    void Update()
    {
        DetectSwipe();
        DetectKeyboardInput();
    }

    private void DetectSwipe()
    {
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                Vector2 swipeDelta = endTouchPosition - startTouchPosition;

                if (swipeDelta.magnitude > swipeThreshold)
                {
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        if (swipeDelta.x > 0)
                        {
                            playerManager.MoveRight();
                        }
                        else
                        {
                            playerManager.MoveLeft();
                        }
                    }
                    else
                    {
                        if (swipeDelta.y > 0)
                        {
                            playerManager.Jump();
                        }
                        else
                        {
                            playerManager.Roll();
                        }
                    }
                }
            }
        }
    }

    private void DetectKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerManager.MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            playerManager.MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            playerManager.Jump();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerManager.Roll();
        }
    }
}
