using Spyke.Scripts.Utilities;
using Spyke.Scripts.Interfaces;
using UnityEngine;

namespace Spyke.Scripts.Modules.SlotMachine
{
    public class SlotMachine : ISlotMachine
    {
        private IOutputChance m_ChanceOutputter;
        private IPersistData m_PersistentDataManager;
    
        private SlotMachineOutputs PreCalculatedOutputsAsRowIndexes;
 
    
        public SlotMachine(IOutputChance chanceOutputter, IPersistData persistentDataManager)
        {
            m_ChanceOutputter = chanceOutputter;
            m_PersistentDataManager = persistentDataManager;
        
            GetOutputsFromSave();
        
            if (PreCalculatedOutputsAsRowIndexes == null)
                GenerateNewRowIndexes();
        }


        public int GetNextRowIndex()
        {
            if (PreCalculatedOutputsAsRowIndexes.CanGetNext() == false)
                GenerateNewRowIndexes();
        
            return PreCalculatedOutputsAsRowIndexes.GetNext();
        }


        public void SaveCurrentOutputsToDisk()
        {
            m_PersistentDataManager.SaveJson(Constants.SLOT_MACHINE_OUTPUT_SAVE_KEY, PreCalculatedOutputsAsRowIndexes);
        }


        private void GenerateNewRowIndexes()
        {
            var outputs = m_ChanceOutputter.GetNewOutputs();
            PreCalculatedOutputsAsRowIndexes = new SlotMachineOutputs(outputs);
        
            Debug.Log($"{PreCalculatedOutputsAsRowIndexes.CurrentIndex}");
        }
    
    
        private void GetOutputsFromSave()
        {
            PreCalculatedOutputsAsRowIndexes = m_PersistentDataManager.LoadJson<SlotMachineOutputs>(Constants.SLOT_MACHINE_OUTPUT_SAVE_KEY);
        }
    }
}