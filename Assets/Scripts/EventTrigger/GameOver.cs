using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.ClosePanel("KnightPanel");
            GameManager.Instance.gameState = GameState.UI;
            UIManager.Instance.ClearPanel();
            FindAnyObjectByType<ScreenMask>()._maskImage.DOFade(1,1f).OnComplete(() =>
            {
                SceneManager.LoadScene(0);
            });
        }
    }
}
