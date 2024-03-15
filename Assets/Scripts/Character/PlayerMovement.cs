using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterStats Stats =>  CharacterStats.Instance;

    public float force;

    public float airControl = 0.3f;

    public float extraDrag;

    public float extraAngularDrag;

    public float wallGrabDrag;

    private BaseCharacter data;

    private CharacterStatModifiers stats;

    private float multiplier = 1f;

    public Vector3 aimDirection;
    public Vector3 moveDirection;

    //public void SetMoveDirection(Vector3 d)
    //{
    //    moveDirection = d;
    //    Debug.Log("set move dir to: " + d);
    //}

    public bool jumpWasPressed;

    public bool jumpIsPressed;

    public bool shootWasPressed;

    public bool shootIsPressed;

    public bool shootWasReleased;

    public bool shieldWasPressed;

    public bool editorMove;

    public bool controlledElseWhere;

    public bool stunnedInput;

    bool isInitPhakeShootPos = false;
    [SerializeField] HoldingObject phakeHoldingObject;
    [SerializeField] Transform phakeShootPos;

    private void Start()
    {
        data = GetComponent<BaseCharacter>();
        stats = GetComponent<CharacterStatModifiers>();

        if (Stats != null) force += force * Stats.TalentSpeed;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) editorMove = false;
        if (Application.platform == RuntimePlatform.WindowsEditor) editorMove = true;
    }

    private void FixedUpdate()
    {
        if (!data.isPlaying || data.UI)
        {
            return;
        }
        Move(moveDirection);
        if (data.isWallGrab && data.wallDistance < 0.7f)
        {
            Vector2 velocity = data.playerVel.velocity;
            if (data.movement.moveDirection.y >= 0f)
            {
                float x = data.movement.moveDirection.x;
            }

            data.playerVel.velocity = velocity;
        }
        data.playerVel.velocity -= data.playerVel.velocity * TimeHandler.timeScale * 0.01f * 0.1f * extraDrag * multiplier;
        data.playerVel.angularVelocity -= data.playerVel.angularVelocity * TimeHandler.timeScale * 0.01f * 0.1f * extraAngularDrag * multiplier;
    }

    public Transform target;
    Player targetPlayer;

    private void Update()
    {
        if (!isInitPhakeShootPos)
        {
            Holdable g = GetComponent<Holding>().holdable;
            phakeShootPos.transform.position = g.GetComponent<Gun>().shootPosition.position;
            isInitPhakeShootPos = true;
            return;
        }

        if (target != null)
        {
            switch (data.Gun.shootType)
            {
                case ShootType.Straight:
                    aimDirection = target.position - transform.position;
                    aimDirection.z = 0f;
                    aimDirection.Normalize();
                    break;
                case ShootType.Curve:
                    AimAtTarget();
                    break;
                case ShootType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (data.UI || data.fightingUI) return;
        if (editorMove && !controlledElseWhere && !stunnedInput)
        {
            moveDirection.x = Input.GetAxisRaw("Horizontal");
        }
        if (!data.AI)
        {
            targetPlayer = PlayerManager.Instance.GetClosestPlayerToAim(data.player.transform.position, data.player.playerID);
            if (targetPlayer != null) target = targetPlayer.transform;
        }
    }

    public void Move(Vector2 direction)
    {
        UpdateMultiplier();
        if (!data.isStunned)
        {
            direction.y = Mathf.Clamp(direction.y, -1f, 0f);
            direction.y *= 2f;
            data.playerVel.AddForce(direction * TimeHandler.timeScale * (1f - stats.slow) * stats.movementSpeed * force * data.playerVel.mass * 0.01f * multiplier * slowAmount, ForceMode2D.Force);
        }
    }

    private void UpdateMultiplier()
    {
        multiplier = 1f;
        if (!data.isGrounded)
        {
            multiplier = airControl;
        }
    }


    public ParticleSystem slowPart;
    public void IceStorm()
    {
        if (data.dead) return;
        slowPart.Play();
        StartCoroutine(IceStormCoro());
    }

    public float slowAmount = 1f;

    IEnumerator IceStormCoro()
    {
        data.healthHandler.TakeDamage(Vector3.zero, 15f, PlayerManager.Instance.players[data.player.playerID]);
        slowAmount = 0.5f;
        yield return new WaitForSeconds(3f);
        slowAmount = 1;
    }

    void AimAtTarget()
    {
        float shootForce = 50f;
        float gravity = 100f;
        List<ProjectilePosition> positionList = new List<ProjectilePosition>();
        Vector3 orgDir = target.position - transform.position;
        orgDir.Normalize();
        float yStep = 0.1f;

        for (int i = 0; i < 10; i++)
        {
            Vector3 newDir = orgDir + new Vector3(0, yStep * i, 0);
            phakeHoldingObject.transform.rotation = Quaternion.LookRotation(newDir);

            ProjectilePosition pp = new ProjectilePosition(newDir, shootForce, gravity);
            positionList.Add(pp);
            //new
            pp.GenerateAllPositionsAndGetBestResult(phakeShootPos.position, target.position);

            //foreach (Vector3 v in pp.positions)
            //{
            //    GameObject o = new GameObject();
            //    o.name = pp.aimDir.y.ToString();
            //    o.transform.position = v;
            //}
            //GameObject o1 = new GameObject();
            //o1.name = "best " + pp.aimDir.y.ToString();
            //o1.transform.position = pp.bestResult;
        }
        ProjectilePosition bestCase = null;
        float bestDistance = Mathf.Infinity;
        foreach (ProjectilePosition pp in positionList)
        {
            if (pp.bestDistance < bestDistance || bestCase == null)
            {
                bestDistance = pp.bestDistance;
                bestCase = pp;
            }
        }
        //Debug.Log(bestCase.aimDir);

        aimDirection = bestCase.aimDir;
    }

    void CreatePhakeHoldingObject()
    {

    }

}

public class ProjectilePosition
{
    public Vector3 aimDir;
    public List<Vector3> positions;
    public Vector3 bestResult;
    public float bestDistance = 9999f;
    float shootForce;
    float gravity;

    public ProjectilePosition(Vector3 dir, float force, float gravity)
    {
        this.aimDir = dir.normalized;
        this.shootForce = force;
        this.gravity = gravity;
        positions = new List<Vector3>();
    }

    public void GenerateAllPositionsAndGetBestResult(Vector3 aimer, Vector3 target)
    {
        Vector3 startV = aimDir * shootForce;
        float vX = startV.x;
        float vY = startV.y;
        Vector3 lastPos = aimer;
        Vector3 lastVel = startV;

        for (int i = 0; i < 40; i++)
        {
            //float t = (i + 1) * TimeHandler.deltaTime;
            lastVel += gravity * Vector3.down * TimeHandler.deltaTime;
            lastPos += lastVel * TimeHandler.deltaTime;
            float distance = Vector3.Distance(lastPos, target);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestResult = lastPos;
            }
            positions.Add(lastPos);
        }
    }
}
