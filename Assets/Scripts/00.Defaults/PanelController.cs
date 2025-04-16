using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public GameObject[] panels;

    /// <summary>
    /// 지정한 이름의 패널만 활성화하고, 나머지 패널은 비활성화하는 함수
    /// </summary>
    /// <param name="panelName"></param>
    public void OpenPanel(string panelName)
    {
        foreach (GameObject panel in panels)
        {
            // 이름이 일치하는 패널만 활성화 (나머지는 비활성화)
            panel.SetActive(panel.name == panelName);
        }
    }

    /// <summary>
    /// 모든 패널을 비활성화하는 함수
    /// </summary>
    public void CloseAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            // 전부 비활성화
            panel.SetActive(false);
        }
    }
}
