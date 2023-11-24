using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Initialization:
        Declare an Tweener with `new Tweener(this, <tweenAction(float)>, (opt) <endAction>)`.
        tweenAction should be some lambda/function taking a float to be tweened: `result => <value to be tweened> = result`.
        endAction should be some lambda/function called when the tween finishes: `() => <statements>`.
    
    TweenWithTime(float startAmount, float endAmount, float time, Func<float, float> interpolation):
    TweenWithSpeed(float startAmount, float endAmount, float speed, Func<float, float> interpolation):
        Move the referenced value from startAmount to endAmount either within some given time or with some average speed.
        Interpolation is some function representing the shape of the transition from beginning to end.
            Use one defined below or define a custom curve.
            Function takes a time input from 0 to 1, and transitions from 0 to 1.

*/


public class Tweener {

    private MonoBehaviour monoBehaviour;
    private IEnumerator currentCoroutine;
    private Action<float> tweenAction;
    private Action endAction;
    private bool isTweening;

    public Tweener(MonoBehaviour monoBehaviour, Action<float> tweenAction, Action endAction) {
        this.monoBehaviour = monoBehaviour;
        this.tweenAction = tweenAction;
        this.endAction = endAction;
        this.isTweening = false;
    }

    public Tweener(MonoBehaviour monoBehaviour, Action<float> tweenAction) {
        this.monoBehaviour = monoBehaviour;
        this.tweenAction = tweenAction;
        this.endAction = null;
        this.isTweening = false;
    }

    public void TweenWithTime(float startAmount, float endAmount, float time, Func<float, float> interpolation) {
        if(currentCoroutine != null) {
            monoBehaviour.StopCoroutine(currentCoroutine);
        }
        currentCoroutine = TweenCoroutine(startAmount, endAmount, time, interpolation);
        monoBehaviour.StartCoroutine(currentCoroutine);
    }

    public void TweenWithSpeed(float startAmount, float endAmount, float speed, Func<float, float> interpolation) {
        if(currentCoroutine != null) {
            monoBehaviour.StopCoroutine(currentCoroutine);
        }
        float time = Mathf.Abs((endAmount - startAmount) / speed);
        currentCoroutine = TweenCoroutine(startAmount, endAmount, time, interpolation);
        monoBehaviour.StartCoroutine(currentCoroutine);
    }

    private IEnumerator TweenCoroutine(float startAmount, float endAmount, float time, Func<float, float> interpolation) {
        isTweening = true;
        float t = 0;
        while (t < time) {
            t += Time.unscaledDeltaTime;
            tweenAction?.Invoke((endAmount - startAmount) * interpolation(t / time) + startAmount);
            yield return null;
        }
        tweenAction?.Invoke(endAmount);
        endAction?.Invoke();
        isTweening = false;
    }

    public bool IsTweening()
    {
        return isTweening;
    }

    public static float JUMP(float t) => 1;
    public static float LINEAR(float t) => t;

    public static float QUAD_EASE_IN(float t) => t*t;
    public static float QUAD_EASE_OUT(float t) => 1-(1-t)*(1-t);
    public static float QUAD_EASE_IN_OUT(float t) => t<0.5 ? 2*t*t : 1-(-2*t + 2)*(-2*t + 2)/2;
}
