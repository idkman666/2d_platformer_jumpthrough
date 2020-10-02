using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    NativeArray<float> yPos;
    NativeArray<float3> playerPos;
    int dir = 1;

    public float speed = 5f;
    [SerializeField] Transform[] checkpoints;

    private void Awake()
    {
        string gameObjectName = gameObject.name;
        string positionGameObject = gameObjectName == "MovingPlatform" ? "Positions" : "EnemyPlatformPos";
        checkpoints = new Transform[2];
        GameObject positions = GameObject.Find(positionGameObject);
        checkpoints[0] = positions.transform.GetChild(0);
        checkpoints[1] = positions.transform.GetChild(1);
    }
    private void Start()
    {
        //_renderer = GetComponent<SpriteRenderer>();

    }
    private void Update()
    {
        playerPos = new NativeArray<float3>(1, Allocator.TempJob);
        yPos = new NativeArray<float>(1, Allocator.TempJob);
        playerPos[0] = transform.position;
        yPos[0] = transform.position.y;

        float deltatime = Time.deltaTime;
        PlatformMoveJob goMoveJob = new PlatformMoveJob
        {
            deltaTime = deltatime,
            moveY = playerPos,
            dir = dir,
            speed = speed
        };
        JobHandle jobHandle = goMoveJob.Schedule(1, 1);
        jobHandle.Complete();

        transform.position = playerPos[0];
        playerPos.Dispose();
        yPos.Dispose();

    }

    private void FixedUpdate()
    {
        if (transform.position.y >= checkpoints[1].position.y)
        {
            dir = -1;            
        }
        else if (transform.position.y <= checkpoints[0].position.y)
        {
            dir = 1;           
        }
    }

}
public struct PlatformMoveJob : IJobParallelFor
{
    public NativeArray<float3> moveY;
    public int dir;
    [ReadOnly] public float deltaTime;
    public float speed;

    void IJobParallelFor.Execute(int index)
    {
        moveY[index] += new float3(0, speed * dir * deltaTime, 0f);
    }
}
