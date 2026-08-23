using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private bool isPlayerDead = false;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventListener("PlayerDead",PlayerDead);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerFSM player = collision.GetComponent<PlayerFSM>();
        if (player != null)
        {
            player.TakeDamage(1,transform);
            if (isPlayerDead == false)
            {
                FindAnyObjectByType<ScreenMask>()._maskImage.DOFade(1, 0.5f).OnComplete(() =>
                {
                    player.transform.position = player.lastGroundedPosition;
                    player.transform.localScale = player.lastGroundedScale;
                    player.anim.Play("HardLand");
                    FindAnyObjectByType<ScreenMask>()._maskImage.DOFade(0, 0.5f).SetDelay(0.5f);
                });
            }
        }
    }

    private void PlayerDead()
    {
        isPlayerDead = true;
    }
}
