using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBehaviour : StateMachineBehaviour
{
    // Este método se llama cuando la animación de Roll termina
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Obtén el componente PlayerManager del objeto que tiene el Animator
        PlayerManager playerManager = animator.GetComponent<PlayerManager>();

        if (playerManager != null)
        {
            playerManager.EndRoll(); // Llama a EndRoll para desactivar el estado de Roll
        }
    }
}
