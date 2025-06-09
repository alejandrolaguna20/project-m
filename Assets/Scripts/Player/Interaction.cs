using UnityEngine;

public class InteractionHitbox : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        playerController.OnHitboxTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        playerController.OnHitboxTriggerExit(other);
    }
}
