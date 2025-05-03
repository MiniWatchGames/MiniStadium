using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillData
{
    public ObservableFloat CoolTime { get; }
    //키 프레스 방식인지
    public bool IsNeedPresse { get; }
    //키 프레스 방식일때 얼마나 눌러야 하는지
    public ObservableFloat NeedPressTime { get; }
    //키 프레스 진행상황
    public ObservableFloat PressTime { get; }
    public float SkillMount { get; }


}
