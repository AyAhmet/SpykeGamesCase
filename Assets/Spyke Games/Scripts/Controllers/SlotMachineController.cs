using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class SlotMachineController : MonoBehaviour, ICollectCoin, ISpin
{
    [SerializeField] private List<SlotMachineColumnController> m_ColumnControllers;
    [SerializeField] private float m_SpinDuration = 2;
    [SerializeField] private float m_SpinSpeed = 100;
    [Range(0.1f,0.2f)][SerializeField] private float m_MinColumnSpinDelay = 0.1f;
    [Range(0.1f,0.2f)][SerializeField] private float m_MaxColumnSpinDelay = 0.2f;
    [SerializeField] private AnimationCurve m_LastColumnAnimationCurve;

    public event Action OnSpinStarted;
    public event Action OnSpinEnded;
    public event Action<int> OnCoinCollected;
    
    private ISlotMachine m_SlotMachine;
    private IOutputsAndOdds m_OutputsAndOddsTable;
    private ISpinInitiator m_SpinInitiator;
    
    private bool m_IsSpinning;

    public bool IsSpinning() => m_IsSpinning;


    #region Dependency Injection

    public void SetSlotMachine(ISlotMachine slotMachine)
    {
        m_SlotMachine = slotMachine;
    }

    public void SetOutputsAndOddsTable(IOutputsAndOdds outputsAndOddsTable)
    {
        m_OutputsAndOddsTable = outputsAndOddsTable;
    }

    public void SetSpinInitiator(ISpinInitiator spinInitiator)
    {
        m_SpinInitiator = spinInitiator;
        m_SpinInitiator.OnSpinInitiated += Spin;
    }

    public void SetSymbolSprites(List<Sprite> sharpSprites, List<Sprite> blurrySprites)
    {
        foreach (var columnController in m_ColumnControllers)
        {
            columnController.SetSymbolControllerSprites(sharpSprites, blurrySprites);
        }
    }

    #endregion
    
    private void Awake()
    {
        InjectColumnConfigs();
    }

    private void InjectColumnConfigs()
    {
        var spinDelay = Random.Range(m_MinColumnSpinDelay, m_MaxColumnSpinDelay);
        
        for (var i = 0; i < m_ColumnControllers.Count; i++)
        {
            var animCurve = (i + 1) == m_ColumnControllers.Count ? m_LastColumnAnimationCurve : null;
            var config = new ColumnConfig(m_SpinDuration, m_SpinSpeed, spinDelay * i, animCurve);
            m_ColumnControllers[i].SetConfig(config);
        }
    }

    private void Spin()
    {
        if (m_IsSpinning) return;
        m_IsSpinning = true;
        OnSpinStarted?.Invoke();

        var rowIndex = m_SlotMachine.GetNextRowIndex();

        StartCoroutine(SpinSequence(rowIndex));
    }

    private IEnumerator SpinSequence(int rowIndex)
    {
        yield return SpinCoroutine(m_OutputsAndOddsTable.GetSymbolSequenceAtRowIndex(rowIndex));

        var coinsCollected = m_OutputsAndOddsTable.GetCoinRewardAtRowIndex(rowIndex); 
        if (coinsCollected > 0)
            OnCoinCollected?.Invoke(coinsCollected);
    }
    
    
    // TODO: Check the behaviour of this and OnApplicationQuit functions on target OSs.
    private void OnApplicationQuit()
    {
        m_SlotMachine.SaveCurrentOutputsToDisk();
    }


    private SpinStopAnimation GetLastColumnStopAnimation(int[] output)
    {
        // Comparing first symbol against the rest, excluding the last symbol.
        for (var i = 1; i < output.Length - 1; i++)
        {
            if (output[i] == output[0]) continue;

            return SpinStopAnimation.Fast; // Returning early if the sequence is broken 
        }

        // If the last symbol is ALSO equals to the first symbol, we return slow. Otherwise, normal.
        return output[0] == output[^1] ? SpinStopAnimation.Slow : SpinStopAnimation.Normal;
    }
    

    private bool IsAnyColumnRolling()
    {
        foreach (var columnController in m_ColumnControllers)
        {
            if (columnController.IsRolling)
                return true;
        }

        return false;
    }
    
    
    private IEnumerator SpinCoroutine(int[] symbolSequence)
    {
        var lastColumnStopAnimation = GetLastColumnStopAnimation(symbolSequence);
        
        for (var i = 0; i < symbolSequence.Length; i++)
        {
            var stopAnimation = (i + 1) == symbolSequence.Length ? lastColumnStopAnimation : SpinStopAnimation.Fast;
            m_ColumnControllers[i].Spin(symbolSequence[i], stopAnimation);
        }

        do
        {
            yield return null;
        } while (IsAnyColumnRolling());

        m_IsSpinning = false;
        OnSpinEnded?.Invoke();
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        m_MinColumnSpinDelay = Mathf.Min(m_MinColumnSpinDelay, m_MaxColumnSpinDelay);
        m_MaxColumnSpinDelay = Mathf.Max(m_MinColumnSpinDelay, m_MaxColumnSpinDelay);
    }
#endif
}



