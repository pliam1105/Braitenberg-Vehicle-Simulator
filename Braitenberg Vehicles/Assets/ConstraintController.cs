using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ConstraintController : MonoBehaviour
{
    public float speed = 2;
    public float radius = 5;
    private Vector3 originalPosition;
    private Vector3 moveDirection;

    private float minX = -1;
    private float maxX = 1;
    //private float minY = -1;
    //private float maxY = 1;
    private float minZ = -1;
    private float maxZ = 1;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        moveDirection = randomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        var newPosition = transform.position + (moveDirection * speed * Time.deltaTime);
        if(Vector3.Distance(newPosition, originalPosition) > radius)
        {
            while (Vector3.Distance(newPosition, originalPosition) > radius)
            {
                moveDirection = randomDirection();
                newPosition = transform.position + (moveDirection * speed * Time.deltaTime);
            }
        }
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
    }

    Vector3 randomDirection()
    {
        Vector3 rand = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));
        rand.Normalize();
        return rand;
    }
}
