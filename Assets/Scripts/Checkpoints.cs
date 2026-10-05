using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    GameObject homeObj;
    GameObject base1Obj;
    GameObject base2Obj;
    GameObject base3Obj;
    GameObject base4Obj;

    public Vector3 home;
    public Vector3 base1;
    public Vector3 base2;
    public Vector3 base3;

    private void Start()
    {
        homeObj = GameObject.Find("Home");
        base1Obj = GameObject.Find("Base1");
        base2Obj = GameObject.Find("Base2");
        base3Obj = GameObject.Find("Base3");
        home = homeObj.transform.position;
        base1 = base1Obj.transform.position;
        base2 = base2Obj.transform.position;
        base3 = base3Obj.transform.position;
    }

    public Vector3 GetPlate(int checkpointNumber)
    {
        switch (checkpointNumber) {
            case 0:
                return home;
            case 1:
                return base1;
            case 2:
                return base2;
            case 3:
                return base3;
            default:
                return Vector3.zero;
        }
    }
}
