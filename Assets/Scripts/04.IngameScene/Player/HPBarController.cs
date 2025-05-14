using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour, IStatObserver
{
    [SerializeField] private PlayerController pc;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Image _hpGauge;

    private Transform cameraTransform;
    private float currentHp;
    private float maxHp;

    private void Awake()
    {
        if (pc != null && pc.CurrentHp != null)
            pc.CurrentHp.AddObserver(this);

        maxHp = pc.BaseMaxHp.Value;
    }

    private void Start()
    {
        TryAssignCamera();
    }

    private void TryAssignCamera()
    {
        if (cameraTransform != null) return;

        var hitColliders = Physics.OverlapSphere(transform.position, 20f, targetLayer);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                var cam = hit.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    cameraTransform = cam.transform;
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (cameraTransform == null)
        {
            TryAssignCamera();
            return;
        }

        transform.rotation = cameraTransform.rotation * Quaternion.Euler(0, 180f, 0);
    }

    public void SetHp(float ratio)
    {
        _hpGauge.fillAmount = Mathf.Clamp01(ratio);
    }

    public void WhenStatChanged((float, string) data)
    {
        switch (data.Item2)
        {
            case "currentHp":
                currentHp = data.Item1;
                break;
            case "baseMaxHp":
                maxHp = data.Item1;
                break;
        }

        if (maxHp > 0f)
        {
            SetHp(currentHp / maxHp);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
#endif
}
