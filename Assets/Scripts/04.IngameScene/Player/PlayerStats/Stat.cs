using System;
using System.Collections.Generic;

public class Stat
{
    private readonly float baseValue;
    private List<Func<float, float>> modifiers;
    private float value;

    public float BaseValue => baseValue;
    public float Value
    {
        get
        {
            float result = baseValue;
            foreach (var modifier in modifiers)
            {
                result = modifier(result);
            }
            value = result;
            return value;
        }
    }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="baseValue"> PlayerController의 fixed변수들의 기본값을 할당 </param>
    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        this.modifiers = new List<Func<float, float>>();
        this.value = baseValue;
    }

    /// <summary>
    /// 데코레이트 추가 메소드
    /// </summary>
    /// <param name="Decorate">데코레이트 할 func 내용이 들어간다</param>
    public void AddDecorate(Func<float, float> Decorate)
    {
        modifiers.Add(Decorate);
    }

    /// <summary>
    /// 인덱스 번째 버프 제거 
    /// </summary>
    public void RemoveModifiers() {
        if (modifiers.Count > 0)
        {
            modifiers.RemoveAt(modifiers.Count - 1);
        }
    }

    /// <summary>
    /// 버프 모두 제거
    /// </summary>
    public void RemoveAllModifiers()
    {
        modifiers.Clear();
    }
}
