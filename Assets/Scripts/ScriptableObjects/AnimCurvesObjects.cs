using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimCurves", menuName = "ScriptableObjects/AnimCurves", order = 1)]
public class AnimCurvesObjects : ScriptableObject
{
    public AnimationCurve m_scaleCurve;
    public AnimationCurve m_scaleOffCurve;
    public AnimationCurve m_bouncingCurve;
    public float scaleDuration = 1;
    public float bouncingDuration = 0.5f;
}
