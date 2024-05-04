using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static AnimationManager;
using System.Collections;

public class CardItem : MonoBehaviour
{
    public int Id;
    public Vector2 GridPosition;
    public bool IsMine;
    public bool IsClicked;
    public bool HasMatched;
    public bool Hidden;
    public Image CardImage;
    public RectTransform CardRectTransform;
    public EventTrigger EventTrigger;
    public GameObject CardObject;
    public AnimationManager AnimationManager;
    public void Flip(TweenEffect tweenEffect)
    {
        Hidden = !Hidden;
        StartCoroutine(AnimateFlip(tweenEffect));
    }

    private IEnumerator AnimateFlip(TweenEffect tweenEffect)
    {
        AnimationCurve curve = AnimationManager.GetAnimationCurve(tweenEffect.SelectedEffect);
        float duration = tweenEffect.Delay;

        // Calculate the halfway point of the animation
        float halfwayTime = duration / 2f;

        // Flip to show
        if (Hidden)
        {
            Quaternion startRotation = CardRectTransform.GetChild(0).rotation;
            Quaternion targetRotation = Quaternion.Euler(0f, 180f, 0f);

            float timeElapsed = 0f;
            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;
                float curveValue = curve.Evaluate(t);
                CardRectTransform.GetChild(0).rotation = Quaternion.Lerp(startRotation, targetRotation, curveValue);

                // Disable the CardImage halfway through
                if (timeElapsed >= halfwayTime)
                {
                    CardImage.gameObject.SetActive(false);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            // Ensure the rotation is set correctly at the end
            CardRectTransform.GetChild(0).rotation = targetRotation;
        }
        // Flip to hide
        else
        {
            Quaternion startRotation = CardRectTransform.GetChild(0).rotation;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0f);

            float timeElapsed = 0f;
            while (timeElapsed < duration)
            {
                float t = timeElapsed / duration;
                float curveValue = curve.Evaluate(t);
                CardRectTransform.GetChild(0).rotation = Quaternion.Lerp(startRotation, targetRotation, curveValue);

                // Disable the CardImage halfway through
                if (timeElapsed >= halfwayTime)
                {
                    CardImage.gameObject.SetActive(true);
                }

                yield return null;
                timeElapsed += Time.deltaTime;
            }

            // Ensure the rotation is set correctly at the end
            CardRectTransform.GetChild(0).rotation = targetRotation;
        }
    }

    public IEnumerator ScaleWithCurve(Transform targetTransform, Vector3 targetScale, AnimationCurve curve, float duration)
    {
        float timeElapsed = 0f;
        Vector3 initialScale = targetTransform.localScale;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float curveValue = curve.Evaluate(t);
            targetTransform.localScale = Vector3.Lerp(initialScale, targetScale, curveValue);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the target scale is set correctly at the end
        targetTransform.localScale = targetScale;
    }
}
