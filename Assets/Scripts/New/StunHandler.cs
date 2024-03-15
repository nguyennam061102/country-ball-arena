//using Sonigon;
using UnityEngine;

public class StunHandler : MonoBehaviour
{
    //[Header("Sounds")]
    //public SoundEvent soundCharacterStunLoop;

    //private bool soundStunIsPlaying;

    //[Header("Settings")]
    //public CodeAnimation codeAnim;

    //private Player player;

    private BaseCharacter data;
    [SerializeField] private ParticleSystem stunFx;

    private void Start()
    {
        //player = GetComponent<Player>();
        data = GetComponent<BaseCharacter>();
    }

    private void Update()
    {
        if (data.stunTime > 0f)
        {
            data.stunTime -= TimeHandler.deltaTime;
            data.sinceGrounded = 0f;
            if (!data.isStunned)
            {
                StartStun();
            }
        }
        else if (data.isStunned)
        {
            StopStun();
        }
        // if (data.isStunned && data.isPlaying && !data.dead)
        // {
        //     if (!soundStunIsPlaying)
        //     {
        //         soundStunIsPlaying = true;
        //         SoundManager.Instance.Play(soundCharacterStunLoop, base.transform);
        //     }
        // }
        // else if (soundStunIsPlaying)
        // {
        //     soundStunIsPlaying = false;
        //     SoundManager.Instance.Stop(soundCharacterStunLoop, base.transform);
        // }
    }

    private void StartStun()
    {
        data.playerVel.velocity *= 0f;
        //data.playerVel.isKinematic = true;
        data.movement.stunnedInput = true;
        //codeAnim.PlayIn();
        data.isStunned = true;
        stunFx.Play();
    }

    public void StopStun()
    {
        //data.playerVel.isKinematic = false;
        data.movement.stunnedInput = false;
        // if (codeAnim.currentState == CodeAnimationInstance.AnimationUse.In)
        // {
        //     codeAnim.PlayOut();
        // }
        data.isStunned = false;
        data.stunTime = 0f;
        stunFx.Stop();
    }

    // private void OnDisable()
    // {
    //     codeAnim.transform.localScale = Vector3.zero;
    //     soundStunIsPlaying = false;
    //     SoundManager.Instance.Stop(soundCharacterStunLoop, base.transform);
    // }

    // private void OnDestroy()
    // {
    //     soundStunIsPlaying = false;
    //     SoundManager.Instance.Stop(soundCharacterStunLoop, base.transform);
    // }

    public void AddStun(float f)
    {
        if (!data.block.IsBlocking())
        {
            if (f > data.stunTime)
            {
                data.stunTime = f;
            }
            if (!data.isStunned)
            {
                StartStun();
            }
        }
    }
}