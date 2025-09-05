using UnityEngine;
using UnityEngine.Rendering;

public class MagicMeteorScript : MonoBehaviour
{
    public float lifetime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Collision entered");
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Destroy(gameObject,lifetime);   
    }
}
