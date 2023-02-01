using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SlotMachineSymbolController : MonoBehaviour
{
    [field : SerializeField] public SlotMachineSymbols Symbol { get; private set; }
    [SerializeField] private SpriteRenderer m_SpriteRenderer; 
    
    private Sprite m_SharpSprite;
    private Sprite m_BlurrySprite;

    #region Dependency Injection

    public void SetSharpAndBlurrySprites(Sprite sharp, Sprite blurry)
    {
        m_SharpSprite = sharp;
        m_BlurrySprite = blurry;
        
        EnableSharpSprite();
    }

    #endregion
    
    public void EnableSharpSprite()
    {
        m_SpriteRenderer.sprite = m_SharpSprite;
    }
    
    public void EnableBlurrySprite()
    {
        m_SpriteRenderer.sprite = m_BlurrySprite;
    }
}
