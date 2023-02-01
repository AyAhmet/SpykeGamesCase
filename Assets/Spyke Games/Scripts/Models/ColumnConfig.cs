using UnityEngine;

public class ColumnConfig
{
    public readonly float SpinDuration;
    public readonly float SpinSpeed;
    public readonly float SpinDelay;
    public readonly AnimationCurve AnimationCurve;

    public ColumnConfig(float spinDuration, float spinSpeed, float spinDelay, AnimationCurve animationCurve)
    {
        SpinDuration = spinDuration;
        SpinSpeed = spinSpeed;
        SpinDelay = spinDelay;
        AnimationCurve = animationCurve;
    }
}