using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoStone : Breakable
{
    [SerializeField] GameObject coin;
    [SerializeField] int minSpawnCoins;
    [SerializeField]int maxSpawnCoins;
    [SerializeField]float maxBumpYForce;
    [SerializeField]float minBumpYForce;
    [SerializeField]float maxBumpXForce;

    private Animator animator;
    private AudioController audio;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioController>();
    }

    private void Update()
    {
        CheckIsDead();
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Attack"))
        {
            print(1);
            Hurt(1, FindObjectOfType<Attack>().transform);
        }
    }*/
    
    public override void Hurt(int damage, Transform attackPosition)
    {
        base.Hurt(damage, attackPosition);
        int random = Random.Range(1, 4);
        audio.PlaySound("GeoStone/geo_rock_hit_" + random.ToString(),false);
        Vector2 vector = attackPosition.position - transform.position;
        if (vector.x > 0)
        { 
        //�������Ч
        }
        else
        {
            //����
        }
        SpawnCoins();
        animator.SetTrigger("Hurt");
    }
    protected override void Dead()
    {
        base.Dead();
        //��Ч
        audio.PlaySound("BreakableWall/breakable_wall_death", false);
        animator.SetTrigger("Dead");
    }
    private void SpawnCoins()
    {
        int randomCount = Random.Range(minSpawnCoins, maxSpawnCoins);
        for (int i = 0; i < randomCount; i++)
        {
            GameObject geo = Instantiate(coin, transform.position, Quaternion.identity, transform) as GameObject;
            Vector2 force = new Vector2(Random.Range(-maxBumpXForce, maxBumpXForce), Random.Range(minBumpYForce, maxBumpYForce));
            geo.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        }
    }
}
