using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class PlayerAI : MonoBehaviour
{
    public float range = 6f;
    private BaseCharacter data;
    private Vector3 moveDir = Vector3.zero;
    private Vector3 aimDir = Vector3.zero;
    private Vector3 targetPos;
    public BaseCharacter target;
    public bool canSeeTarget;
    private float untilNextDataUpdate;
    private float getRandomTargetPosCounter;
    public bool isShooting;
    private float distanceToTarget = 5f;

    public Seeker seeker;
    Path p;
    public List<Vector3> path;

    float seeDistance = 8f;

    private void Start()
    {
        data = GetComponentInParent<BaseCharacter>();
        path = new List<Vector3>();
        seeDistance = Random.Range(9f, 12f);
        fakeDestination = new GameObject("AI Fake Destination");
    }

    void CreateSeeker()
    {
        seeker = gameObject.AddComponent<Seeker>();
    }

    bool enableMovement = false;
    float seekTime = 0f;
    float baseSeekTime = 0.8f;
    Vector3 curDestination = Vector3.zero;
    GameObject fakeDestination;

    float playerAggressiveCounter = 0f;
    float fightBackLine = 0.75f;
    int fightBackSide = 0;
    float fightBackDuration = 0.5f;
    float fightBackTime = 0f;
    float jumpCoolDown = 0f;
    float targetFindCoolDown = 5f;

    public void EnableMovement(bool flag)
    {
        enableMovement = flag;
    }

    private void Update()
    {
        return;
        if (GameFollowData.Instance.playingGameMode.Equals(GameMode.SandBox)) return;
        if (data.dead) return;
        if (!enableMovement) return;
        if (seeker == null) CreateSeeker();

        targetFindCoolDown -= TimeHandler.deltaTime;
        if (targetFindCoolDown <= 0 || target == null || (target != null && target.dead))
        {
            targetFindCoolDown = Random.Range(3f, 9f);
            if (PlayerManager.Instance != null && !data.fightingUI) target = PlayerManager.Instance.GetRandomPlayerToAim(data.player.playerID);
        }

        if (target == null || (target != null && target.dead))
        {
            Stand();
            return;
        }

        if (data.isStunned) return;
        if (fightBackTime > 0f)
        {
            // Debug.Log("panic bro");
            PanicMove();
        }
        else
        {
            // Debug.Log("just simple day");
            seekTime -= TimeHandler.deltaTime;
            if (seekTime <= 0)
            {
                fakeDestination.transform.position = target.transform.position;
                seekTime = baseSeekTime;
                p = seeker.StartPath(transform.position, fakeDestination.transform.position);
                p.BlockUntilCalculated();
                path = p.vectorPath;
            }
            if (path.Count != 0)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (Vector3.Distance(transform.position, path[0]) <= 0.5f) path.RemoveAt(0);
                //Move func
                if (path.Count != 0) MoveToPosition(path[0] - transform.position, distance);
            }
        }

        //attack phase
        canSeeTarget = PlayerManager.Instance.CanSeePlayer(base.transform.position, target).canSee;
        if (UnityEngine.Random.value < 0.1f / Mathf.Clamp(distanceToTarget * 0.1f, 0.1f, 10f) && canSeeTarget)
        {
            data.movement.shieldWasPressed = true;
        }

        if (UnityEngine.Random.value < 0.1f && canSeeTarget)
        {
            isShooting = true;
            if (data.isPlaying)
            {
                data.holding.holdable.GetComponent<Gun>().Attack(0);
            }
        }
        else
        {
            isShooting = false;
        }
        #region Action

        if (canSeeTarget)
        {
            targetPos = target.transform.position;
        }

        aimDir = (targetPos - base.transform.position).normalized;
        data.movement.target = target.targetToShoot.transform;

        #endregion
    }

    void Stand()
    {
        moveDir = Vector3.zero;
        data.movement.moveDirection = moveDir;
    }

    void PanicMove()
    {
        fightBackTime -= TimeHandler.deltaTime;
        moveDir = Vector3.zero;
        moveDir.x = fightBackSide;
        if (UnityEngine.Random.value < 0.4f || data.isWallGrab)
        {
            data.jump.Jump();
        }

        data.movement.moveDirection = moveDir;
    }

    void MoveToPosition(Vector3 movementVector, float cDistance)
    {
        moveDir = Vector3.zero;
        if (Mathf.Abs(movementVector.x) >= 0.15f)
        {
            moveDir.x = Mathf.Sign(movementVector.x);
            if (cDistance < seeDistance)
            {
                moveDir.x *= -1;
                playerAggressiveCounter += TimeHandler.deltaTime;
                if (playerAggressiveCounter >= fightBackLine)
                {
                    fightBackTime = fightBackDuration;
                    fightBackSide = (int)(Mathf.Sign(target.transform.position.x - transform.position.x));
                }
            }
            else
            {
                playerAggressiveCounter = 0f;
            }
        }
        jumpCoolDown += TimeHandler.deltaTime;

        bool flag1 = (movementVector.y > 0.25f && Mathf.Abs(movementVector.x) <= 0.25f);
        bool flag2 = data.isWallGrab;
        bool flag3 = UnityEngine.Random.value < 0.001f;
        if (flag1 || flag2 || flag3)
        {
            if (jumpCoolDown >= 0.2f)
            {
                //Debug.Log("flag1: " + flag1 + " flag2: " + flag2 + " flag3: " + flag3);
                jumpCoolDown = 0f;
                data.jump.Jump();
            }
        }

        data.movement.moveDirection = moveDir;
    }

    private void OnDestroy()
    {
        if (fakeDestination != null) Destroy(fakeDestination);
    }
}