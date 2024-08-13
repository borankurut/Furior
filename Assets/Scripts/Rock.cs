using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField] float destroyWhenY = -30.0f;
    void Update()
    {
        if(transform.position.y < destroyWhenY)
            Destroy(gameObject);
    }
}
