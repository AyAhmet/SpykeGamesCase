using System;
using Spyke.Scripts.Interfaces;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Spyke.Scripts.Controllers
{
    public class SpinButtonController : MonoBehaviour, ISpinInitiator
    {
        [SerializeField] private SpriteRenderer m_ButtonInnerPartRenderer;
        [SerializeField] private Transform m_ButtonInnerPartTransform;
        [SerializeField] private Color m_ButtonDownColor = Color.white;
        [SerializeField] private Color m_ButtonUpColor= Color.white;
        
        public event Action OnSpinInitiated;
    
        private ISpin m_EventReceiver;
    
        #region Dependency Injection
    
        public void SetEventReceiver(ISpin eventReceiver)
        {
            m_EventReceiver = eventReceiver;
        }
    
        #endregion
        
        private void OnMouseDown()
        {
            if (m_EventReceiver.IsSpinning()) return;
            
            m_ButtonInnerPartRenderer.color = m_ButtonDownColor;
            m_ButtonInnerPartTransform.localScale = new Vector3(1, 0.9f, 1);
            OnSpinInitiated?.Invoke();
        }
    
        private void OnMouseUp()
        {
            m_ButtonInnerPartRenderer.color = m_ButtonUpColor;
            m_ButtonInnerPartTransform.localScale = Vector3.one;
        }
    }
}

