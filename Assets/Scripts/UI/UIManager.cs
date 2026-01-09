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
  [SerializeField] private Button Play_Button;
  [SerializeField] private Button RND_Button;
  [SerializeField] internal Button Clear_Button;
  [SerializeField] private Button StakePlus_Button;
  [SerializeField] private Button StakeMinus_Button;
  [SerializeField] private Button Turbo_Button;
  [SerializeField] private Button Reset_Button;
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
  [SerializeField] private Button skipwinButton;

  [Header("Button Black Images")]
  [SerializeField] private GameObject RNDPanel;
  [SerializeField] private GameObject PlayBlackImage;
  [SerializeField] private GameObject BetUpBlackImage;
  [SerializeField] private GameObject BetDownBlackImage;
  [SerializeField] internal GameObject ClearBlackImage;

  [Header("Texts")]
  [SerializeField] internal TMP_Text Turbo_Text;
  [SerializeField] private TMP_Text PopupWin_Text;
  [SerializeField] private TMP_Text Win_Text;
  [SerializeField] private TMP_Text TotalBet_text;
  // [SerializeField] private TMP_Text Desc_Text;
  [SerializeField] internal TMP_Text BalanceAmt_Text;
  [SerializeField] private TMP_Text PayoutInitialText;

  [Header("Lists")]
  [SerializeField] internal List<RectTransform> HighlightTransform;
  [SerializeField] private List<TMP_Text> Payout_Text;
  [SerializeField] private List<TMP_Text> Hits_Text;
  [SerializeField] private List<Button> RND_NumberButtons;

  [Header("GameObjects")]
  [SerializeField] internal GameObject PayoutHighlight;
  [SerializeField] private GameObject Reset_Object;

  [Header("Scripts")]
  [SerializeField] private KenoBehaviour KenoManager;
  [SerializeField] private SocketIOManager socketManager;
  [SerializeField] private AudioController audioController;

  [Header("Popups")]
  [SerializeField] private GameObject MainPopup_Object;
  [SerializeField] private GameObject MaxPopup_Object;
  [SerializeField] private GameObject WinPopup_Object;
  [SerializeField] private Transform WinPopup_Transform;
  [SerializeField] private GameObject CoinAnim_Object;
  [SerializeField] private GameObject InfoScreen_object;
  [SerializeField] private GameObject QuitGame_Object;
  [SerializeField] private GameObject LowBalance_Object;


  [Header("Disconnection Popup")]
  [SerializeField]
  private Button CloseDisconnect_Button;
  [SerializeField]
  private GameObject DisconnectPopup_Object;

  [Header("Reconnection Popup")]
  [SerializeField]
  private GameObject ReconnectPopup_Object;

  internal Vector2 initialPayoutHighlightPosi;
  internal bool isReset = false;
  internal bool turboSpin = false;
  internal bool IsQuitSelf = false;

  private void Start()
  {
    if (Play_Button) Play_Button.onClick.RemoveAllListeners();
    if (Play_Button) Play_Button.onClick.AddListener(PlayKeeno);

    CheckPlayButton(false);

    if (StakePlus_Button) StakePlus_Button.onClick.RemoveAllListeners();
    if (StakePlus_Button) StakePlus_Button.onClick.AddListener(delegate { ChangeStake(true); audioController.PlayKenoAudio(2); });

    if (StakeMinus_Button) StakeMinus_Button.onClick.RemoveAllListeners();
    if (StakeMinus_Button) StakeMinus_Button.onClick.AddListener(delegate { ChangeStake(false); audioController.PlayKenoAudio(2); });

    if (RND_Button) RND_Button.onClick.RemoveAllListeners();
    if (RND_Button) RND_Button.onClick.AddListener(delegate { ToggleRNDNumbers(); audioController.PlayKenoAudio(2); });

    if (Reset_Button) Reset_Button.onClick.RemoveAllListeners();
    if (Reset_Button) Reset_Button.onClick.AddListener(delegate { ResetGame(); audioController.PlayKenoAudio(2); });

    if (Clear_Button) Clear_Button.onClick.RemoveAllListeners();
    if (Clear_Button) Clear_Button.onClick.AddListener(delegate { CleanButtons(); audioController.PlayKenoAudio(2); });

    if (Turbo_Button) Turbo_Button.onClick.RemoveAllListeners();
    if (Turbo_Button) Turbo_Button.onClick.AddListener(ToggleTurbo);

    if (MaxPopup_Button) MaxPopup_Button.onClick.RemoveAllListeners();
    if (MaxPopup_Button) MaxPopup_Button.onClick.AddListener(MaxPopupDisable);

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

    RNDNumberButtons();

    initialPayoutHighlightPosi = PayoutHighlight.GetComponent<RectTransform>().anchoredPosition;

  }

  private void PlayKeeno()
  {
    if (socketManager.playerdata.balance < socketManager.initialData.bets[KenoManager.betCounter])
    {
      LowBalancePopupEnable();
      return;
    }
    audioController.PlayKenoAudio(1);
    if (isReset)
    {
      ResetGame();
    }
    isReset = true;
    CheckPlayButton(false);
    if (Clear_Button) Clear_Button.interactable = false;
    Debug.Log($"Balance before play " + (socketManager.playerdata.balance - socketManager.initialData.bets[KenoManager.betCounter]));
    RNDPanel.SetActive(false);
    GameButtonToggle(false);
    BalanceAmt_Text.text = (socketManager.playerdata.balance - socketManager.initialData.bets[KenoManager.betCounter]).ToString("f2");
    KenoManager.PlayKeeno();
  }

  internal void GameButtonToggle(bool toggle)
  {
    Turbo_Button.gameObject.SetActive(!toggle);
    RND_Button.gameObject.SetActive(toggle);
    Clear_Button.gameObject.SetActive(toggle);
    StakePlus_Button.gameObject.SetActive(toggle);
    StakeMinus_Button.gameObject.SetActive(toggle);
  }

  private void ChangeStake(bool type)
  {
    if (type)
    {
      BetDownBlackImage.SetActive(false);
      StakeMinus_Button.interactable = true;
      KenoManager.betCounter++;
      if (KenoManager.betCounter == socketManager.initialData.bets.Count - 1)
      {
        // KenoManager.betCounter = 0;
        BetUpBlackImage.SetActive(true);
        StakePlus_Button.interactable = false;
      }
    }
    else
    {
      BetUpBlackImage.SetActive(false);
      StakePlus_Button.interactable = true;
      KenoManager.betCounter--;
      if (KenoManager.betCounter < 1)
      {
        // KenoManager.betCounter = socketManager.initialData.bets.Count - 1;
        KenoManager.betCounter = 0;
        BetDownBlackImage.SetActive(true);
        StakeMinus_Button.interactable = false;
      }
    }
    if (TotalBet_text) TotalBet_text.text = socketManager.initialData.bets[KenoManager.betCounter].ToString();
    UpdateSelectedText();
  }

  internal void initGame()
  {
    UpdateSelectedText();
    ClearBlackImage.SetActive(true);
    PayoutHighlight.SetActive(false);
    Clear_Button.interactable = false;
    BalanceAmt_Text.text = socketManager.playerdata.balance.ToString("F2");
    if (TotalBet_text) TotalBet_text.text = socketManager.initialData.bets[KenoManager.betCounter].ToString();
    // Desc_Text.text = socketManager.initUIData.description;
    KenoManager.MainNumber_Text.text = "00";
  }

  private void RNDNumberButtons()
  {
    for (int i = 0; i < RND_NumberButtons.Count; i++)
    {
      int index = i;
      Button btn = RND_NumberButtons[index];
      btn.onClick.RemoveAllListeners();
      btn.onClick.AddListener(() => PickRandomIndices(index + 1));
    }
  }

  private void PickRandomIndices(int num)
  {
    audioController.PlayButtonAudio();
    if (isReset)
    {
      ResetGame();
    }
    KenoManager.PickRandoms(num);
    ToggleRNDNumbers();
  }

  internal void CheckPlayButton(bool isActive)
  {
    if (Play_Button) Play_Button.interactable = isActive;
    if (isActive)
    {
      PlayBlackImage.SetActive(false);
    }
    else
    {
      PlayBlackImage.SetActive(true);
    }
  }

  private void WinPopupEnable()
  {
    CancelInvoke("WinPopupDisable");
    if (PopupWin_Text) PopupWin_Text.text = socketManager.resultData.currenWinning.ToString("F2");
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
      audioController.PlayMainAudio(5);
    }
    else
    {
      KenoManager.CheckPopup = false;
    }
  }

  internal void WinningsTextUpdate(double amount)
  {
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
    if (KenoManager.selectionCounter < 1)
    {
      PayoutInitialText.gameObject.SetActive(true);
    }
    // if (KenoManager.selectionCounter <= 1)
    // {
    //   Hits_Text[0].text = "1";
    //   Payout_Text[0].text = (socketManager.initialData.paytable[0][0] * socketManager.initialData.bets[KenoManager.betCounter]).ToString("F2");
    // }
    // 
    else
    {
      PayoutInitialText.gameObject.SetActive(false);

      Hits_Text[0].text = "0";
      Payout_Text[0].text = "0.00";

      for (int i = 0; i < KenoManager.selectionCounter; i++)
      {
        if (Hits_Text[i + 1]) Hits_Text[i + 1].text = (i + 1).ToString();
      }
      for (int i = 0; i < socketManager.initialData.paytable[KenoManager.selectionCounter - 1].Count; i++)
      {
        if (Payout_Text[i + 1]) Payout_Text[i + 1].text = (socketManager.initialData.paytable[KenoManager.selectionCounter - 1][i] * socketManager.initialData.bets[KenoManager.betCounter]).ToString("F2");
      }
    }
  }

  internal void HighlightPayout(int hits)
  {
    RectTransform highlightRect = PayoutHighlight.GetComponent<RectTransform>();
    RectTransform targetRect = HighlightTransform[hits];

    // Move using anchored position
    Vector2 targetPos = targetRect.anchoredPosition;

    highlightRect.DOKill();
    highlightRect.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.OutQuad);
  }

  internal void EnableReset()
  {
    if (Reset_Object) Reset_Object.SetActive(true);
    if (Clear_Button) Clear_Button.interactable = true;
    CheckPlayButton(true);
  }

  private void ResetGame()
  {
    // audioController.PlayKenoAudio(2);
    KenoManager.ResetButtons();
    if (Reset_Object) Reset_Object.SetActive(false);
    isReset = false;
    WinningsTextUpdate(0);
  }

  private void CleanButtons()
  {
    // audioController.PlayButtonAudio();
    // BetAmountUpdate(0);
    UpdateSelectedText();
    WinningsTextUpdate(0);
    KenoManager.CleanPage();
    UpdateSelectedText();
    CheckPlayButton(false);

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
    if (MainPopup_Object) MainPopup_Object.SetActive(false);
    LowBalance_Object.SetActive(false);
  }

  private void ToggleTurbo()
  {
    audioController.PlayKenoAudio(2);
    turboSpin = !turboSpin;
    if (turboSpin)
    {
      Turbo_Text.text = "TURBO<br>IS<br>ON";
    }
    else
    {
      Turbo_Text.text = "TURBO<br>IS<br>OFF";
    }
  }

  private void ToggleRNDNumbers()
  {
    if (RNDPanel.activeSelf)
    {
      RNDPanel.SetActive(false);
      if (KenoManager.selectionCounter > 0)
      {
        ClearBlackImage.SetActive(false);
      }
      BetUpBlackImage.SetActive(false);
      BetDownBlackImage.SetActive(false);
    }
    else
    {
      RNDPanel.SetActive(true);
      ClearBlackImage.SetActive(true);
      BetUpBlackImage.SetActive(true);
      BetDownBlackImage.SetActive(true);
    }
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
