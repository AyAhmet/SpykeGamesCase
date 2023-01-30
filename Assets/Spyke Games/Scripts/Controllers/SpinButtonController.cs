using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SpinButtonController : MonoBehaviour, ISpinInitiator
{
    [SerializeField] private SpriteRenderer ButtonInnerPartRenderer;
    [SerializeField] private Transform ButtonInnerPartTransform;
    [SerializeField] private Color ButtonDownColor = Color.white;
    [SerializeField] private Color ButtonUpColor= Color.white;
    
    public event Action OnSpinInitiated;
    
    private void OnMouseDown()
    {
        ButtonInnerPartRenderer.color = ButtonDownColor;
        ButtonInnerPartTransform.localScale = new Vector3(1, 0.9f, 1);
        OnSpinInitiated?.Invoke();
    }

    private void OnMouseUp()
    {
        ButtonInnerPartRenderer.color = ButtonUpColor;
        ButtonInnerPartTransform.localScale = Vector3.one;
    }
}
