using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchingScenePanelController : PanelController
{
    public GameObject currentPanel;
    public GameObject nextPanel;
    public float panelDelayTime = 5f;
    public float sceneDelayTime = 5f;

    private void Start()
    {
        StartCoroutine(SwitchPanelAfterDelay());
    }

    private IEnumerator SwitchPanelAfterDelay()
    {
        yield return new WaitForSeconds(panelDelayTime);
        currentPanel.SetActive(false);
        nextPanel.SetActive(true);

        yield return new WaitForSeconds(sceneDelayTime);
        SceneManager.LoadScene("IngameScene");
    }
}
