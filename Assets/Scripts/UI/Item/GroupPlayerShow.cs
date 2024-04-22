using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GroupPlayerShow : MonoBehaviour
{
    [SerializeField] UILabel lbPlayerName;
    [SerializeField] private SkeletonAnimation characterAnim;
    [SerializeField] private SpriteRenderer offHand;
    [SerializeField] private SpriteRenderer eye;
    [SerializeField] private Sprite[] offHandList;
    [SerializeField] GameObject[] mainHand;

    public void ShowPlayer(string pName, int skinID, int mainHandID, int offHandID)
    {
        lbPlayerName.text = pName;
        characterAnim.skeleton.SetSkin(GameFollowData.Instance.skinList[skinID].connectedSkin.skinName);
        characterAnim.Skeleton.SetSlotsToSetupPose();
        characterAnim.AnimationState.Apply(characterAnim.Skeleton);
        var tmp = Random.Range(0, 2);
        switch (tmp)
        {
            case 0:
                characterAnim.state.SetAnimation(0, "0. Idle", true);
                characterAnim.timeScale = Random.Range(0.8f, 1.2f);
                break;
            case 1:
                characterAnim.state.SetAnimation(0, "0. Idle", true);
                characterAnim.timeScale = Random.Range(0.8f, 1.2f);
                break;
        }
        foreach (GameObject go in mainHand) go.SetActive(false);
        mainHand[mainHandID].gameObject.SetActive(true);
        offHand.sprite = offHandList[offHandID];
        eye.sprite = GameFollowData.Instance.skinList[skinID].connectedSkin.eye;
        //offHand.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        offHand.transform.SetEulerAnglesZAxis(20);
        offHand.transform.localRotation = Quaternion.Euler(0, 0, 20f);
    }
}
