using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class KenoButton : MonoBehaviour
{
  [SerializeField]
  private Button This_Button;
  [SerializeField]
  private TMP_Text Normal_Text;
  [SerializeField]
  private TMP_Text Black_Text;
  [SerializeField]
  private Image This_Image;
  [SerializeField]
  private Sprite Black_Sprite;
  [SerializeField]
  private Sprite Blue_Sprite;
  [SerializeField]
  private Sprite Red_Sprite;
  [SerializeField]
  private Sprite Yellow_Sprite;
  [SerializeField]
  private KenoBehaviour KenoManager;

  internal bool isActive = false;
  [SerializeField] private AudioController audioController;

  private void Start()
  {
    This_Button = this.gameObject.GetComponent<Button>();
    This_Image = this.gameObject.GetComponent<Image>();
    Normal_Text = this.transform.GetChild(0).GetComponent<TMP_Text>();
    Black_Text = this.transform.GetChild(1).GetComponent<TMP_Text>();
    Black_Text.color = Color.black;
    Black_Text.gameObject.SetActive(false);
    if (This_Button) This_Button.onClick.RemoveAllListeners();
    if (This_Button) This_Button.onClick.AddListener(OnKenoSelect);
  }

  internal void OnKenoSelect()
  {
    audioController.PlayKenoAudio(0);
    isActive = !isActive;
    if (isActive)
    {
      if (KenoManager.selectionCounter < 15)
      {
        if (This_Image) This_Image.sprite = Blue_Sprite;
        Black_Text.gameObject.SetActive(true);
        Normal_Text.gameObject.SetActive(false);
        KenoManager.selectionCounter++;
        KenoManager.AddKeno(int.Parse(Black_Text.text));
      }
      else
      {
        KenoManager.ShowMaxPopup();
        isActive = false;
      }
    }
    else
    {
      if (This_Image) This_Image.sprite = Black_Sprite;
      Normal_Text.gameObject.SetActive(true);
      Black_Text.gameObject.SetActive(false);
      KenoManager.selectionCounter--;
      KenoManager.RemoveKeno(int.Parse(Normal_Text.text));
    }
  }

  internal void ResultColor()
  {
    if (This_Image) This_Image.sprite = Red_Sprite;
    Black_Text.gameObject.SetActive(true);
    Normal_Text.gameObject.SetActive(false);
    if (isActive)
    {
      audioController.PlayKenoAudio(4);
      if (This_Image) This_Image.sprite = Yellow_Sprite;
      Black_Text.gameObject.SetActive(true);
      Normal_Text.gameObject.SetActive(false);
      KenoManager.ResultCounter++;
    }
  }

  internal void ResetButton()
  {
    isActive = false;
    if (This_Image) This_Image.sprite = Black_Sprite;
    Normal_Text.gameObject.SetActive(true);
    Black_Text.gameObject.SetActive(false);
  }

}
