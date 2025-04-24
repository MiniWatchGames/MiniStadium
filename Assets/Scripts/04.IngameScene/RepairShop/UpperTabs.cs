using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class UpperTabs : MonoBehaviour
{
    
    public delegate Button TabDelegate(Button button);
    public TabDelegate tabButtonDelegate;
    [SerializeField] private RepairShop repairShop;
    [SerializeField] private GameObject WeaponTab;
    [SerializeField] private GameObject StatusTab;
    [SerializeField] private GameObject SkillTab;
    [SerializeField] private GameObject SkillLowerTabs;
    [SerializeField] private GameObject SkillUpperTabs;
    [SerializeField] private List<GameObject> SkillLowerTabs2 = new List<GameObject>();
    private List<Button> skillTabs;
    private List<Button> upperTabs;
    public int currentTab;
    Color selectedColor = new Color(0.623f, 0.623f, 0.623f);
    Color defaultColor = new Color(1f, 1f, 1f);
    void Start()
    {
        skillTabs = new List<Button>();
        upperTabs = new List<Button>();
        for(int i = 0; i < SkillLowerTabs.transform.childCount; i++)
        {
            skillTabs.Add(SkillLowerTabs.transform.GetChild(i).GetComponent<Button>());
        }
        for(int i = 0; i < SkillUpperTabs.transform.childCount; i++)
        {
            upperTabs.Add(SkillUpperTabs.transform.GetChild(i).GetComponent<Button>());
        }

        SetBeginTab();
    }

    void SetBeginTab()
    {
        SetSeletectColor(upperTabs[0], upperTabs);
        WeaponTab.SetActive(true);
        StatusTab.SetActive(false);
        SkillTab.SetActive(false);
        OnSkillLowerTabClick(0);
        currentTab = 0;
    }
    void SetSeletectColor(Button button, List<Button> Tabs)
    {
        for (int i = 0; i < Tabs.Count; i++)
        {
            
            if(Tabs[i] == button)
            {
                Tabs[i].GetComponent<Image>().color = Color.Lerp(selectedColor,defaultColor, 1f * Time.deltaTime);
            }
            else
            {
                Tabs[i].GetComponent<Image>().color = Color.Lerp(defaultColor,selectedColor, 1f * Time.deltaTime);
            }
        }
    }
    public void OnWeaponTabClick(Button button)
    {
        if (currentTab == 0)
            return;
        SetSeletectColor(button, upperTabs);
        WeaponTab.SetActive(true);
        StatusTab.SetActive(false);
        SkillTab.SetActive(false);
        
        currentTab = 0;
    }
    public void OnStatusTabClick(Button button)
    {
        if (currentTab == 1)
            return;
        SetSeletectColor(button, upperTabs);
        WeaponTab.SetActive(false);
        StatusTab.SetActive(true);
        SkillTab.SetActive(false);
        
        currentTab = 1;
    }
    public void OnSkillTabClick(Button button)
    {
        if (currentTab == 2)
            return;
        SetSeletectColor(button, upperTabs);
        WeaponTab.SetActive(false);
        StatusTab.SetActive(false);
        SkillTab.SetActive(true);
        
        currentTab = 2;
    }
    
    public void OnSkillLowerTabClick(int index)
    {
        //SetSeletectColor(skillTabs[index], skillTabs);
        for (int i = 0; i < SkillLowerTabs2.Count; i++)
        {
            if (i == index)
            {
                SkillLowerTabs2[i].SetActive(true);
                skillTabs[i].GetComponent<Image>().color = selectedColor;
            }
            else
            {
                SkillLowerTabs2[i].SetActive(false);
                skillTabs[i].GetComponent<Image>().color = defaultColor;
            }
        }
    }
    
}
