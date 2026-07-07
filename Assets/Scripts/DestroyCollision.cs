using UnityEngine;

public class DestroyCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(player.MaxHealth); //Mata al jugador si cae fuera de los límites del juego
        }
        else
        {
            Debug.Log(other.gameObject.name + " ha caído fuera de los límites del juego.");
            Destroy(other.gameObject);
        }
    }
}
