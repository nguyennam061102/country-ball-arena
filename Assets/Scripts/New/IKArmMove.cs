
using UnityEngine;

public class IKArmMove : MonoBehaviour
{
    //private BaseCharacter data;

    public PlayerVelocity rig;

    public Transform target;

    private Vector3 startPos;

    private Holding holding;

    private bool isActive;

    private Vector3 velolcity;

    private float sinceRaise = 10f;

    private void Start()
    {
        //data = GetComponentInParent<BaseCharacter>();
        rig = GetComponentInParent<PlayerVelocity>();
        startPos = target.localPosition;
        holding = GetComponentInParent<Holding>();
    }

    private void FixedUpdate()
    {
        isActive = false;
        sinceRaise += TimeHandler.deltaTime;
        if ((bool) holding.holdable && (bool) holding.holdable.rig &&
            holding.holdable.rig.transform.position.x > rig.transform.position.x ==
            base.transform.position.x > rig.transform.position.x)
        {
            target.position = holding.holdable.rig.transform.position;
            velolcity = Vector3.zero;
            isActive = true;
            return;
        }

        Vector3 a = base.transform.parent.TransformPoint(startPos) + ((sinceRaise < 0.3f) ? Vector3.up : Vector3.zero);
        Vector3 a2 = rig.velocity;
        a2.x *= 0.3f;
        velolcity = FRILerp.Lerp(velolcity, (a - target.position) * 15f, 15f);
        target.position += velolcity * TimeHandler.deltaTime;
        target.position += a2 * -0.3f * TimeHandler.deltaTime;
    }

    public void RaiseHands()
    {
        if (!isActive)
        {
            sinceRaise = 0f;
            velolcity += Vector3.up * 20f;
        }
    }
}