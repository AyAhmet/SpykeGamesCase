using UnityEngine;

public class DependencyManager : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private SlotMachineController SlotMachineController;
    [SerializeField] private CoinParticleController CoinParticleController;
    [SerializeField] private SpinButtonController SpinButtonController;
    
    [Header("Slot Machine Controller Dependencies")]
    [SerializeField] private OutputsAndOddsTable SlotMachineOutputsAndOddsTable;

    private IPersistData PersistentDataManager;
    
    
    private void Awake()
    {
        CreateDependencies();
        InjectDependencies();
    }


    private void CreateDependencies()
    {
        CreatePersistentDataManager();
    }


    private void InjectDependencies()
    {
        InjectSlotMachineControllerDependencies();
        InjectCoinParticleControllerDependencies();
        InjectSpinButtonControllerDependencies();
    }


    #region Creation

    private void CreatePersistentDataManager()
    {
        PersistentDataManager = new PlayerPrefsStorage();
    }

    #endregion


    #region Injection

    private void InjectSlotMachineControllerDependencies()
    {
        if (SlotMachineController == null) return;
        
        IOutputChance chanceOutputter = new PeriodicChanceDistributor(SlotMachineOutputsAndOddsTable);
        ISlotMachine slotMachine = new SlotMachine(chanceOutputter, PersistentDataManager);

        SlotMachineController.SetSlotMachine(slotMachine);
        SlotMachineController.SetOutputsAndOddsTable(SlotMachineOutputsAndOddsTable);
        SlotMachineController.SetSpinInitiator(SpinButtonController);
    }

    private void InjectCoinParticleControllerDependencies()
    {
        CoinParticleController.Subscribe(SlotMachineController);
    }

    private void InjectSpinButtonControllerDependencies()
    {
        SpinButtonController.SetEventReceiver(SlotMachineController);
    }

    #endregion
}
