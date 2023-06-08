using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "GenerationRule", order = 1)]
public class GenerationRule : ScriptableObject
{
    public float[] SpawnRoomSpawn;
    public float[] CapoucRoomSpawn;
    public float[] BossRoomSpawn;
}
