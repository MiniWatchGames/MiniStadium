using System;
using System.Collections.Generic;

public class Stat : IStatPublisher
{
    private readonly float baseValue;
    public List<Func<float, float>> Modifiers => modifiers;
    private List<Func<float, float>> modifiers;

    private string name;
    private float value;
    private List<IStatObserver> observers;
    private (float,string) data;
    private float passResult;
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
            return result;
        }
    }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="baseValue"> PlayerController의 fixed변수들의 기본값을 할당 </param>
    /// <param name="statName">비교를 위한 Stat의 이름을 정함</param>
    public Stat(float baseValue, string statName)
    {
        this.baseValue = baseValue;
        this.modifiers = new List<Func<float, float>>();
        this.value = baseValue;
        this.observers = new List<IStatObserver>();
        name = statName;
    }

    /// <summary>
    /// 데코레이트 추가 메소드
    /// </summary>
    /// <param name="Decorate">데코레이트 할 func 내용이 들어간다</param>
    public int AddDecorate(Func<float, float> Decorate)
    {
        modifiers.Add(Decorate);
        checkValueChanged();
        return modifiers.Count - 1;
    }

    /// <summary>
    /// 인덱스 번째 버프 제거 
    /// </summary>
    public void RemoveModifiers() {
        if (modifiers.Count > 0)
        {
            modifiers.RemoveAt(modifiers.Count - 1);
            checkValueChanged();
        }
    }
    
    /// <summary>
    /// 특정 순서의 버프 제거 
    /// </summary>
    public void RemoveTargetModifier(int targetIndex) {
        if (modifiers.Count > 0 && modifiers.Count-1 <= targetIndex)
        {
            modifiers.RemoveAt(targetIndex);
            checkValueChanged();
        }
    }

    /// <summary>
    /// 버프 모두 제거
    /// </summary>
    public void RemoveAllModifiers()
    {
        modifiers.Clear();
        checkValueChanged();
    }

    public void AddObserver(IStatObserver observer)
    {
        if (observers == null) {
            observers = new List<IStatObserver>();
        }
        if(observers.Contains(observer))return;
        observers.Add(observer);
    }

    public void RemoveAllObservers()
    {
        observers.Clear();
    }
    public void RemoveObserver(IStatObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers((float,string) value)
    {
        if (observers == null) return;
        foreach (IStatObserver observer in observers)
        {
            observer.WhenStatChanged(value);
        }
    }

    private void checkValueChanged() {
        float curr = Value;
        if (passResult != curr)
        {
            NotifyObservers(new(curr, name));
            passResult = curr;
        }
    }
}
