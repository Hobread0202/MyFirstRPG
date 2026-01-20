using System;
using UnityEngine;

public class Detectionarea : MonoBehaviour
{
    [SerializeField] string _tag;   //감지할태그 이름

    public event Action<Collider> OnTargetEnter;
    public event Action<Collider> OnTargetStay;
    public event Action<Collider> OnTargetExit;
        
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_tag))
        {
            OnTargetEnter?.Invoke(other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_tag))
        {
            OnTargetStay?.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_tag))
        {
            OnTargetExit?.Invoke(other);
        }
    }
}
