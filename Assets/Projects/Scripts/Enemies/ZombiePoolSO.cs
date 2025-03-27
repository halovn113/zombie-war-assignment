using UnityEngine;

[CreateAssetMenu(fileName = "ZombiePoolData", menuName = "ScriptableObjects/ZombiePoolData", order = 0)]
public class ZombiePoolSO : ScriptableObject
{
    [SerializeField]
    public ZombiePoolInfo[] zombiePoolDatas;
}
