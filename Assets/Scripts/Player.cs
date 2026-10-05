using UnityEngine;

public class Player : MonoBehaviour
{
    public Checkpoints plates;
    public GameObject statsGenerator;

    public string firstName;
    public string lastName;
    public int number;
    public int age;
    public Vector3 stats;

    void Awake()
    {
        plates = GameObject.Find("Checkpoints").GetComponent<Checkpoints>();

        firstName = assignFirstName();
        lastName = assignLastName();
        number = assignNumber();
        age = assignAge();
        stats = assignStats();
    }

    private string assignFirstName()
    {
        string[] firstNames = { "John", "Jane", "Michael", "Emily", "David", "Sarah", "Daniel", "Olivia", "Matthew", "Sophia" };
        return firstNames[Random.Range(0, firstNames.Length)];
    }
    private string assignLastName()
    {
        string[] lastNames = { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor" };
        return lastNames[Random.Range(0, lastNames.Length)];
    }
    private int assignNumber()
    {
        return Random.Range(0, 100);
    }
    private int assignAge()
    {
        return Random.Range(18, 40);
    }
    private Vector3 assignStats()
    {
        statsGenerator = GameObject.Find("StatsGenerator");
        StatsGenerator statsGeneratorScript = statsGenerator.GetComponent<StatsGenerator>();
        return statsGeneratorScript.generateStats();
    }
}
