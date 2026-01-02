using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KenoButton : MonoBehaviour
{
  [SerializeField]
  private Button This_Button;
  [SerializeField]
  private TMP_Text This_Text;
  [SerializeField]
  private Image This_Image;
  [SerializeField]
  private Sprite Pink_Sprite;
  [SerializeField]
  private Sprite Purple_Sprite;
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
    if (this.transform.GetChild(0).GetComponent<TMP_Text>() == null)
    {
      This_Text = this.transform.GetChild(1).GetComponent<TMP_Text>();
    }
    else
    {
      This_Text = this.transform.GetChild(0).GetComponent<TMP_Text>();
    }
    if (This_Button) This_Button.onClick.RemoveAllListeners();
    if (This_Button) This_Button.onClick.AddListener(OnKenoSelect);
  }

  internal void OnKenoSelect()
  {
    audioController.PlayKenoAudio(3);
    isActive = !isActive;
    if (isActive)
    {
      if (KenoManager.selectionCounter < 10)
      {
        if (This_Image) This_Image.sprite = Pink_Sprite;
        //if (This_Image) This_Image.color = _selectedColor;
        KenoManager.selectionCounter++;
        KenoManager.AddKeno(int.Parse(This_Text.text));
      }
      else
      {
        KenoManager.ShowMaxPopup();
        isActive = false;
      }
    }
    else
    {
      if (This_Image) This_Image.sprite = Purple_Sprite;
      //if (This_Image) This_Image.color = _normalColor;
      KenoManager.selectionCounter--;
      KenoManager.RemoveKeno(int.Parse(This_Text.text));
    }
  }

  internal void ResultColor()
  {
    if (This_Image) This_Image.sprite = Yellow_Sprite;
    //if (This_Image) This_Image.color = _resultColor;
    if (isActive)
    {
      if (This_Image) This_Image.sprite = Red_Sprite;
      //enableWinTick();
      KenoManager.ResultCounter++;
      KenoManager.CheckTransform(this.transform);
    }
  }

  internal void ResetButton()
  {
    isActive = false;
    if (This_Image) This_Image.sprite = Purple_Sprite;
  }

}
