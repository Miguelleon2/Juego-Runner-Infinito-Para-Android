using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving;
    private bool isRolling;
    private bool isJumping;
    private Animator animator;
    public float lanesDistance = 1f;
    public float moveSpeed = 0.5f;

    public SkinnedMeshRenderer[] renderableMeshes;
    private Collider playerCollider;

    public TMP_Text countdownText; // Referencia al texto de cuenta regresiva en la UI
    public TMP_Text sprayCounterText; // Referencia al contador de sprays en la UI
    private int sprayCount = 0; // Contador de sprays recogidos
    private int maxSprayCount = 5; // Número máximo de sprays para completar el desafío

    private bool isImmortal = false; // Bandera de inmunidad
    private bool gameOver = false; // Bandera para indicar si el juego está en estado de fin

    private Section[] sections; // Referencia a todas las secciones

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
        isMoving = false;
        isRolling = false;
        isJumping = false;

        playerCollider = GetComponent<Collider>();

        // Buscar y almacenar todas las secciones al inicio
        sections = FindObjectsOfType<Section>();

        // Iniciar la cuenta regresiva de inicio
        StartCoroutine(StartGameCountdown());

        // Actualiza el contador de sprays al inicio
        UpdateSprayCounter();
    }

    public void MoveRight()
    {
        if (IsBusy() || transform.position.x > 0 || gameOver) return;
        targetPosition = transform.position + Vector3.right * lanesDistance;
        isMoving = true;
    }

    public void MoveLeft()
    {
        if (IsBusy() || transform.position.x < 0 || gameOver) return;
        targetPosition = transform.position + Vector3.left * lanesDistance;
        isMoving = true;
    }

    public void Jump()
    {
        if (IsBusy() || gameOver) return;
        animator.SetTrigger("Jump");
        isJumping = true;
    }

    public void Roll()
    {
        if (IsBusy() || gameOver) return;
        animator.SetTrigger("Roll");
        isRolling = true;
        StartCoroutine(EndRollAfterTime(1f)); // Duración de la animación Roll
    }

    private IEnumerator EndRollAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        isRolling = false;
    }

    public void EndJump()
    {
        isJumping = false;
    }

    public void EndRoll()
    {
        isRolling = false;
    }

    private bool IsBusy()
    {
        return isMoving || isRolling || isJumping;
    }

    private void MoveToTargetPosition()
    {
        if (!isMoving || gameOver) return;

        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed);
        float distance = (targetPosition - transform.position).magnitude;

        if (distance < 0.001f)
        {
            transform.position = targetPosition;
            isMoving = false;
        }
    }

    public void CollectSpray()
    {
        sprayCount++;
        UpdateSprayCounter();

        if (sprayCount >= maxSprayCount)
        {
            ShowSuccessMessage();
        }
    }

    private void UpdateSprayCounter()
    {
        sprayCounterText.text = $"{sprayCount}/{maxSprayCount}";
    }

    private void ShowSuccessMessage()
    {
        GameUI.Instance.ShowSuccessPanel();
        Time.timeScale = 0; // Pausa el juego
    }

    public void ShowFailureMessage()
    {
        GameUI.Instance.ShowFailurePanel();
        Time.timeScale = 0; // Pausa el juego
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isImmortal || gameOver) return;

        if (other.CompareTag("SmallObstacle") && !isJumping)
        {
            animator.SetTrigger("Stumble");
            StartCoroutine(BlinkAndDisableCollisions());
        }
        else if (other.CompareTag("BigObstacle"))
        {
            animator.SetTrigger("StumbleDown");
            gameOver = true;
            playerCollider.enabled = false;

            foreach (Section section in sections)
            {
                section.StopSection();
            }

            // Mostrar el mensaje de fallo después de la animación
            StartCoroutine(ShowFailureAfterAnimation(2f)); // Ajusta 2f según la duración de la animación
        }
        else if (other.CompareTag("DownObstacle"))
        {
            if (!isRolling)
            {
                animator.SetTrigger("StumbleDown");
                gameOver = true;
                playerCollider.enabled = false;

                foreach (Section section in sections)
                {
                    section.StopSection();
                }

                StartCoroutine(ShowFailureAfterAnimation(2f)); // Ajusta 2f según la duración de la animación
            }
        }
    }

    private IEnumerator BlinkAndDisableCollisions()
    {
        playerCollider.enabled = false;

        for (int i = 0; i < 6; i++)
        {
            foreach (SkinnedMeshRenderer mesh in renderableMeshes)
            {
                mesh.enabled = false;
            }
            yield return new WaitForSeconds(0.25f);

            foreach (SkinnedMeshRenderer mesh in renderableMeshes)
            {
                mesh.enabled = true;
            }
            yield return new WaitForSeconds(0.25f);
        }

        playerCollider.enabled = true;
    }

    private IEnumerator StartGameCountdown()
    {
        Time.timeScale = 0;
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return StartCoroutine(WaitForRealSeconds(1f));
        }

        countdownText.text = "GO!";
        yield return StartCoroutine(WaitForRealSeconds(1f));

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;

        StartCoroutine(StartImmortalityWithBlink());
    }

    private IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    private IEnumerator StartImmortalityWithBlink()
    {
        isImmortal = true;
        playerCollider.enabled = false;

        for (int i = 0; i < 6; i++)
        {
            foreach (SkinnedMeshRenderer mesh in renderableMeshes)
            {
                mesh.enabled = false;
            }
            yield return new WaitForSeconds(0.25f);

            foreach (SkinnedMeshRenderer mesh in renderableMeshes)
            {
                mesh.enabled = true;
            }
            yield return new WaitForSeconds(0.25f);
        }

        isImmortal = false;
        playerCollider.enabled = true;
    }

    private IEnumerator ShowFailureAfterAnimation(float animationDuration)
    {
        yield return new WaitForSeconds(animationDuration); // Espera el tiempo de la animación
        ShowFailureMessage(); // Muestra el mensaje de fallo
    }

    void Update()
    {
        if (!gameOver)
        {
            MoveToTargetPosition();
        }
    }
}
