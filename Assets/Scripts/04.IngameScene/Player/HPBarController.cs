using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour, IStatObserver

{
    [SerializeField] private DummyController dummy;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Image _hpGauge;
    private Transform cameraTransform;
    float currentHp = 0;
    float maxHp = 0;

    private void Awake()
    {
        dummy.CurrentHp.AddObserver(this);
        maxHp = dummy._maxHp;
    }
    public Transform DetectPlayerInCircle()
    {
        var hitColliders = Physics.OverlapSphere(transform.position,10, targetLayer);
        if (hitColliders.Length > 0 )
        {
            if (hitColliders[0].CompareTag("Player")) { 
                return hitColliders[0].transform;
            }
            return null;
        }
        else
        {
            return null;
        }
    }
    public void SetHp(float hp)
    {
        _hpGauge.fillAmount = hp;
    }
    void Update()
    {
        var target = DetectPlayerInCircle();
        if (target != null) {
            cameraTransform = target.GetComponentInChildren<Camera>().transform;
        }

        if (cameraTransform == null) return;

        transform.rotation = cameraTransform.rotation * Quaternion.Euler(0, 180, 0);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10);
    }

    public void WhenStatChanged((float, string) data)
    {   
        currentHp = data.Item1;
        SetHp(currentHp / maxHp);
    }
}
