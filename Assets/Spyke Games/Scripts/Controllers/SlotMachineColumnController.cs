using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SlotMachineColumnController : MonoBehaviour
{
    [SerializeField] private List<SlotMachineSymbolController> m_SymbolControllers;

    public bool IsRolling { get; private set; }
    
    private Transform m_Transform;
    private ColumnConfig m_Config;
    private WaitForSeconds m_SpinDelayYield;
    private Dictionary<int, int> m_SymbolsIndexed;
    private float m_ColumnRepeatsSymbolsEvery;

    private const float SymbolGapVertical = 2.25f;


    private void Awake()
    {
        CalculateDynamicValues();
        CacheComponents();
        CacheSymbolIndexes();
        AddFillerSymbols();
    }

    public void SetConfig(ColumnConfig config)
    {
        m_Config = config;

        m_SpinDelayYield = new WaitForSeconds(m_Config.SpinDelay);
    }

    public void SetSymbolControllerSprites(List<Sprite> sharpSprites, List<Sprite> blurrySprites)
    {
        foreach (var symbolController in m_SymbolControllers)
        {
            var value = (int) symbolController.Symbol;
            var sharp = sharpSprites[value];
            var blurry = blurrySprites[value];
            symbolController.SetSharpAndBlurrySprites(sharp, blurry);
        }
    }

    private void CalculateDynamicValues()
    {
        m_ColumnRepeatsSymbolsEvery = SymbolGapVertical * m_SymbolControllers.Count;
    }

    private void CacheComponents()
    {
        m_Transform = GetComponent<Transform>();
    }

    private void CacheSymbolIndexes()
    {
        m_SymbolsIndexed = new Dictionary<int, int>();
        
        for (var i = 0; i < m_SymbolControllers.Count; i++)
        {
            m_SymbolsIndexed.Add((int) m_SymbolControllers[i].Symbol, i);
        }
    }

    private void AddFillerSymbols()
    {
        var topFiller = m_SymbolControllers[0];
        var topFiller2 = m_SymbolControllers[1];
        var bottomFiller = m_SymbolControllers[^1];

        var pos = m_Transform.position;
        var topFillerPosition = pos + new Vector3(0, m_SymbolControllers.Count * SymbolGapVertical, 0);
        var topFiller2Position = pos + new Vector3(0, (m_SymbolControllers.Count + 1) * SymbolGapVertical, 0);
        var bottomFillerPosition = pos + new Vector3(0, -SymbolGapVertical, 0);
        
        var topFillerGameObject = Instantiate(topFiller, topFillerPosition, Quaternion.identity, m_Transform);
        var topFiller2GameObject = Instantiate(topFiller2, topFiller2Position, Quaternion.identity, m_Transform);
        var bottomFillerGameObject = Instantiate(bottomFiller, bottomFillerPosition, Quaternion.identity, m_Transform);
        
        m_SymbolControllers.Add(topFillerGameObject);
        m_SymbolControllers.Add(topFiller2GameObject);
        m_SymbolControllers.Add(bottomFillerGameObject);
    }

    public void Spin(int targetSymbolValue, SpinStopAnimation stopAnimation)
    {
        StartCoroutine(SpinCoroutine(targetSymbolValue, stopAnimation));
    }

    private float GetDurationForStopAnimation(SpinStopAnimation stopAnim)
    {
        return stopAnim switch
        {
            SpinStopAnimation.Fast => 0,
            SpinStopAnimation.Normal => 1,
            SpinStopAnimation.Slow => 2.25f,
            _ => throw new ArgumentOutOfRangeException(nameof(stopAnim), stopAnim, null)
        };
    }

    private int GetExtraSpinCount(float startSpeed, float animDuration)
    {
        var averageSpeed = startSpeed / 2; // initial speed is 'startSpeed', final speed will be 0
        var distance = averageSpeed * animDuration;
        return Mathf.FloorToInt(distance / m_ColumnRepeatsSymbolsEvery);
    }

    private float GetSymbolLocalY(int symbolValue)
    {
        return m_SymbolsIndexed[symbolValue] * SymbolGapVertical;
    }
    
    private float GetTravelDistanceToSymbol(int symbolValue, Vector3 localPos)
    {
        var symbolLocalY = GetSymbolLocalY(symbolValue);
        var distance = symbolLocalY + localPos.y; // localPos.y is always negative, symbolLocalY is always positive. 
        return distance < 0 ? (distance + m_ColumnRepeatsSymbolsEvery) : distance;
    }

    private void SetBlurrySprites()
    {
        foreach (var symbol in m_SymbolControllers)
        {
            symbol.EnableBlurrySprite();
        }
    }

    private void SetSharpSprites()
    {
        foreach (var symbol in m_SymbolControllers)
        {
            symbol.EnableSharpSprite();
        }
    }

    private IEnumerator SpinCoroutine(int targetSymbolValue, SpinStopAnimation spinStopAnimation)
    {
        IsRolling = true;

        yield return m_SpinDelayYield;

        SetBlurrySprites();
        
        yield return MoveTransformLocalForSeconds(m_Config.SpinDuration, Vector3.down * m_Config.SpinSpeed);
        
        SetSharpSprites();

        if (spinStopAnimation == SpinStopAnimation.Fast)
            yield return MoveAndStopAtTargetImmediate(targetSymbolValue, Vector3.down * m_Config.SpinSpeed);
        else
            yield return MoveAndStopAtTargetWithAnimation(targetSymbolValue, spinStopAnimation, m_Config.SpinSpeed);

        IsRolling = false;
    }


    private IEnumerator MoveTransformLocalForSeconds(float duration, Vector3 velocity)
    {
        var localPos = m_Transform.localPosition;
        var startTime = Time.time;
        while(Time.time - startTime < duration)
        {
            localPos += velocity * Time.deltaTime;
            localPos.y %= m_ColumnRepeatsSymbolsEvery;
            m_Transform.localPosition = localPos;
            yield return null;
        }
    }


    private IEnumerator MoveAndStopAtTargetImmediate(int targetSymbolValue, Vector3 velocity)
    {
        var localPos = m_Transform.localPosition;
        var targetSymbolLocalPosY = GetSymbolLocalY(targetSymbolValue);
        var finalLocalPosition = new Vector3(localPos.x, -targetSymbolLocalPosY, localPos.z);
        var yDistance2Target = GetTravelDistanceToSymbol(targetSymbolValue, localPos);

        while (yDistance2Target > 0)
        {
            var deltaPos = velocity * Time.deltaTime;
            yDistance2Target -= Mathf.Abs(deltaPos.y);
            localPos += deltaPos;
            localPos.y %= m_ColumnRepeatsSymbolsEvery;
            m_Transform.localPosition = localPos;
            yield return null;
        }

        m_Transform.localPosition = finalLocalPosition;
    }
    
    
    private IEnumerator MoveAndStopAtTargetWithAnimation(int targetSymbolValue, SpinStopAnimation spinStopAnimation, float startSpeed)
    {
        var localPos = m_Transform.localPosition;
        var initialPos = localPos;
        var animDuration = GetDurationForStopAnimation(spinStopAnimation);
        var extraSpinCount = GetExtraSpinCount(startSpeed, animDuration);
        var travelDistanceToTargetSymbol = GetTravelDistanceToSymbol(targetSymbolValue, localPos);
        var totalTravelDistance = travelDistanceToTargetSymbol + (extraSpinCount * m_ColumnRepeatsSymbolsEvery);

        var timeSinceAnimationStarted = 0f;
        var normalizedAnimationTime = 0f;
        
        while (normalizedAnimationTime < 1)
        {
            timeSinceAnimationStarted += Time.deltaTime;
            normalizedAnimationTime = Mathf.Clamp01(timeSinceAnimationStarted / animDuration); 
            var yChangeSinceAnimationStarted = totalTravelDistance * m_Config.AnimationCurve.Evaluate(normalizedAnimationTime);
            localPos = initialPos + (Vector3.down * yChangeSinceAnimationStarted);
            localPos.y %= m_ColumnRepeatsSymbolsEvery;
            m_Transform.localPosition = localPos;
            yield return null;
        }
    }
}
