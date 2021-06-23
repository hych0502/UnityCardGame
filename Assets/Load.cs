using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    [SerializeField]
    public Image progressBar;
    public string Scenename;



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation oper = SceneManager.LoadSceneAsync(Scenename);
        oper.allowSceneActivation = false;
        float timer = 0.0f;
        while (!oper.isDone)
        {
            yield return null;
            timer += Time.deltaTime;
            if(oper.progress >= 0.9f)
            {
                progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
                if(progressBar.fillAmount == 1.0f)
                {
                    oper.allowSceneActivation = true;
            
                }
                else
                {
                    progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, oper.progress, timer);
                    if(progressBar.fillAmount >= oper.progress)
                    {
                        timer = 0f;
                    }
                }
            }
        }
    }
}
