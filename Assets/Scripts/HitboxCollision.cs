using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HitboxCollision : MonoBehaviour
{
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerAttackController pac = collider.gameObject.transform.GetComponentInParent<PlayerAttackController>();
        if(pac != null){
            Vector2 kb = pac.GetKnockBackVector();
            if(collider.gameObject.transform.position.x > transform.position.x){
                // Debug.Log("reverted");
                kb.x = -kb.x;
            }
            // Debug.Log("kb: " + kb);
            rb.AddForce(kb * 600);
        }

    }
    
}
