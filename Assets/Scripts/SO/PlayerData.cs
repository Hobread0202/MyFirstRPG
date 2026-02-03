using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "SO/PlayerData")]
public class PlayerData : ScriptableObject
{
    public int maxHp;
    public int speed;
    public int Damage;
    public int maxExp;
}