using UnityEngine;

public class ColumnConfig
{
    public readonly int Index;
    public readonly float SpinDuration;
    public readonly float SpinSpeed;
    public readonly float SpinDelay;
    public readonly AnimationCurve AnimationCurve;

    public ColumnConfig(int index, float spinDuration, float spinSpeed, float spinDelay, AnimationCurve animationCurve)
    {
        Index = index;
        SpinDuration = spinDuration;
        SpinSpeed = spinSpeed;
        SpinDelay = spinDelay;
        AnimationCurve = animationCurve;
    }
}