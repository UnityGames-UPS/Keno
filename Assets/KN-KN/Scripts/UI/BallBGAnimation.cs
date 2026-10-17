using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class BallBGAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform wrapper;
    [SerializeField] private List<RectTransform> balls;
    [SerializeField] private float speed = 200f;
    // [SerializeField] private float rotationSpeed = 200f;

    private List<Vector2> directions = new List<Vector2>();

    void Start()
    {
        directions.Clear();
        foreach (var b in balls)
        {
            directions.Add(Random.insideUnitCircle.normalized);

            b.DOLocalRotate(new Vector3(0, 0, 360), Random.Range(7f,15f), RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        }
    }

    void Update()
    {
        float halfW = wrapper.rect.width * 0.5f;
        float halfH = wrapper.rect.height * 0.5f;

        for (int i = 0; i < balls.Count; i++)
        {
            RectTransform ball = balls[i];
            Vector2 direction = directions[i];

            Vector2 pos = ball.anchoredPosition;
            pos += direction * speed * Time.deltaTime;

            if (pos.x < -halfW || pos.x > halfW)
            {
                direction.x = -direction.x;
                pos.x = Mathf.Clamp(pos.x, -halfW, halfW);
            }

            if (pos.y < -halfH || pos.y > halfH)
            {
                direction.y = -direction.y;
                pos.y = Mathf.Clamp(pos.y, -halfH, halfH);
            }

            balls[i].anchoredPosition = pos;
            directions[i] = direction;
        }
    }
}
