using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMovement : MonoBehaviour
{
    private BaseCharacter Data => PlayerManager.Instance.GetPlayerWithID(0);
    private PlayerMovement Movement => Data.movement;
    private PlayerJump Jump => Data.jump;
    private Gun Gun => Data.Gun;
    public List<Image> images;
    public List<GameObject> gameObjects;
    public List<GameObject> objActive;
    public Button hideUI;
    bool isLeftDown, isRightDown, isAttackDown;

    bool computerControl;

    private void OnEnable()
    {
        if (SkygoBridge.instance.isForRecording())
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        computerControl = false;
#if UNITY_EDITOR
        computerControl = true;
#endif
    }

    bool keyCodeADown = false;
    bool keyCodeDDown = false;

    private void Update()
    {
        if (SkygoBridge.instance.isForRecording() || computerControl)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                keyCodeADown = true;
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                keyCodeADown = false;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                keyCodeDDown = true;
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                keyCodeDDown = false;
            }
            if (keyCodeADown)
            {
                MoveLeft();
            }
            else if (keyCodeDDown)
            {
                MoveRight();
            }
            else
            {
                StopMove();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DoJump();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                UseOffHand();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                Attack();
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                EndAttack();
            }
        }

        if (isLeftDown)
        {
            Movement.moveDirection = new Vector3(-1, 0);
        }
        else if (isRightDown)
        {
            Movement.moveDirection = new Vector3(1, 0);
        }
        else
        {
            Movement.moveDirection = new Vector3(0, 0);
        }

        //attack always down
        //if (GameController.Instance.gamePaused || Movement.stunnedInput) isAttackDown = false;
        //else isAttackDown = true;

        if (isAttackDown)
        {
            Gun.Attack(Gun.currentCharge);
        }
    }
    bool isHide;
    public void HideUI()
    {
        if (!isHide)
        {
            isHide = true;
            foreach(Image image in images)
            {
                image.color = Color.clear;
            }
            foreach(GameObject gameObject in gameObjects)
            {
                if (gameObject.activeSelf)
                {
                    objActive.Add(gameObject);
                }
            }
            foreach (GameObject gameObject in objActive)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            isHide = false;
            foreach (Image image in images)
            {
                image.color = Color.white;
            }
            foreach (GameObject gameObject in objActive)
            {
                gameObject.SetActive(true);
            }
        }
    }
    public void ResetAllButtonsState()
    {
        StopMove();
        EndAttack();
    }

    public void MoveLeft()
    {
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        isLeftDown = true;
        isRightDown = false;
    }

    public void MoveRight()
    {      
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        isLeftDown = false;
        isRightDown = true;
    }

    public void StopMove()
    {     
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        isLeftDown = false;
        isRightDown = false;
    }

    public void LeftRelease()
    {        
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        isLeftDown = false;
    }

    public void RightRelease()
    {       
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        isRightDown = false;
    }

    public void DoJump()
    {
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        Jump.Jump();
    }

    public void Attack()
    {
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        isAttackDown = true;
    }

    public void EndAttack()
    {
        if (GameController.Instance.gamePaused) return;
        isAttackDown = false;
    }

    public void UseOffHand()
    {
        if (GameController.Instance.gamePaused) return;
        if (Movement.stunnedInput) return;
        Data.blockTrigger.DoBlock();
    }
}
