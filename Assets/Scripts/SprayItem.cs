using UnityEngine;

public class SprayItem : MonoBehaviour
{
    public float rotationSpeed = 50f; // Velocidad de rotación

    void Update()
    {
        // Hace que el objeto gire constantemente
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Detecta si el jugador recoge el objeto
        {
            PlayerManager player = other.GetComponent<PlayerManager>();
            if (player != null)
            {
                player.CollectSpray();
                Destroy(gameObject); // Destruye el objeto después de ser recogido
            }
        }
    }
}
