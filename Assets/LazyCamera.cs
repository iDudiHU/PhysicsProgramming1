using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazyCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    public float speed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;    
    }


    void LateUpdate()
    {
        // If there is a target, move the camera towards it
        if (target != null)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), speed);
            transform.position = newPosition;
        }
    }
}
