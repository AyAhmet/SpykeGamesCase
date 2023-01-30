using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlotMachineSymbolController : MonoBehaviour
{
    [field : SerializeField] public SlotMachineSymbols Symbol { get; private set; }
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private List<Sprite> SharpSprites;
    [SerializeField] private List<Sprite> BlurrySprites;

    private Sprite m_SharpSprite;
    private Sprite m_BlurrySprite;

    private void Awake()
    {
        m_SharpSprite = SharpSprites[(int) Symbol];
        m_BlurrySprite = BlurrySprites[(int) Symbol];
    }


    public void SetSharpSprite()
    {
        SpriteRenderer.sprite = m_SharpSprite;
    }


    public void SetBlurrySprite()
    {
        SpriteRenderer.sprite = m_BlurrySprite;
    }
    
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        
        gameObject.name = Symbol.ToString();
        SpriteRenderer.sprite = SharpSprites[(int) Symbol];
    }

#endif
}
