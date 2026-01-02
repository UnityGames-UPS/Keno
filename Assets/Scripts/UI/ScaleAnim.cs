using UnityEngine;
using DG.Tweening;

public class ScaleAnim : MonoBehaviour
{
    private void OnEnable()
    {
        transform.localScale = Vector3.one;

        // Kill previous tweens on this transform
        transform.DOKill();

        // Smooth pulse animation (up and down continuously)
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }
}
