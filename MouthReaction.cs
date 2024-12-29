
using UnityEngine;
using UnityEngine.UI;

public class MouthReaction : MonoBehaviour
{
    private Image image;
    float currentTimer;
    const float timermin = 120;
    const float timermax = 240;
    const float disappeartimermin = 2;
    const float disappeartimermax = 10;

    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
        currentTimer = Random.Range(timermin, timermax);
    }

    void Update()
    {
        if (currentTimer <= 0f)
        {
            image.enabled = !image.enabled;
            if (image.enabled)
            {
                currentTimer = Random.Range(disappeartimermin,disappeartimermax);
            }
            else
            {
                currentTimer = Random.Range(timermin,timermax);
            }
        }
        else
        {
            currentTimer -= Time.deltaTime;
        }
    }
}