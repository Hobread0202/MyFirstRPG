using System;
using UnityEngine;

[Serializable]public class SaveData
{
    public string lastSceneName = "Stage1";
    public int currentHp = 100;
    public int currentExp = 0;
    public int currentLevel = 0;

    public float lastX = 19.28154f;
    public float lastY = 0.53f;
    public float lastZ = 18.13f;


    //불러오기용 벡터 변환
    public Vector3 lastPos
    {
        get { return new Vector3(lastX, lastY, lastZ); }
        set
        {
            lastX = value.x;
            lastY = value.y;
            lastZ = value.z;
        }
    }
}
