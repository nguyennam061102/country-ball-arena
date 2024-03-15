using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBananaTalk : SingletonMonoBehavior<AgentBananaTalk>
{
    [SerializeField] GameObject panel, groupTalk, groupButtonGuide, groupReward;
    [SerializeField] UILabel lbContent, lbButtonContent;
    Action onContinueButton;
    Action onEasyGuideButton;
    [SerializeField] TweenRotation agentBanana;
    [SerializeField] TweenScale dialogCallScale;
    [SerializeField] TweenAlpha dialogCallAlpha;
    [SerializeField] TweenScale pButtonScale;
    [SerializeField] TweenAlpha pButtonAlpha;

    private void Start()
    {
        groupReward.SetActive(false);
    }

    IEnumerator RunAnimForAgentBanana(bool showAgent, int type)
    {
        if (showAgent)
        {
            agentBanana.ResetToBeginning();
            agentBanana.PlayForward();
            dialogCallScale.ResetToBeginning();
            dialogCallAlpha.ResetToBeginning();
            pButtonScale.ResetToBeginning();
            pButtonAlpha.ResetToBeginning();
            yield return new WaitForSecondsRealtime(agentBanana.duration * 1.1f);
        }
        if (type == 0)
        {
            dialogCallScale.ResetToBeginning();
            dialogCallAlpha.ResetToBeginning();
            dialogCallScale.PlayForward();
            dialogCallAlpha.PlayForward();
        }
        else if (type == 1)
        {
            pButtonScale.ResetToBeginning();
            pButtonAlpha.ResetToBeginning();
            pButtonScale.PlayForward();
            pButtonAlpha.PlayForward();
        }
    }

    public void ShowAgentGuide(string content, string buttonContent, Action onContinue)
    {
        bool showAgent = !panel.activeSelf;
        panel.SetActive(true);
        groupTalk.SetActive(true);
        groupButtonGuide.SetActive(false);
        onContinueButton = onContinue;
        lbContent.text = content;
        lbButtonContent.text = buttonContent;
        StartCoroutine(RunAnimForAgentBanana(showAgent, 0));
    }

    public void ClickContinueButton()
    {
        onContinueButton?.Invoke();
    }

    public void HidePanel()
    {
        groupReward.SetActive(false);
        panel.SetActive(false);
    }

    public void ShowPanelButtonIngame(Action onHidePanelButton)
    {
        bool showAgent = !panel.activeSelf;
        panel.SetActive(true);
        groupTalk.SetActive(false);
        groupButtonGuide.SetActive(true);
        onEasyGuideButton = onHidePanelButton;
        StartCoroutine(RunAnimForAgentBanana(showAgent, 1));
    }

    public void ShowGroupReward()
    {
        groupReward.SetActive(true);
    }

    public void ClickHidePanelButton()
    {
        onEasyGuideButton?.Invoke();
    }
}
