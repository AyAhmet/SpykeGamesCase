using System.Collections.Generic;
using UnityEngine;
using Spyke.Scripts.Controllers;
using Spyke.Scripts.Interfaces;
using Spyke.Scripts.Modules.ChanceDistributor;
using Spyke.Scripts.Modules.PersistentDataStorage;
using Spyke.Scripts.Modules.SlotMachine;
using Spyke.Scripts.ScriptableObjects;

namespace Spyke.Scripts.Managers
{
    public class DependencyManager : MonoBehaviour
    {
        [Header("Controllers")]
        [SerializeField] private SlotMachineController m_SlotMachineController;
        [SerializeField] private CoinParticleController m_CoinParticleController;
        [SerializeField] private SpinButtonController m_SpinButtonController;
 
        [Header("Slot Machine Controller Dependencies")]
        [SerializeField] private SlotMachineOutputsAndOddsTable m_SlotMachineOutputsAndOddsTable;
 
        [Header("Slot Machine Column Controller Dependencies")] 
        [SerializeField] private List<Sprite> m_SharpSprites;
        [SerializeField] private List<Sprite> m_BlurrySprites;
 
        private IPersistData PersistentDataManager;
     
     
        private void Start()
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
            if (m_SlotMachineController == null) return;
         
            IOutputChance chanceOutputter = new PeriodicChanceDistributor(m_SlotMachineOutputsAndOddsTable.GetOutputsAndOddsAsDictionary());
            ISlotMachine slotMachine = new SlotMachine(chanceOutputter, PersistentDataManager);
 
            m_SlotMachineController.SetSlotMachine(slotMachine);
            m_SlotMachineController.SetOutputsAndOddsTable(m_SlotMachineOutputsAndOddsTable);
            m_SlotMachineController.SetSpinInitiator(m_SpinButtonController);
            m_SlotMachineController.SetSymbolSprites(m_SharpSprites, m_BlurrySprites);
        }
 
        private void InjectCoinParticleControllerDependencies()
        {
            m_CoinParticleController.Subscribe(m_SlotMachineController);
        }
 
        private void InjectSpinButtonControllerDependencies()
        {
            m_SpinButtonController.SetEventReceiver(m_SlotMachineController);
        }
 
        #endregion
    }
    
}