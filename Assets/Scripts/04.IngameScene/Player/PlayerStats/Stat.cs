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
    /// ������
    /// </summary>
    /// <param name="baseValue"> PlayerController�� fixed�������� �⺻���� �Ҵ� </param>
    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        this.modifiers = new List<Func<float, float>>();
        this.value = baseValue;
    }

    /// <summary>
    /// ���ڷ���Ʈ �߰� �޼ҵ�
    /// </summary>
    /// <param name="Decorate">���ڷ���Ʈ �� func ������ ����</param>
    public void AddDecorate(Func<float, float> Decorate)
    {
        modifiers.Add(Decorate);
    }

    /// <summary>
    /// �ε��� ��° ���� ���� 
    /// </summary>
    public void RemoveModifiers() {
        if (modifiers.Count > 0)
        {
            modifiers.RemoveAt(modifiers.Count - 1);
        }
    }

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    public void RemoveAllModifiers()
    {
        modifiers.Clear();
    }
}
