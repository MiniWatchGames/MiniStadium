using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamagedUxController : MonoBehaviour
{
    [SerializeField] private Image[] damagedImages;
    private Coroutine upCoroutine;
    private Coroutine downCoroutine;
    private Coroutine rightCoroutine;
    private Coroutine leftCoroutine;
    //0 up
    //1 down
    //2 right
    //3 left

    public void ShowDamagedUx(float hitDegree)
    {
        if ((hitDegree > 150 && hitDegree <= 180) || (hitDegree < -150 && hitDegree >= -180))
        {
            setCoroutine(ref upCoroutine, 0);
        }
        else if ((hitDegree > 0 && hitDegree <= 30) || (hitDegree < 0 && hitDegree >= -30))
        {
            setCoroutine(ref downCoroutine, 1);
        }
        else if (hitDegree < -60 && hitDegree >= -120)
        {
            setCoroutine(ref rightCoroutine, 2);
        }
        else if (hitDegree > 60 && hitDegree <= 120)
        {
            setCoroutine(ref leftCoroutine, 3);
        }
        else if (hitDegree > 30 && hitDegree <= 60)
        {
            setCoroutine(ref downCoroutine, 1);
            setCoroutine(ref leftCoroutine, 3);
        }
        else if (hitDegree > 120 && hitDegree <= 150)
        {
            setCoroutine(ref upCoroutine, 0);
            setCoroutine(ref leftCoroutine, 3);
        }
        else if (hitDegree < -120 && hitDegree >= -150)
        {
            setCoroutine(ref upCoroutine, 0);
            setCoroutine(ref leftCoroutine, 2);
        }
        else if (hitDegree < -30 && hitDegree >= -60)
        {
            setCoroutine(ref downCoroutine, 1);
            setCoroutine(ref rightCoroutine, 2);
        }
        else
        {
            return;
        }
    }

    private void setCoroutine(ref Coroutine routine, int num) {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(DamagedImageCoroutine(num));
    }


    IEnumerator DamagedImageCoroutine(int index)
    {
        float time = 1.5f;
        float startAlpha = 130f / 255f;
        Color originalColor = damagedImages[index].color;
        damagedImages[index].gameObject.SetActive(true);
        while (time > 0)
        {
            time -= Time.deltaTime;
            float alpha = startAlpha * (time / 1.5f); // 시간에 비례해 줄어드는 비율
            damagedImages[index].color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        damagedImages[index].gameObject.SetActive(false);
        damagedImages[index].color = originalColor;
    }
}
