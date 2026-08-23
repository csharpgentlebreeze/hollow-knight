using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBlood : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D capsule;
    private Transform child;
    private Animator anim;
    private RaycastHit2D[] hits;
    private Vector2 velocity;
    private Vector2 aver_normal;
    private bool isHit = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        child = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        /*hits = Physics2D.BoxCastAll(capsule.bounds.center, capsule.size, 0, velocity, 0.2f,
            LayerMask.GetMask("Terrain"));
        if (hits.Length > 0)
        {
            float x = 0;
            float y = 0;
            foreach (RaycastHit2D hit in hits)
            {
                x += hit.normal.x;
                y += hit.normal.y;
            }
            aver_normal = new Vector2(x / hits.Length, y / hits.Length);
            float angle = Vector2.SignedAngle(Vector2.up, aver_normal);
            child.rotation = Quaternion.Euler(0, 0, angle);
            isHit = true;
            anim.Play("blood");
            rb.bodyType = RigidbodyType2D.Static;
        }*/
    }

    private void FixedUpdate()
    {
        if (isHit == false)
        {
            velocity = rb.velocity;
        }
        
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity, Mathf.Infinity, LayerMask.GetMask("Terrain"));
            aver_normal = hit.normal;
            float angle = Vector2.SignedAngle(Vector2.up, aver_normal);
            child.rotation = Quaternion.Euler(0, 0, angle);
            
            isHit = true;
            anim.Play("blood");
            float angel = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    private void Destroy()
    {
        PoolManager.Instance.Push("Prefabs/Enemy/Blood", gameObject);
    }
}
