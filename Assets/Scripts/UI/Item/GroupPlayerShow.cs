using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupPlayerShow : MonoBehaviour
{
    [SerializeField] UILabel lbPlayerName;
    [SerializeField] private SkeletonAnimation characterAnim;
    [SerializeField] private SpriteRenderer offHand;
    [SerializeField] private Sprite[] offHandList;
    [SerializeField] GameObject[] mainHand;

    public void ShowPlayer(string pName, int skinID, int mainHandID, int offHandID)
    {
        lbPlayerName.text = pName;
        characterAnim.skeleton.SetSkin(skinID.ToString());
        characterAnim.Skeleton.SetSlotsToSetupPose();
        characterAnim.AnimationState.Apply(characterAnim.Skeleton);
        var tmp = Random.Range(0, 2);
        switch (tmp)
        {
            case 0:
                characterAnim.state.SetAnimation(0, "idle 1", true);
                characterAnim.timeScale = Random.Range(0.8f, 1.2f);
                break;
            case 1:
                characterAnim.state.SetAnimation(0, "idle 2", true);
                characterAnim.timeScale = Random.Range(0.8f, 1.2f);
                break;
        }
        foreach (GameObject go in mainHand) go.SetActive(false);
        mainHand[mainHandID].gameObject.SetActive(true);
        offHand.sprite = offHandList[offHandID];
        offHand.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        offHand.transform.SetEulerAnglesZAxis(180);
        offHand.transform.localRotation = Quaternion.Euler(0, 0, 180f);
    }
}
