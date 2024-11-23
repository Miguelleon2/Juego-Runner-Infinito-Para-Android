using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private bool swipeDetected;

    public InputAction touchAction;

    private void OnEnable()
    {
        touchAction.Enable();
        touchAction.started += OnTouchStarted;
        touchAction.canceled += OnTouchEnded;
    }

    private void OnDisable()
    {
        touchAction.started -= OnTouchStarted;
        touchAction.canceled -= OnTouchEnded;
        touchAction.Disable();
    }

    private void OnTouchStarted(InputAction.CallbackContext context)
    {
        startTouchPosition = context.ReadValue<Vector2>();
        swipeDetected = false;
    }

    private void OnTouchEnded(InputAction.CallbackContext context)
    {
        endTouchPosition = context.ReadValue<Vector2>();
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        Vector2 swipe = endTouchPosition - startTouchPosition;
        if (!swipeDetected && swipe.magnitude > 100) // Ajusta el umbral según la sensibilidad deseada
        {
            float yDifference = Mathf.Abs(swipe.y);
            float xDifference = Mathf.Abs(swipe.x);

            if (yDifference > xDifference)
            {
                if (swipe.y > 0)
                {
                    Debug.Log("Swipe Up - Jump");
                    // Llama a tu función de salto aquí
                }
                else
                {
                    Debug.Log("Swipe Down - Roll");
                    // Llama a tu función de roll aquí
                }
            }
            swipeDetected = true;
        }
    }
}
