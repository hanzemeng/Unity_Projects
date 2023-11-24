using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TweenerColor is dedicated to tween a Color; its syntax is very similar to Tweener.
    Note that the tweening functions are definded in Tweener.
*/

public class TweenerColor {

    private Tweener tweener;
    private float tweenAmount;

    private MonoBehaviour monoBehaviour;
    private IEnumerator currentCoroutine;
    private Action<Color> tweenColor;


    public TweenerColor(MonoBehaviour monoBehaviour, Action<Color> tweenColor) {
        tweener = new Tweener(monoBehaviour, res=>tweenAmount=res);
        this.monoBehaviour = monoBehaviour;
        this.tweenColor = tweenColor;
    }

    public void TweenWithTime(Color startColor, Color endColor, float time, Func<float, float> interpolation) {
        if(currentCoroutine != null) {
            monoBehaviour.StopCoroutine(currentCoroutine);
        }
        currentCoroutine = TweenCoroutine(startColor, endColor, time);
        tweener.TweenWithTime(0f, 1f, time, interpolation); // start a tweener that modifies tweenAmount
        monoBehaviour.StartCoroutine(currentCoroutine);
    }

    private IEnumerator TweenCoroutine(Color startColor, Color endColor, float time) {
        float t = 0;
        while (t < time) {
            t += Time.unscaledDeltaTime;
            tweenColor.Invoke(Color.Lerp(startColor, endColor, tweenAmount)); // tweenAmount is modified by tweener, which is another running coroutine
            yield return null;
        }
        tweenColor.Invoke(endColor);
    }

    public bool IsTweening()
    {
        return tweener.IsTweening();
    }
}
