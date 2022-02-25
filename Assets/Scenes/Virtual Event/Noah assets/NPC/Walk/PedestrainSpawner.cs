using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrainSpawner : MonoBehaviour
{
    public GameObject pedestrianPrefab;
    public int pedestrianToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        int count = 0;
        while (count < pedestrianToSpawn)
        {
            GameObject obj = Instantiate(pedestrianPrefab);
            Transform child = transform.GetChild(Random.Range(0, transform.childCount - 1));
            obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();

            yield return new WaitForEndOfFrame();

            count++;
        }
    }
}
