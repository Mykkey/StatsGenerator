using UnityEngine;

public class StatsGenerator : MonoBehaviour
{
    public int bounding;
    public Vector3 generateStats()
    {
        bounding = Random.Range(1, 100);
        int temp = Random.Range(1, 100);
        if (temp > bounding) bounding = temp;
        return new Vector3(Random.Range(1, bounding), Random.Range(1, bounding), Random.Range(1, bounding));
    }
}
