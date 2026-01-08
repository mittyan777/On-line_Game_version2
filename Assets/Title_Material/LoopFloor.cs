using UnityEngine;

public class LoopFloor : MonoBehaviour
{
    public float speed = 5f;
    public float resetPosition = -20f;
    public float startPosition = 20f;

    void Update()
    {
        transform.Translate(Vector3.left * -speed * Time.deltaTime);

        if (transform.position.z < resetPosition)
        {
            Vector3 pos = transform.position;
            pos.z = startPosition;
            transform.position = pos;
        }
    }
}
