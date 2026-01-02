using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
  [Header("Buttons")]
  [SerializeField] private Button Random_Button;
  [SerializeField] private Button Play_Button;
  [SerializeField] private Button AutoPlay_Button;
  [SerializeField] private Button StopAutoPlay_Button;
  [SerializeField] private Button StakePlus_Button;
  [SerializeField] private Button StakeMinus_Button;
  [SerializeField] private Button Reset_Button;
  [SerializeField] private Button Delete_Button;
  [SerializeField] private Button GameExit_Button;
  [SerializeField] private Button MaxPopup_Button;
  [SerializeField] private Button Info_Button;
  [SerializeField] private Button CloseInfo_Button;

  [SerializeField] private Button Sound_Button;
  [SerializeField] private Button CloseSound_Button;
  [SerializeField] private Button Music_Button;
  [SerializeField] private Button CloseMusic_Button;

  [SerializeField] private Button QuitGame_Button;
  [SerializeField] private Button YesQuit_Button;
  [SerializeField] private Button NoQuit_Button;
  [SerializeField] private Button LowBalanceOk_Button;



  [Header("Texts")]
  [SerializeField] private TMP_Text Stake_Text;
  [SerializeField] private TMP_Text PopupWin_Text;
  [SerializeField] private TMP_Text Win_Text;
  [SerializeField] private TMP_Text TotalBet_text;
  [SerializeField] private TMP_Text Desc_Text;
  public TMP_Text BalanceAmt_Text;

  [Header("Lists")]
  [SerializeField] private List<TMP_Text> Payout_Text;
  [SerializeField] private List<TMP_Text> Hits_Text;
  [SerializeField] private List<GameObject> Win_Objects;

  [Header("GameObjects")]
  [SerializeField] private GameObject Reset_Object;
  [SerializeField] private GameObject PlayAnim_Object;
  [SerializeField] private GameObject CoinValueDisable_object;
  [SerializeField] private GameObject StarAnim_Object;


  [Header("Scripts")]
  [SerializeField] private KenoBehaviour KenoManager;
  [SerializeField] private SocketIOManager socketManager;

  [Header("Popups")]
  [SerializeField] private GameObject MainPopup_Object;
  [SerializeField] private GameObject MaxPopup_Object;
  [SerializeField] private GameObject WinPopup_Object;
  [SerializeField] private Transform WinPopup_Transform;
  [SerializeField] private GameObject CoinAnim_Object;
  [SerializeField] private GameObject InfoScreen_object;
  [SerializeField] private GameObject QuitGame_Object;
  [SerializeField] private GameObject LowBalance_Object;


  [Header("Image Animation Script")]
  [SerializeField] private ImageAnimation TitleAnim;

  private int stake = 5;
  internal bool isReset = false;

  [Header("Disconnection Popup")]
  [SerializeField]
  private Button CloseDisconnect_Button;
  [SerializeField]
  private GameObject DisconnectPopup_Object;

  [Header("Reconnection Popup")]
  [SerializeField]
  private TMP_Text reconnect_Text;
  [SerializeField]
  private GameObject ReconnectPopup_Object;
  internal bool IsAutoPlay = false;

  [SerializeField] private AudioController audioController;
  [SerializeField] private Button skipwinButton;
  internal bool IsQuitSelf =false;

  [SerializeField] internal GameObject SelectionIndicatorObject;

  void Start()
  {
    IsAutoPlay = false;
    if (Random_Button) Random_Button.onClick.RemoveAllListeners();
    if (Random_Button) Random_Button.onClick.AddListener(PickRandomIndices);

    if (Play_Button) Play_Button.onClick.RemoveAllListeners();
    if (Play_Button) Play_Button.onClick.AddListener(PlayKeeno);

    CheckPlayButton(false);

    if (StakePlus_Button) StakePlus_Button.onClick.RemoveAllListeners();
    if (StakePlus_Button) StakePlus_Button.onClick.AddListener(delegate { ChangeStake(true); audioController.PlayButtonAudio(); });

    if (StakeMinus_Button) StakeMinus_Button.onClick.RemoveAllListeners();
    if (StakeMinus_Button) StakeMinus_Button.onClick.AddListener(delegate { ChangeStake(false); audioController.PlayButtonAudio(); });

    if (Reset_Button) Reset_Button.onClick.RemoveAllListeners();
    if (Reset_Button) Reset_Button.onClick.AddListener(ResetGame);

    if (Delete_Button) Delete_Button.onClick.RemoveAllListeners();
    if (Delete_Button) Delete_Button.onClick.AddListener(CleanButtons);

    // if (GameExit_Button) GameExit_Button.onClick.RemoveAllListeners();
    // if (GameExit_Button) GameExit_Button.onClick.AddListener(CallOnExitFunction);    //TODO: Uncomment this line and change the function logic.

    if (MaxPopup_Button) MaxPopup_Button.onClick.RemoveAllListeners();
    if (MaxPopup_Button) MaxPopup_Button.onClick.AddListener(MaxPopupDisable);

    stake = 5;
    if (Stake_Text) Stake_Text.text = stake.ToString();
    if (TotalBet_text) TotalBet_text.text = stake.ToString();

    AutoPlay_Button.onClick.RemoveAllListeners();
    AutoPlay_Button.onClick.AddListener(AutoSpin);

    StopAutoPlay_Button.onClick.RemoveAllListeners();
    StopAutoPlay_Button.onClick.AddListener(delegate { StartCoroutine(StopAutoPlayKeeno()); });
    Info_Button.onClick.RemoveAllListeners();
    Info_Button.onClick.AddListener(OpenInfoPanel);
    CloseInfo_Button.onClick.RemoveAllListeners();
    CloseInfo_Button.onClick.AddListener(CloseInfoPanel);

    Music_Button.onClick.RemoveAllListeners();
    Music_Button.onClick.AddListener(delegate { audioController.PlayButtonAudio(); ToggleMusic(true); });

    CloseMusic_Button.onClick.RemoveAllListeners();
    CloseMusic_Button.onClick.AddListener(delegate { audioController.PlayButtonAudio(); ToggleMusic(false); });

    Sound_Button.onClick.RemoveAllListeners();
    Sound_Button.onClick.AddListener(delegate { audioController.PlayButtonAudio(); ToggleSound(true); });

    CloseSound_Button.onClick.RemoveAllListeners();
    CloseSound_Button.onClick.AddListener(delegate { audioController.PlayButtonAudio(); ToggleSound(false); });

    QuitGame_Button.onClick.RemoveAllListeners();
    QuitGame_Button.onClick.AddListener(OpenQuitGamePopup);

    YesQuit_Button.onClick.RemoveAllListeners();
    YesQuit_Button.onClick.AddListener(QuitGame);

    NoQuit_Button.onClick.RemoveAllListeners();
    NoQuit_Button.onClick.AddListener(CloseQuitGamePopup);
    skipwinButton.onClick.RemoveAllListeners();
    skipwinButton.onClick.AddListener(delegate { WinPopupDisable(); audioController.StopMainAudio(); });

    LowBalanceOk_Button.onClick.RemoveAllListeners();
    LowBalanceOk_Button.onClick.AddListener(delegate { CloseLowbalancePanel(); });

    CloseDisconnect_Button.onClick.RemoveAllListeners();
    CloseDisconnect_Button.onClick.AddListener(delegate { StartCoroutine(socketManager.CloseSocket()); });
    //if (Win_Text) Win_Text.text = winning.ToString();
    // Application.ExternalCall("window.parent.postMessage", "OnEnter", "*");
  }

  // private void CallOnExitFunction()
  // {
  //   Application.ExternalCall("window.parent.postMessage", "onExit", "*");
  // }

  private void PlayKeeno()
  {
    if (socketManager.playerdata.balance < socketManager.initialData.bets[KenoManager.betCounter])
    {
      LowBalancePopupEnable();
      if(IsAutoPlay) StartCoroutine(StopAutoPlayKeeno());
      return;
      }
    audioController.PlayButtonAudio();
    if (StarAnim_Object) StarAnim_Object.SetActive(true);
    if (isReset)
    {
      ResetGame();
    }
    isReset = true;
    CheckPlayButton(false);
    if (Delete_Button) Delete_Button.interactable = false;
    if (CoinValueDisable_object) CoinValueDisable_object.SetActive(true);
    Debug.Log($"Balance before play " + (socketManager.playerdata.balance - socketManager.initialData.bets[KenoManager.betCounter]));
    BalanceAmt_Text.text = (socketManager.playerdata.balance - socketManager.initialData.bets[KenoManager.betCounter]).ToString("f2");
    KenoManager.PlayKeeno();
    DOVirtual.DelayedCall(0.5f, () =>
    {
      if (StarAnim_Object) StarAnim_Object.SetActive(false);
    });
  }


  private void AutoSpin()
  {
    audioController.PlayButtonAudio();
    if (!IsAutoPlay)
    {

      IsAutoPlay = true;
      if (StopAutoPlay_Button) StopAutoPlay_Button.gameObject.SetActive(true);
      if (AutoPlay_Button) AutoPlay_Button.gameObject.SetActive(false);
      StartCoroutine(AutoPlayKeenoRoutine());

    }
  }

  private IEnumerator AutoPlayKeenoRoutine()
  {
    while (IsAutoPlay )
    {
      PlayKeeno();
      yield return null;
      yield return new WaitUntil(() => KenoManager.IsKenoComplete);
    }

  }

  private IEnumerator StopAutoPlayKeeno()
  {
    audioController.PlayButtonAudio();
    if (IsAutoPlay)
    {
      IsAutoPlay = false;
      if (StopAutoPlay_Button) StopAutoPlay_Button.gameObject.SetActive(false);
      if (AutoPlay_Button) AutoPlay_Button.gameObject.SetActive(true);
      //StartCoroutine(StopAutoSpinCoroutine());
      IsAutoPlay = false;
      // CheckPlayButton(true);
      yield return new WaitUntil(() => KenoManager.IsKenoComplete);
      EnableReset();
    }
    else
    {
      yield return null;
    }
  }

  private void ChangeStake(bool type)
  {
    if (type)
    {
      KenoManager.betCounter++;
      if (KenoManager.betCounter >= socketManager.initialData.bets.Count)
      {
        KenoManager.betCounter = 0;
      }
    }
    else
    {
      KenoManager.betCounter--;
      if (KenoManager.betCounter < 0)
      {
        KenoManager.betCounter = socketManager.initialData.bets.Count - 1;
      }
    }
    if (Stake_Text) Stake_Text.text = socketManager.initialData.bets[KenoManager.betCounter].ToString();
    if (TotalBet_text) TotalBet_text.text = socketManager.initialData.bets[KenoManager.betCounter].ToString();
    UpdateSelectedText();
  }

  internal void initGame()
  {
    UpdateSelectedText();
    BalanceAmt_Text.text = socketManager.playerdata.balance.ToString("F2");
    if (Stake_Text) Stake_Text.text = socketManager.initialData.bets[0].ToString();
    if (TotalBet_text) TotalBet_text.text = socketManager.initialData.bets[0].ToString();
    Desc_Text.text = socketManager.initUIData.description;
  }

  private void PickRandomIndices()
  {
    audioController.PlayButtonAudio();
    if (isReset)
    {
      ResetGame();
    }
    KenoManager.PickRandoms();
  }

  internal void CheckPlayButton(bool isActive)
  {
    if (Play_Button) Play_Button.interactable = isActive;
    if (Random_Button) Random_Button.interactable = isActive;
    if (AutoPlay_Button) AutoPlay_Button.interactable = isActive;
    if (PlayAnim_Object) PlayAnim_Object.SetActive(isActive);
  }

  private void WinPopupEnable()
  {
    CancelInvoke("WinPopupDisable");
    if (PopupWin_Text) PopupWin_Text.text = socketManager.resultData.currenWinning.ToString("F2");
    if (TitleAnim) TitleAnim.StartAnimation();
    if (WinPopup_Transform) WinPopup_Transform.localScale = Vector3.zero;
    if (MainPopup_Object) MainPopup_Object.SetActive(true);
    if (WinPopup_Object) WinPopup_Object.SetActive(true);
    if (WinPopup_Transform) WinPopup_Transform.DOScale(Vector3.one, 0.5f);
    if (CoinAnim_Object) CoinAnim_Object.SetActive(true);
    Invoke("WinPopupDisable", 5f);
  }

  private void WinPopupDisable()
  {
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
    if (WinPopup_Object) WinPopup_Object.SetActive(false);
    if (CoinAnim_Object) CoinAnim_Object.SetActive(false);
    KenoManager.CheckPopup = false;
  }

  internal void MaxPopupEnable()
  {
    CancelInvoke("MaxPopupDisable");
    if (MainPopup_Object) MainPopup_Object.SetActive(true);
    if (MaxPopup_Object) MaxPopup_Object.SetActive(true);
    Invoke("MaxPopupDisable", 2f);
  }

  private void MaxPopupDisable()
  {
    audioController.PlayButtonAudio();
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
    if (MaxPopup_Object) MaxPopup_Object.SetActive(false);
  }

  internal void LowBalancePopupEnable()
  {
    if (MainPopup_Object) MainPopup_Object.SetActive(true);
    if (LowBalance_Object) LowBalance_Object.SetActive(true);
  }

  internal void UpdateSelectedText()
  {
    BetAmountUpdate();
  }

  internal void CheckFinalWinning()
  {
    if (socketManager.resultData.currenWinning > 0)
    {
      WinningsTextUpdate(socketManager.resultData.currenWinning);
      WinPopupEnable();
      audioController.PlayMainAudio(2);
    }
    else
    {
      KenoManager.CheckPopup = false;

    }
  }

  internal void WinningsTextUpdate(double amount)
  {
    // Debug.Log("Here");
    if (Win_Text) Win_Text.text = amount.ToString("F2");
  }

  void BetAmountUpdate()
  {

    for (int i = 0; i < Payout_Text.Count; i++)
    {
      if (Payout_Text[i]) Payout_Text[i].text = string.Empty;
    }

    for (int i = 0; i < Hits_Text.Count; i++)
    {
      if (Hits_Text[i]) Hits_Text[i].text = string.Empty;
    }

    if (KenoManager.selectionCounter <= 1)
    {
      Hits_Text[0].text = "1";
      Payout_Text[0].text = (socketManager.initialData.paytable[0][0] * socketManager.initialData.bets[KenoManager.betCounter]).ToString("F2");
    }
    else
    {
      for (int i = 0; i < KenoManager.selectionCounter; i++)
      {
        if (Hits_Text[i]) Hits_Text[i].text = (i + 1).ToString();
      }
      for (int i = 0; i < socketManager.initialData.paytable[KenoManager.selectionCounter - 1].Count; i++)
      {
        if (Payout_Text[i]) Payout_Text[i].text = (socketManager.initialData.paytable[KenoManager.selectionCounter - 1][i] * socketManager.initialData.bets[KenoManager.betCounter]).ToString("F2");
      }
    }
  }

  internal void EnableReset()
  {
    if (Reset_Object) Reset_Object.SetActive(true);
    if (Delete_Button) Delete_Button.interactable = true;
    if (CoinValueDisable_object) CoinValueDisable_object.SetActive(false);
    CheckPlayButton(true);
  }

  private void ResetGame()
  {
    audioController.PlayButtonAudio();
    KenoManager.ResetWinAnim();
    if (TitleAnim) TitleAnim.StopAnimation();
    KenoManager.ResetButtons();
    if (Reset_Object) Reset_Object.SetActive(false);
    isReset = false;
    WinningsTextUpdate(0);
  }

  private void CleanButtons()
  {
    audioController.PlayButtonAudio();
    // BetAmountUpdate(0);
    UpdateSelectedText();
    WinningsTextUpdate(0);
    KenoManager.CleanPage();
    CheckPlayButton(false);
    SelectionIndicatorObject.SetActive(true);

  }

  internal void DisconnectionPopup()
  {
    OpenPopup(DisconnectPopup_Object);
  }

  internal void ReconnectionPopup()
  {
    OpenPopup(ReconnectPopup_Object);
  }

  internal void CheckAndClosePopups()
  {
    if (ReconnectPopup_Object.activeInHierarchy)
    {
      ClosePopup(ReconnectPopup_Object);
    }
    if (DisconnectPopup_Object.activeInHierarchy)
    {
      ClosePopup(DisconnectPopup_Object);
    }
  }

  private void ClosePopup(GameObject Popup)
  {
    if (Popup) Popup.SetActive(false);
    if (!DisconnectPopup_Object.activeSelf)
    {
      if (MainPopup_Object) MainPopup_Object.SetActive(false);
    }
  }

  private void OpenPopup(GameObject Popup)
  {
    if (Popup) Popup.SetActive(true);
    if (MainPopup_Object) MainPopup_Object.SetActive(true);
  }

  private void OpenInfoPanel()
  {
    audioController.PlayButtonAudio();
    OpenPopup(InfoScreen_object);
  }
  private void CloseInfoPanel()
  {
    audioController.PlayButtonAudio();
    ClosePopup(InfoScreen_object);
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
  }
  private void CloseLowbalancePanel()
  {
    audioController.PlayButtonAudio();
    // ClosePopup(LowBalance_Object);
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
    LowBalance_Object.SetActive(false);
  }

  private void ToggleSound(bool IsOn)
  {
    Debug.Log($"toggle Sound called " + IsOn);
    if (IsOn)
    {
      Sound_Button.gameObject.SetActive(false);
      CloseSound_Button.gameObject.SetActive(true);
      audioController.ToggleBgSound(false);

    }
    else
    {
      Sound_Button.gameObject.SetActive(true);
      CloseSound_Button.gameObject.SetActive(false);
      audioController.ToggleBgSound(true);
    }

  }

  private void ToggleMusic(bool IsOn)
  {
    Debug.Log($"toggle Music called " + IsOn);

    if (IsOn)
    {
      Music_Button.gameObject.SetActive(false);
      CloseMusic_Button.gameObject.SetActive(true);
      audioController.ToggleMainSound(false);
    }
    else
    {
      Music_Button.gameObject.SetActive(true);
      CloseMusic_Button.gameObject.SetActive(false);
      audioController.ToggleMainSound(true);
    }

  }

  private void OpenQuitGamePopup()
  {
    audioController.PlayButtonAudio();
    OpenPopup(QuitGame_Object);
  }
  private
  void CloseQuitGamePopup()
  {
    audioController.PlayButtonAudio();
    ClosePopup(QuitGame_Object);
  }
  private void QuitGame()
  {
    IsQuitSelf = true;
    audioController.PlayButtonAudio();
    StartCoroutine(socketManager.CloseSocket());
    ClosePopup(QuitGame_Object);
  }


}
