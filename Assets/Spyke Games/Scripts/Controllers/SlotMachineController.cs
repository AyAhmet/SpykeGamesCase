using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class SlotMachineController : MonoBehaviour, ICollectCoin
{
    [SerializeField] private List<SlotMachineColumnController> ColumnControllers;
    [SerializeField] private float SpinDuration;
    [SerializeField] private float SpinSpeed;
    [Range(0.1f,0.2f)][SerializeField] private float MinColumnSpinDelay;
    [Range(0.1f,0.2f)][SerializeField] private float MaxColumnSpinDelay;
    [SerializeField] private AnimationCurve LastColumnAnimationCurve;

    public event Action<int> OnCoinCollected;
    
    private ISlotMachine m_SlotMachine;
    private IOutputsAndOdds m_OutputsAndOddsTable;
    private ISpinInitiator m_SpinInitiator;

    private bool m_IsSpinning;


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

    #endregion

    private void Awake()
    {
        InjectColumnConfigs();
    }

    private void InjectColumnConfigs()
    {
        var spinDelay = Random.Range(MinColumnSpinDelay, MaxColumnSpinDelay);
        
        for (var i = 0; i < ColumnControllers.Count; i++)
        {
            var animCurve = (i + 1) == ColumnControllers.Count ? LastColumnAnimationCurve : null;
            var config = new ColumnConfig(i, SpinDuration, SpinSpeed, spinDelay * i, animCurve);
            ColumnControllers[i].SetConfig(config);
        }
    }

    private void Spin()
    {
        if (m_IsSpinning) return;
        m_IsSpinning = true;

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
        foreach (var columnController in ColumnControllers)
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
            ColumnControllers[i].Spin(symbolSequence[i], stopAnimation);
        }

        do
        {
            yield return null;
        } while (IsAnyColumnRolling());

        m_IsSpinning = false;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        MinColumnSpinDelay = Mathf.Min(MinColumnSpinDelay, MaxColumnSpinDelay);
        MaxColumnSpinDelay = Mathf.Max(MinColumnSpinDelay, MaxColumnSpinDelay);
    }
#endif
}



