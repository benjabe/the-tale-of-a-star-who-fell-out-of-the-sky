using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] Sprite[] intro;
    [SerializeField] Sprite[] end;
    [SerializeField] Sprite death;
    Image image = null;

    int introIndex = 0;
    int endIndex = 0;

    bool playingIntro = false;
    bool playingEnd = false;
    bool playingDeath = false;

    float inputDelay = -0.0f;

    private void Start()
    {
        image = GetComponentInChildren<Image>();
        playingIntro = true;
        image.sprite = intro[introIndex++];
    }

    private void Update()
    {
        inputDelay -= Time.deltaTime;
        if (inputDelay <= 0.0f && Input.GetMouseButtonUp(0))
        {
            if (playingIntro)
            {
                image.gameObject.SetActive(true);
                image.sprite = intro[introIndex++];
                if (introIndex >= intro.Length)
                {
                    playingIntro = false;
                }
            }
            else if (playingEnd)
            {
                image.gameObject.SetActive(true);
                image.sprite = end[endIndex++];
                if (endIndex >= end.Length)
                {
                    playingEnd = false;
                }
            }
            else if (playingDeath || endIndex >= end.Length)
            {
                Application.Quit();
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    public void End()
    {
        image.gameObject.SetActive(true);
        playingEnd = true;
        image.sprite = end[endIndex++];
    }

    public void Death()
    {
        image.gameObject.SetActive(true);
        image.sprite = death;
        inputDelay = 2.0f;
        playingDeath = true;
    }
}
