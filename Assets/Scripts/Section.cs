using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public List<GameObject> obstacles;
    private static int lastRandomIndex = -1;
    private int sectionsCount = 0;
    public float speed;
    public float sectionSize = 20;

    void Start()
    {
        sectionsCount = GameObject.FindGameObjectsWithTag("Section").Length;

        obstacles = new List<GameObject>();

        foreach (Transform child in transform) {
            if (child.tag == "Obstacle"){
                obstacles.Add(child.gameObject);
            }
        }

        EnableRandomObstacles();
    }

    public void EnableRandomObstacles()
    {
        foreach (GameObject obstacle in obstacles){
            obstacle.SetActive(false);
        }

        int randomIndex = lastRandomIndex;
        while (randomIndex == lastRandomIndex){
            randomIndex = Random.Range(0, obstacles.Count);
        }
        lastRandomIndex = randomIndex;
        obstacles[randomIndex].SetActive(true);
    }

    void Update()
    {
        // Mueve la sección solo si speed es mayor que 0
        if (speed > 0)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);

            if (transform.position.z <= -sectionSize) {
                transform.Translate(Vector3.forward * sectionSize * sectionsCount);
                EnableRandomObstacles();
            }
        }
    }

    // Método para detener el movimiento
    public void StopSection()
    {
        speed = 0; // Detiene el avance del mundo al establecer la velocidad en cero
    }
}
