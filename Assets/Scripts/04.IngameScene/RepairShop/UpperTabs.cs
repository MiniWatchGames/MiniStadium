using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperTabs : MonoBehaviour
{
    [SerializeField] private GameObject WeaponTab;
    [SerializeField] private GameObject StatusTab;
    [SerializeField] private GameObject SkillTab;
    [SerializeField] private GameObject SkillLowerTabs;
    [SerializeField] private List<GameObject> SkillLowerTabs2 = new List<GameObject>();
    public void OnWeaponTabClick()
    {
        WeaponTab.SetActive(true);
        StatusTab.SetActive(false);
        SkillTab.SetActive(false);
    }
    public void OnStatusTabClick()
    {
        WeaponTab.SetActive(false);
        StatusTab.SetActive(true);
        SkillTab.SetActive(false);
    }
    public void OnSkillTabClick()
    {
        WeaponTab.SetActive(false);
        StatusTab.SetActive(false);
        SkillTab.SetActive(true);
    }
    public void OnSkillLowerTabClick(int index)
    {
        for (int i = 0; i < SkillLowerTabs2.Count; i++)
        {
            if (i == index)
            {
                SkillLowerTabs2[i].SetActive(true);
            }
            else
            {
                SkillLowerTabs2[i].SetActive(false);
            }
        }
    }
}
