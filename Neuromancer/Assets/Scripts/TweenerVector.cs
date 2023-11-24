using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TweenerVector is dedicated to tween a Vector; its syntax is very similar to Tweener.
    Note that the tweening functions are definded in Tweener.
*/

public class TweenerVector {

    private Tweener tweener;
    private float tweenAmount;

    private MonoBehaviour monoBehaviour;
    private IEnumerator currentCoroutine;
    private Action<Vector3> tweenVector;


    public TweenerVector(MonoBehaviour monoBehaviour, Action<Vector3> tweenVector) {
        tweener = new Tweener(monoBehaviour, res=>tweenAmount=res);
        this.monoBehaviour = monoBehaviour;
        this.tweenVector = tweenVector;
    }

    public void TweenWithTime(Vector3 startVector, Vector3 endVector, float time, Func<float, float> interpolation) {
        if(currentCoroutine != null) {
            monoBehaviour.StopCoroutine(currentCoroutine);
        }
        currentCoroutine = TweenCoroutine(startVector, endVector, time);
        tweener.TweenWithTime(0f, 1f, time, interpolation); // start a tweener that modifies tweenAmount
        monoBehaviour.StartCoroutine(currentCoroutine);
    }

    private IEnumerator TweenCoroutine(Vector3 startVector, Vector3 endVector, float time) {
        float t = 0;
        while (t < time) {
            t += Time.deltaTime;
            tweenVector.Invoke(Vector3.Lerp(startVector, endVector, tweenAmount)); // tweenAmount is modified by tweener, which is another running coroutine
            yield return null;
        }
        tweenVector.Invoke(endVector);
    }

    public bool IsTweening()
    {
        return tweener.IsTweening();
    }
}
