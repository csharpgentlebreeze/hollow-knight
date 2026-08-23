using Player;
using UnityEngine;

public class FirstLand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<PlayerFSM>().rb.gravityScale = 1.5f;
            UIManager.Instance.ShowPanel<BasePanel>("KnightPanel", E_UI_Layer.Bot);
            AudioManager.Instance.PlaySound("Event/S75 Opening Sting-08",false);
            gameObject.SetActive(false);
        }
    }
}
