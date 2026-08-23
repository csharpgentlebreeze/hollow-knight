using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Player;
using UnityEngine;

public class BossFight : MonoBehaviour
{
    public GameObject BossAreaLeft;
    public GameObject BossAreaRight;
    public CinemachineVirtualCamera virtualCamera;
    private Transform player;
    private BossPanel bossPanel;
    private bool isFollow;//是否跟随玩家

    void Awake()
    {
        EventManager.Instance.AddEventListener("GruzWakeUp", WakeUp);
        EventManager.Instance.AddEventListener("GruzAllDead", Dead);
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isFollow)
        {
            virtualCamera.transform.DOMove(new Vector3(player.position.x,player.position.y, -10), 3f).onComplete += () =>
            {
                virtualCamera.Follow = player;
                isFollow = false;
            };
        }
    }

    public void WakeUp()
    {
        BossAreaLeft.SetActive(true);
        BossAreaRight.SetActive(true);
        UIManager.Instance.ShowPanel<BasePanel>("BossPanel", E_UI_Layer.Mid, (panel) =>
        {
            bossPanel = panel as BossPanel;
            bossPanel.SetBossName("格鲁兹之母");
            Invoke(nameof(ClosePanel),7f);
        });
        AudioManager.Instance.PlayBackgroundMusic("Enemy/GruzMother/S18 Enemy Battle-02 LOOP");
    }

    private void ClosePanel()
    {
        StartCoroutine(bossPanel.WaitAndClose("BossPanel", () =>
        {
            
        }));
    }

    public void Dead()
    {
        print(1);
        virtualCamera.transform.DOMove(new Vector3(player.position.x,player.position.y, -10), 3f).onComplete += () =>
        {
            virtualCamera.Follow = player;
        };
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;
            virtualCamera.Follow = null;
            virtualCamera.transform.DOMove(new Vector3(transform.position.x,transform.position.y,-10), 3f);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.transform;
            isFollow = true;
        }
    }

    public void OnDisable()
    {
        EventManager.Instance.RemoveEventListener("GruzWakeUp", WakeUp);
        EventManager.Instance.RemoveEventListener("GruzAllDead", Dead);
    }
}
