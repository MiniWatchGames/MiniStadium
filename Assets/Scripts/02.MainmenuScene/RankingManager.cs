using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    /*
    [Header("Ranking Scroll")]
    public GameObject rankingInfoPrefab;
    public Transform contentParent;
    */

    [Header("Button")] 
    public Button closeButton;

    // 생성할 프리팹 개수
    // public int rankingCount = 100;

    private void Start()
    {
        // CreateRankingInfo();
        closeButton.onClick.AddListener(CloseRanking);
    }

    /*
    private void CreateRankingInfo()
    {
        for (int i = 1; i <= rankingCount; i++)
        {
            GameObject item = Instantiate(rankingInfoPrefab, contentParent);
            item.name = "[Prefab] RankingInfo" + i;

            // TextMeshProUGUI rankText = item.transform.Find("[Text] Rank").GetComponent<TextMeshProUGUI>();
            // rankText.text = i.ToString();
        }
    }
    */

    private void CloseRanking()
    {
        gameObject.SetActive(false);
    }
}
