using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject NPC;
    public int xPos;
    public int zPos;
    public int npcCount;
    IEnumerator NPCDrop() 
    {
        while (npcCount < 50) 
        {
            xPos = Random.Range(5, 51);
            zPos = Random.Range(1, 31);
            Instantiate(NPC, new Vector3(xPos, 2.49f, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            npcCount += 1;
        }   
    }

    private void Start()
    {
        StartCoroutine(NPCDrop());
    }
}
