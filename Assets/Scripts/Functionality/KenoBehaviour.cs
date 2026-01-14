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
  [SerializeField] internal TMP_Text Balls_Text;

  [Header("RectTransforms")]
  [SerializeField] private List<RectTransform> ballTransforms;
  [SerializeField] private List<TMP_Text> ballTexts;
  [SerializeField] private List<RectTransform> finalPositions;
  [SerializeField] private RectTransform initialPosition;
  [SerializeField] private RectTransform FirstCurve;
  [SerializeField] private RectTransform FirstCurveMiddle;
  [SerializeField] private RectTransform SecondCurve;
  [SerializeField] private RectTransform MiddleCurve;
  [SerializeField] private RectTransform ThirdCurve;
  [SerializeField] private float animationTime = 0.5f;

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

  private void Start()
  {
    for (int i = 0; i < ballTexts.Count; i++)
    {
      // ballTexts[i] = ballTransforms[i].GetComponentInChildren<TMP_Text>();
      ballTexts[i].text = "";
    }
    initialPosition.anchoredPosition = ballTransforms[0].anchoredPosition;
  }

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

    uiManager.PayoutHighlight.GetComponent<RectTransform>().anchoredPosition = uiManager.initialPayoutHighlightPosi;
    uiManager.PayoutHighlight.SetActive(true);

    if (socketIOManager)
    {
      socketIOManager.isResultdone = false;
      socketIOManager.AccumulateResult(betCounter, SelectedList);
    }

    yield return new WaitUntil(() => socketIOManager.isResultdone);

    ResultList = socketIOManager.resultData.drawn;
    yield return new WaitForSeconds(1f);

    for (int i = 0; i < ResultList.Count; i++)
    {
      audioController.PlayKenoAudio(3);
      if (Balls_Text)
      {
        Balls_Text.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, Random.Range(-7f, 7f));
      }
      if (Balls_Text) Balls_Text.text = ResultList[i].ToString();
      if (MainNumber_Text) MainNumber_Text.text = (i + 1).ToString();

      if (uiManager.turboSpin)
      {
        // Ball_Transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
        StartCoroutine(BallAnimation(i));
        yield return new WaitForSeconds(0.4f);
      }
      else
      {
        Ball_Transform.localScale = Vector3.zero;
        Ball_Transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.3f);
        RectTransform rt = Ball_Transform as RectTransform;
        rt.DOShakeAnchorPos(0.3f, new Vector2(6f, 0f), 50, 0, false, true);
        // yield return new WaitForSeconds(0.5f);
        StartCoroutine(BallAnimation(i));
        yield return new WaitForSeconds(0.7f);
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
    uiManager.GameButtonToggle(true);
    uiManager.turboSpin = false;
    uiManager.Turbo_Text.text = "TURBO<br>IS<br>OFF";
    audioController.StopMainAudio();
  }

  private IEnumerator BallAnimation(int ballNumber)
  {
    ballTexts[ballNumber].text = ResultList[ballNumber].ToString();
    ballTexts[ballNumber].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, Random.Range(-11f, 11f));

    Vector3[] path;
    Tween rotationTween = null;
    Tween moveTween = null;

    if (ballNumber < 20)
    {
      float totalDuration = animationTime;

      float rotationSpeed = Random.Range(1.5f, 2.5f);
      rotationTween = ballTransforms[ballNumber].DOLocalRotate(new Vector3(0, 0, -720), rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);

      if (ballNumber < 9)
      {
        path = new Vector3[]
        {
                ballTransforms[ballNumber].anchoredPosition,
                Vector3.Lerp(ballTransforms[ballNumber].anchoredPosition, FirstCurveMiddle.anchoredPosition, 0.5f),
                FirstCurveMiddle.anchoredPosition,
                Vector3.Lerp(FirstCurveMiddle.anchoredPosition, FirstCurve.anchoredPosition, 0.5f),
                FirstCurve.anchoredPosition,
                Vector3.Lerp(FirstCurve.anchoredPosition, SecondCurve.anchoredPosition, 0.3f),
                Vector3.Lerp(FirstCurve.anchoredPosition, SecondCurve.anchoredPosition, 0.7f),
                SecondCurve.anchoredPosition,
                Vector3.Lerp(SecondCurve.anchoredPosition, MiddleCurve.anchoredPosition, 0.4f),
                MiddleCurve.anchoredPosition,
                Vector3.Lerp(MiddleCurve.anchoredPosition, ThirdCurve.anchoredPosition, 0.6f),
                ThirdCurve.anchoredPosition,
                Vector3.Lerp(ThirdCurve.anchoredPosition, finalPositions[ballNumber].anchoredPosition, 0.5f),
                finalPositions[ballNumber].anchoredPosition
        };
        totalDuration = animationTime * 3.5f;
      }
      else if (ballNumber < 11)
      {
        path = new Vector3[]
        {
                ballTransforms[ballNumber].anchoredPosition,
                Vector3.Lerp(ballTransforms[ballNumber].anchoredPosition, FirstCurveMiddle.anchoredPosition, 0.5f),
                FirstCurveMiddle.anchoredPosition,
                Vector3.Lerp(FirstCurveMiddle.anchoredPosition, FirstCurve.anchoredPosition, 0.5f),
                FirstCurve.anchoredPosition,
                Vector3.Lerp(FirstCurve.anchoredPosition, SecondCurve.anchoredPosition, 0.3f),
                Vector3.Lerp(FirstCurve.anchoredPosition, SecondCurve.anchoredPosition, 0.7f),
                SecondCurve.anchoredPosition,
                Vector3.Lerp(SecondCurve.anchoredPosition, MiddleCurve.anchoredPosition, 0.5f),
                MiddleCurve.anchoredPosition,
                Vector3.Lerp(MiddleCurve.anchoredPosition, finalPositions[ballNumber].anchoredPosition, 0.5f),
                finalPositions[ballNumber].anchoredPosition
        };
        totalDuration = animationTime * 2.8f;
      }
      else if (ballNumber < 12)
      {
        path = new Vector3[]
        {
                ballTransforms[ballNumber].anchoredPosition,
                Vector3.Lerp(ballTransforms[ballNumber].anchoredPosition, FirstCurveMiddle.anchoredPosition, 0.5f),
                FirstCurveMiddle.anchoredPosition,
                Vector3.Lerp(FirstCurveMiddle.anchoredPosition, FirstCurve.anchoredPosition, 0.5f),
                FirstCurve.anchoredPosition,
                Vector3.Lerp(FirstCurve.anchoredPosition, SecondCurve.anchoredPosition, 0.4f),
                Vector3.Lerp(FirstCurve.anchoredPosition, SecondCurve.anchoredPosition, 0.8f),
                SecondCurve.anchoredPosition,
                Vector3.Lerp(SecondCurve.anchoredPosition, finalPositions[ballNumber].anchoredPosition, 0.5f),
                finalPositions[ballNumber].anchoredPosition
        };
        totalDuration = animationTime * 2.2f;
      }
      else
      {
        path = new Vector3[]
        {
                ballTransforms[ballNumber].anchoredPosition,
                Vector3.Lerp(ballTransforms[ballNumber].anchoredPosition, FirstCurveMiddle.anchoredPosition, 0.5f),
                FirstCurveMiddle.anchoredPosition,
                Vector3.Lerp(FirstCurveMiddle.anchoredPosition, FirstCurve.anchoredPosition, 0.5f),
                FirstCurve.anchoredPosition,
                Vector3.Lerp(FirstCurve.anchoredPosition, finalPositions[ballNumber].anchoredPosition, 0.5f),
                finalPositions[ballNumber].anchoredPosition
        };
        totalDuration = animationTime * 1.8f;
      }

      moveTween = ballTransforms[ballNumber].DOLocalPath(path, totalDuration, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.InOutQuad);

      if (ballNumber < 9)
      {
        yield return new WaitForSeconds(1.07f);
        rotationTween.Kill();
        ballTransforms[ballNumber].GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 7);
        rotationTween = ballTransforms[ballNumber].DOLocalRotate(new Vector3(0, 0, 720), rotationSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        yield return new WaitForSeconds(totalDuration - 1.07f);
      }
      else
      {
        yield return new WaitForSeconds(totalDuration);
      }
      if (rotationTween != null && rotationTween.IsActive())
      {
        rotationTween.Kill();

        float currentZ = ballTransforms[ballNumber].localEulerAngles.z;
        
        float rotations = Mathf.Round(currentZ / 320f);
        float targetZ = rotations * 320f;

        ballTransforms[ballNumber].DOLocalRotate(new Vector3(0, 0, targetZ), 0.7f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic);

      }

      Vector3 finalPos = ballTransforms[ballNumber].anchoredPosition;
      ballTransforms[ballNumber].DOPunchAnchorPos(new Vector2(-5f, 0f), 0.3f, 5, 0.5f);
    }

    yield return new WaitForSeconds(0.15f);
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
    if (Balls_Text) Balls_Text.text = "";
    foreach (var b in ballTransforms)
    {
      b.anchoredPosition = initialPosition.anchoredPosition;
    }
    foreach (var t in ballTexts)
    {
      t.text = "";
    }
  }

  internal void CleanPage()
  {
    for (int i = 0; i < KenoButtonScripts.Count; i++)
    {
      KenoButtonScripts[i].ResetButton();
    }
    if (MainNumber_Text) MainNumber_Text.text = "00";
    uiManager.PayoutHighlight.SetActive(false);
    uiManager.ClearBlackImage.SetActive(true);
    SelectedList.Clear();
    SelectedList.TrimExcess();
    ResultCounter = 0;
    selectionCounter = 0;
  }
}
