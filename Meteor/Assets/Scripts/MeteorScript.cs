using UnityEngine;

public class MeteorScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float minMass = 1f;
    public float maxMass = 4f;


    private void Start()
    {
        Random_Mass_Allocation();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Collision entered");
            Destroy(collision.gameObject);
        }
    }

    private void Random_Mass_Allocation()
    {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        rb.mass = Random.Range(minMass,maxMass);
    }

}
