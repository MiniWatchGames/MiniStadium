using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillGage : MonoBehaviour, IStatObserver
{
    private Slider _skillGageSlider;
    private List<IStatPublisher> _statPublisher;
    private void Awake()
    {
        _skillGageSlider = GetComponent<Slider>();
        _statPublisher = new List<IStatPublisher>();
    }

    public void Observing(IStatPublisher publisher) {
        _statPublisher.Add(publisher);
        _statPublisher[^1].AddObserver(this);
    }

    public void ResetSkillGage()
    {
        foreach (var statPublisher in _statPublisher)
        {
            statPublisher.RemoveObserver(this);
        }
        _statPublisher.Clear();
    }

    public void WhenStatChanged((float, string) data)
    {
        switch (data.Item2) {
            case "_needPressTime":
                _skillGageSlider.maxValue = data.Item1;
                break;
            case "_pressTime":
                _skillGageSlider.value = data.Item1;
                break;
        }
    }
}
