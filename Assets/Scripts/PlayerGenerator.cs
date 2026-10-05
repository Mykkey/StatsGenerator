using UnityEngine;

public class PlayerGenerator : MonoBehaviour
{
    public GameObject playerPrefab;
    public int numberOfPlayers = 10;
    public GameObject[] players;

    void Start()
    {
        players = new GameObject[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++) {
            GameObject playerObject = Instantiate(playerPrefab, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
            players[i] = playerObject;
            players[i].SetActive(false);
        }
    }
}
