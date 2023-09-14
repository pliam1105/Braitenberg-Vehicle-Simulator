using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SourceController : MonoBehaviour
{
    public int source_type;
    public float source_intensity;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.transform.root.gameObject.tag);
        if (collision.transform.root.gameObject.tag == "vehicle")
        {
            Destroy(collision.transform.root.gameObject);
        }
    }
}