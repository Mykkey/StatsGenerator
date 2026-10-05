using UnityEngine;

public class MatchController : MonoBehaviour
{
    public PlayerGenerator playerGenerator;
    public int currentPlayerIndex = 0;

    void Start()
    {
        playerGenerator = GameObject.Find("PlayerGenerator").GetComponent<PlayerGenerator>();
    }

    [ContextMenu("Spawn Next Player")]
    public void SpawnPlayer()
    {
        GameObject[] players = playerGenerator.players;
        if (currentPlayerIndex >= players.Length) {
            return;
        }
        players[currentPlayerIndex].SetActive(true);
        currentPlayerIndex++;
    }

    [ContextMenu("Move Player to Home")]
    public void MovePlayerToHome()
    {
        GameObject[] players = playerGenerator.players;
        if (currentPlayerIndex == 0) {
            return;
        } else {
            Player playerScript = players[currentPlayerIndex - 1].GetComponent<Player>();
            Checkpoints checkpoints = playerScript.plates;
            players[currentPlayerIndex - 1].transform.position = checkpoints.home;
        }
    }
}