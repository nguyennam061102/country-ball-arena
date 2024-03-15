using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItsRainingCube : MonoBehaviour
{
    [SerializeField] Transform top;
    [SerializeField] GameObject cube;
    [SerializeField] MapTweener map;
    float timer;
    float spawnRate = 0.6f;
    int maxNumCube = 50;
    int numcube = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox)) return;
        if (numcube >= maxNumCube) return;
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            int i = Random.Range(0, 10);
            SpawnCube();
            if (i >= 4) SpawnCube();
            if (i >= 7) SpawnCube();
            if (i == 9) SpawnCube();
            timer = 0f;
        }
    }

    void SpawnCube()
    {
        Vector3 pos = new Vector3(Random.Range(-10f, 10f), top.transform.position.y * Random.Range(0.6f, 0.9f), 0);
        Rigidbody2D cb = Instantiate(cube, pos, Quaternion.identity).GetComponent<Rigidbody2D>();
        cb.transform.parent = this.map.transform;
        cb.transform.localScale = Vector3.one * 0.5f;
        cb.AddTorque(Random.Range(-100f, 100f));
        cb.AddForce(Vector2.down * Random.Range(0, 20f), ForceMode2D.Impulse);
        map.AddPhysicBlock(cb);
        numcube++;
    }
}
