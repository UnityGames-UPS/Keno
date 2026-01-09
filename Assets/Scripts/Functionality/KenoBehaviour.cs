using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class KenoBehaviour : MonoBehaviour
{
  [Header("Lists")]
  [SerializeField] private List<KenoButton> KenoButtonScripts;
  [SerializeField] private List<int> SelectedList;
  [SerializeField] private List<int> ResultList;
  [SerializeField] private Transform Ball_Transform;
  [SerializeField] private TMP_Text Balls_Text;

  [Header("Integers")]
  [SerializeField] internal int selectionCounter = 0;
  [SerializeField] internal int ResultCounter = 0;

  [Header("Scripts")]
  [SerializeField] private SocketIOManager socketIOManager;
  [SerializeField] private UIManager uiManager;

  [Header("Text")]
  [SerializeField] internal TMP_Text MainNumber_Text;

  [Header("GameObject")]
  [SerializeField] private GameObject DisableScreen_object;
  private List<int> templist = new List<int>();

  internal int betCounter = 1;

  internal bool IsKenoComplete = false;
  internal bool CheckPopup = false;

  [SerializeField] AudioController audioController;

  internal void PickRandoms(int num)
  {
    SelectedList.Clear();
    SelectedList.TrimExcess();
    foreach (KenoButton kc in KenoButtonScripts)
    {
      kc.ResetButton();
    }

    templist.Clear();
    templist.TrimExcess();
    templist = GenerateRandomNumbers(num, 0, 79);
    selectionCounter = 0;

    for (int i = 0; i < templist.Count; i++)
    {
      KenoButtonScripts[templist[i]].OnKenoSelect();
    }
  }

  private static List<int> GenerateRandomNumbers(int count, int minValue, int maxValue)
  {
    List<int> possibleNumbers = new List<int>();
    List<int> chosenNumbers = new List<int>();

    for (int index = minValue; index < maxValue; index++)
      possibleNumbers.Add(index);

    while (chosenNumbers.Count < count)
    {
      int position = Random.Range(0, possibleNumbers.Count);
      chosenNumbers.Add(possibleNumbers[position]);
      possibleNumbers.RemoveAt(position);
    }
    return chosenNumbers;
  }

  internal void AddKeno(int value)
  {
    if (!uiManager.isReset)
    {
      SelectedList.Add(value);
    }

    if (selectionCounter >= 1)
    {
      uiManager.CheckPlayButton(true);
      uiManager.ClearBlackImage.SetActive(false);
      uiManager.Clear_Button.interactable = true;
    }
    else
    {
      uiManager.CheckPlayButton(false);
      uiManager.ClearBlackImage.SetActive(true);
      uiManager.Clear_Button.interactable = false;
    }
    uiManager.UpdateSelectedText();
  }

  internal void RemoveKeno(int value)
  {
    SelectedList.Remove(value);
    if (selectionCounter >= 1)
    {
      uiManager.CheckPlayButton(true);
      uiManager.ClearBlackImage.SetActive(false);
      uiManager.Clear_Button.interactable = true;
    }
    else
    {
      uiManager.CheckPlayButton(false);
      uiManager.ClearBlackImage.SetActive(true);
      uiManager.Clear_Button.interactable = false;
    }
    uiManager.UpdateSelectedText();
  }

  internal void ShowMaxPopup()
  {
    uiManager.MaxPopupEnable();
  }

  internal void PlayKeeno()
  {
    if (DisableScreen_object) DisableScreen_object.SetActive(true);
    ResultList.Clear();
    ResultList.TrimExcess();
    // ResultList = GenerateRandomNumbers(20, 0, 79);
    StartCoroutine(PlayGameRoutine());
  }

  private IEnumerator PlayGameRoutine()
  {

    // Debug.Log($"Starting Keno Game with {SelectedList.Count} selections.");
    IsKenoComplete = false;

    if (socketIOManager)
    {
      socketIOManager.isResultdone = false;
      socketIOManager.AccumulateResult(betCounter, SelectedList);
    }

    yield return new WaitUntil(() => socketIOManager.isResultdone);

    ResultList = socketIOManager.resultData.drawn;

    for (int i = 0; i < ResultList.Count; i++)
    {
      audioController.PlayKenoAudio(3);
      if (Balls_Text) Balls_Text.text = ResultList[i].ToString();
      if (MainNumber_Text) MainNumber_Text.text = (i + 1).ToString();

      Ball_Transform.localScale = Vector3.zero;
      if (uiManager.turboSpin)
      {
        Ball_Transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.4f);
      }
      else
      {
        Ball_Transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(1f);
      }

      KenoButtonScripts[ResultList[i] - 1].ResultColor();
      yield return new WaitForSeconds(0.1f);
    }
    CheckPopup = true;

    uiManager.CheckFinalWinning();
    
    uiManager.EnableReset();
    
    if (DisableScreen_object) DisableScreen_object.SetActive(false);
    yield return new WaitUntil(() => !CheckPopup);
    uiManager.BalanceAmt_Text.text = socketIOManager.playerdata.balance.ToString("F2");

    yield return new WaitForSeconds(0.5f);
    IsKenoComplete = true;
    uiManager.turboSpin = false;
    uiManager.GameButtonToggle(true);
    audioController.StopMainAudio();
  }

  internal void ResetButtons()
  {
    for (int i = 0; i < KenoButtonScripts.Count; i++)
    {
      KenoButtonScripts[i].ResetButton();
    }

    ResultCounter = 0;
    selectionCounter = 0;

    for (int i = 0; i < SelectedList.Count; i++)
    {
      KenoButtonScripts[SelectedList[i] - 1].OnKenoSelect();
    }

    if (MainNumber_Text) MainNumber_Text.text = "00";
  }

  internal void CleanPage()
  {
    for (int i = 0; i < KenoButtonScripts.Count; i++)
    {
      KenoButtonScripts[i].ResetButton();
    }
    if (MainNumber_Text) MainNumber_Text.text = "00";
    SelectedList.Clear();
    SelectedList.TrimExcess();
    ResultCounter = 0;
    selectionCounter = 0;
  }
}
