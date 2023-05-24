using UnityEngine;

[CreateAssetMenu(fileName = "GenerationRule", menuName = "GenerationRule", order = 1)]
public class GenerationRule : ScriptableObject
{
    public float[] spawnRoomSpawn;
    public float[] spawnRoomClassic; // => peuvent changer au cours du temps s
}
