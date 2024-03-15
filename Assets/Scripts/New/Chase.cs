using UnityEngine;
using UnityEngine.Events;

public class Chase : MonoBehaviour
{
    public UnityEvent turnOnEvent;

    public UnityEvent turnOffEvent;

    public UnityEvent switchTargetEvent;

    public Player player;

    //private LineEffect lineEffect;

    private bool isOn;

    public Player currentTarget;

    private void Start()
    {
        //lineEffect = GetComponentInChildren<LineEffect>(includeInactive: true);
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        Player prey = PlayerManager.Instance.GetClosestPlayerToAim(transform.position, this.player.playerID);
        //Player player = PlayerManager.instance.target;

        if ((bool)prey && (Vector2.Angle(prey.transform.position - base.transform.position, this.player.movement.moveDirection) > 70f || this.player.movement.moveDirection == Vector3.zero))
        {
            prey = null;
        }
        if ((bool)prey)
        {
            if (currentTarget != this.player)
            {
                currentTarget = this.player;
                switchTargetEvent.Invoke();
                //lineEffect.Play(base.transform, player.transform);
            }
            if (!isOn)
            {
                isOn = true;
                turnOnEvent.Invoke();
                //Debug.Log("Chasing");
            }
        }
        else
        {
            if (isOn)
            {
                isOn = false;
                turnOffEvent.Invoke();
                //Debug.Log("Not Chase");
            }
            currentTarget = null;
        }
    }
}
