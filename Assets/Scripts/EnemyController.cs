using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public class EnemyController : MonoBehaviour
{
    NativeArray<float> yPos;
    NativeArray<float3> playerPos;
    int dir = 1;
    public float speed = 3.4f;
    [SerializeField] Transform[] checkpoints;

    SpriteRenderer _renderer;

    private void Awake()
    {
        checkpoints = new Transform[2];
        GameObject positions = GameObject.Find("Enemy_Pos");
        checkpoints[0] = positions.transform.GetChild(0);
        checkpoints[1] = positions.transform.GetChild(1);
    }
    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        playerPos = new NativeArray<float3>(1, Allocator.TempJob);
        yPos = new NativeArray<float>(1, Allocator.TempJob);       
        playerPos[0] = transform.position;
        yPos[0] = transform.position.x;     

        float deltatime = Time.deltaTime;
        GoMoveJob goMoveJob = new GoMoveJob
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
        if(transform.position.x >= checkpoints[1].position.x)
        {
            dir = -1;
            _renderer.flipX = true;
        }else if(transform.position.x <= checkpoints[0].position.x)
        {
            dir = 1;
            _renderer.flipX = false;
        }
    }

}

public struct GoMoveJob : IJobParallelFor
{
    public NativeArray<float3> moveY;
    public int dir;
    public float speed;
    [ReadOnly] public float deltaTime;

    void IJobParallelFor.Execute(int index)
    {
        moveY[index] += new float3(speed * dir * deltaTime, 0f, 0f);        
    }
}