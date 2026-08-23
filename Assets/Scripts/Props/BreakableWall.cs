using UnityEngine;

public class BreakableWall : Breakable
{
    public ParticleSystem dustHit1;
    public ParticleSystem dustHit2;
    public ParticleSystem dustHit3;
    public ParticleSystem ptWood1;
    public ParticleSystem ptWood2;
    public ParticleSystem ptWood3;
    public ParticleSystem ptBits1;
    public ParticleSystem ptBits2;
    public ParticleSystem ptBits3;
    
    private Animator animator;
    private AudioController audio;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        CheckIsDead();
    }
    
    public override void Hurt(int damage, Transform attackPosition)
    {
        base.Hurt(damage, attackPosition);
        switch (health)
        {
            case 2:
                audio.PlaySound("BreakableWall/breakable_wall_hit_1",false);
                dustHit1.Play();
                ptWood1.Play();
                ptBits1.Play();
                break;
            case 1:
                audio.PlaySound("BreakableWall/breakable_wall_hit_2",false);
                dustHit2.Play();
                ptWood2.Play();
                ptBits2.Play();
                break;
        }
        Vector2 vector = attackPosition.position - transform.position;
        if (vector.x > 0)
        { 
            //�������Ч
        }
        else
        {
            //����
        }
        animator.SetTrigger("Hurt");
    }
    protected override void Dead()
    {
        base.Dead();
        //��Ч
        dustHit3.Play();
        ptWood3.Play();
        ptBits3.Play();
        animator.SetTrigger("Dead");
        AudioManager.Instance.PlaySound("BreakableWall/breakable_wall_death", false);
        Invoke("Destroy", 0.5f);
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}
