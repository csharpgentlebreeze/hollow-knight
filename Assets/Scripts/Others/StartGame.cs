using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public AsyncOperation operation;
    // Start is called before the first frame update
    void Start()
    {
        /*StartCoroutine(LoadScene());*/
        FindAnyObjectByType<ScreenMask>()._maskImage.DOFade(1,0.01f).OnComplete(() =>
        {
            FindAnyObjectByType<ScreenMask>()._maskImage.DOFade(0, 1.5f);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    /*IEnumerator LoadScene()
    {
        operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            yield return null;
        }
    }*/
}
