using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool isHiddenRoom;

    private SpriteRenderer[] sprites;

    private void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (isHiddenRoom)
            {
                AudioManager.Instance.PlaySound("Event/secret_discovered_temp",false);
                isHiddenRoom = false;
            }

            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.DOFade(0, 0.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.DOFade(1, 0.5f);
            }
        }
    }
}
