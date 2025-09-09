using UnityEngine;

public class TileRepeater : MonoBehaviour
{
    public float speed = 5f;
    public float resetPositionX = -20f; // sol kenar
    public float startPositionX = 20f;  // yeniden başlama noktası

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < resetPositionX)
        {
            Vector3 newPos = new Vector3(startPositionX, transform.position.y, transform.position.z);
            transform.position = newPos;
        }
    }
}