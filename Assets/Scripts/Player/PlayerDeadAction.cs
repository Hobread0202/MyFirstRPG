using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadAction : MonoBehaviour
{
    [SerializeField] PlayerCtrl _player;
    void Start()
    {
        //중복체크
        if (_player != null)
        {

            _player.OnDead -= PlayDeadAction;


            _player.OnDead += PlayDeadAction;
        }
    }

    void PlayDeadAction()
    {
        
    }
}
