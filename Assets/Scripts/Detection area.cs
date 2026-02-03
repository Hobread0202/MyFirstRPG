using System;
using System.Collections.Generic;
using UnityEngine;

public class Detectionarea : MonoBehaviour
{
    [SerializeField] string _tag;   //감지할태그 이름

    public event Action<Collider> OnTargetEnter;
    public event Action<Collider> OnTargetStay;
    public event Action<Collider> OnTargetExit;

    //중복안되게 해쉬셋 목록
    private HashSet<Collider> _insideTargets = new HashSet<Collider>();

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

    public bool IsColliderInside(Collider col)
    {
        //해쉬목록에 콜라이더있는지 체크
        return _insideTargets.Contains(col);
    }
}
