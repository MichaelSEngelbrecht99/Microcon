using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [Serializable]
    public struct TweenEffect
    {
        public AnimationEffects SelectedEffect;
        public float Delay;
        public Vector3 SetScale;
        public AudioClip EffectClip;
    }

    [Serializable]
    public enum AnimationEffects
    {
        Linear, //Linear easing, maintains a constant rate of change.
        Clerp, //Circular easing, smooths the animation in and out.
        Spring, //Spring easing, creates a spring-like effect.
        EaseInQuad, //Quadratic easing, starts slowly and accelerates.
        EaseOutQuad,//Quadratic easing, starts fast and decelerates.
        EaseInOutQuad, //Quadratic easing, starts and ends slowly with acceleration in the middle.
        EaseInCubic, //Cubic easing, starts slowly and accelerates.
        EaseOutCubic, //Cubic easing, starts fast and decelerates.
        EaseInOutCubic, //Cubic easing, starts and ends slowly with acceleration in the middle.
        EaseInQuart, //Quartic easing, starts slowly and accelerates.
        EaseOutQuart, //Quartic easing, starts fast and decelerates.
        EaseInOutQuart, //Quartic easing, starts and ends slowly with acceleration in the middle.
        EaseInQuint, //Quintic easing, starts slowly and accelerates.
        EaseOutQuint,// Quintic easing, starts fast and decelerates.
        EaseInOutQuint,// Quintic easing, starts and ends slowly with acceleration in the middle.
        EaseInSine, //Sine easing, starts slowly and accelerates.
        EaseOutSine, //Sine easing, starts fast and decelerates.
        EaseInOutSine, //Sine easing, starts and ends slowly with acceleration in the middle.
        EaseInExpo, //Exponential easing, starts slowly and accelerates.
        EaseOutExpo, //Exponential easing, starts fast and decelerates.
        EaseInOutExpo, //Exponential easing, starts and ends slowly with acceleration in the middle.
        EaseInCirc, //Circular easing, starts slowly and accelerates.
        EaseOutCirc, //Circular easing, starts fast and decelerates.
        EaseInOutCirc, //Circular easing, starts and ends slowly with acceleration in the middle.
        EaseInBack, //Back easing, starts slightly behind the initial position before moving forward.
        EaseOutBack, //Back easing, starts moving forward and then slightly overshoots before settling.
        EaseInOutBack, //Back easing, starts slightly behind, overshoots, and settles.
        EaseInBounce, //Bounce easing, starts slowly and bounces before settling.
        EaseOutBounce, //Bounce easing, starts fast, bounces, and settles.
        EaseInOutBounce //Bounce easing, starts slowly, bounces, and settles.
    }
    public AnimationCurve Linear;  //Linear easing, maintains a constant rate of change.
    public AnimationCurve Clerp; //Circular easing, smooths the animation in and out.
    public AnimationCurve Spring;//Spring easing, creates a spring-like effect.
    public AnimationCurve EaseInQuad; //Quadratic easing, starts slowly and accelerates.
    public AnimationCurve EaseOutQuad;//Quadratic easing, starts fast and decelerates.
    public AnimationCurve EaseInOutQuad; //Quadratic easing, starts and ends slowly with acceleration in the middle.
    public AnimationCurve EaseInCubic; //Cubic easing, starts slowly and accelerates.
    public AnimationCurve EaseOutCubic; //Cubic easing, starts fast and decelerates.
    public AnimationCurve EaseInOutCubic; //Cubic easing, starts and ends slowly with acceleration in the middle.
    public AnimationCurve EaseInQuart; //Quartic easing, starts slowly and accelerates.
    public AnimationCurve EaseOutQuart; //Quartic easing, starts fast and decelerates.
    public AnimationCurve EaseInOutQuart; //Quartic easing, starts and ends slowly with acceleration in the middle.
    public AnimationCurve EaseInQuint; //Quintic easing, starts slowly and accelerates.
    public AnimationCurve EaseOutQuint;// Quintic easing, starts fast and decelerates.
    public AnimationCurve EaseInOutQuint;// Quintic easing, starts and ends slowly with acceleration in the middle.
    public AnimationCurve EaseInSine; //Sine easing, starts slowly and accelerates.
    public AnimationCurve EaseOutSine; //Sine easing, starts fast and decelerates.
    public AnimationCurve EaseInOutSine; //Sine easing, starts and ends slowly with acceleration in the middle.
    public AnimationCurve EaseInExpo; //Exponential easing, starts slowly and accelerates.
    public AnimationCurve EaseOutExpo; //Exponential easing, starts fast and decelerates.
    public AnimationCurve EaseInOutExpo; //Exponential easing, starts and ends slowly with acceleration in the middle.
    public AnimationCurve EaseInCirc; //Circular easing, starts slowly and accelerates.
    public AnimationCurve EaseOutCirc; //Circular easing, starts fast and decelerates.
    public AnimationCurve EaseInOutCirc; //Circular easing, starts and ends slowly with acceleration in the middle.
    public AnimationCurve EaseInBack; //Back easing, starts slightly behind the initial position before moving forward.      
    public AnimationCurve EaseOutBack; //Back easing, starts moving forward and then slightly overshoots before settling.
    public AnimationCurve EaseInOutBack; //Back easing, starts slightly behind, overshoots, and settles.
    public AnimationCurve EaseInBounce; //Bounce easing, starts slowly and bounces before settling.
    public AnimationCurve EaseOutBounce; //Bounce easing, starts fast, bounces, and settles.
    public AnimationCurve EaseInOutBounce; //Bounce easing, starts slowly, bounces, and settles.
    private static AnimationManager _instance;
    public static AnimationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AnimationManager>();
            }

            return _instance;
        }
    }
    public Dictionary<string, Animation> AnimationsList;
    [Serializable]
    public struct Animation
    {
        public string Name;
        public AnimationCurve SetAnimationCurve;
    }

    public AnimationCurve GetAnimationCurve(AnimationEffects effect)
    {
        switch (effect)
        {
            case AnimationEffects.Linear:
                return Linear;
            case AnimationEffects.Clerp:
                return Clerp;
            case AnimationEffects.Spring:
                return Spring;
            case AnimationEffects.EaseInQuad:
                return EaseInQuad;
            case AnimationEffects.EaseOutQuad:
                return EaseOutQuad;
            case AnimationEffects.EaseInOutQuad:
                return EaseInOutQuad;
            case AnimationEffects.EaseInCubic:
                return EaseInCubic;
            case AnimationEffects.EaseOutCubic:
                return EaseOutCubic;
            case AnimationEffects.EaseInOutCubic:
                return EaseInOutCubic;
            case AnimationEffects.EaseInQuart:
                return EaseInQuart;
            case AnimationEffects.EaseOutQuart:
                return EaseOutQuart;
            case AnimationEffects.EaseInOutQuart:
                return EaseInOutQuart;
            case AnimationEffects.EaseInQuint:
                return EaseInQuint;
            case AnimationEffects.EaseOutQuint:
                return EaseOutQuint;
            case AnimationEffects.EaseInOutQuint:
                return EaseInOutQuint;
            case AnimationEffects.EaseInSine:
                return EaseInSine;
            case AnimationEffects.EaseOutSine:
                return EaseOutSine;
            case AnimationEffects.EaseInOutSine:
                return EaseInOutSine;
            case AnimationEffects.EaseInExpo:
                return EaseInExpo;
            case AnimationEffects.EaseOutExpo:
                return EaseOutExpo;
            case AnimationEffects.EaseInOutExpo:
                return EaseInOutExpo;
            case AnimationEffects.EaseInCirc:
                return EaseInCirc;
            case AnimationEffects.EaseOutCirc:
                return EaseOutCirc;
            case AnimationEffects.EaseInOutCirc:
                return EaseInOutCirc;
            case AnimationEffects.EaseInBack:
                return EaseInBack;
            case AnimationEffects.EaseOutBack:
                return EaseOutBack;
            case AnimationEffects.EaseInOutBack:
                return EaseInOutBack;
            case AnimationEffects.EaseInBounce:
                return EaseInBounce;
            case AnimationEffects.EaseOutBounce:
                return EaseOutBounce;
            case AnimationEffects.EaseInOutBounce:
                return EaseInOutBounce;
            default:
                Debug.Log("Animation not found, using Linear as default");
                return Linear;
        }
    }

}
