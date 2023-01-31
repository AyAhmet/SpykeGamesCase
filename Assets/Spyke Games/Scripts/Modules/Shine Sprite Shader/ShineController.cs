using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class ShineController : MonoBehaviour
{
    [SerializeField] private bool Active = true;
    [SerializeField] private Color Color = new Color(0.5f, 0.5f, 0.5f, 1);
    [Range(0, 1)][SerializeField] private float Thickness = 0.25f;
    [Range(0, 1)][SerializeField] private float Smoothness = 0.25f;
    [Range(0, 10)][SerializeField] private float Delay = 2;
    [Range(0, 10)][SerializeField] private float Speed = 0.5f;

    private SpriteRenderer m_SpriteRenderer;
    private MaterialPropertyBlock m_MaterialPropertyBlock;
    
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int Shine = Shader.PropertyToID("_Shine");
    private static readonly int ShineColor = Shader.PropertyToID("_ShineColor");
    private static readonly int ShineThickness = Shader.PropertyToID("_ShineThickness");
    private static readonly int ShineSmoothness = Shader.PropertyToID("_ShineSmoothness");
    private static readonly int ShineDelay = Shader.PropertyToID("_ShineDelay");
    private static readonly int ShineSpeed = Shader.PropertyToID("_ShineSpeed");

    private void Awake()
    {
        SetPropertyBlock();
    }

    private void SetPropertyBlock()
    {
        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        
        if (m_MaterialPropertyBlock == null) 
            m_MaterialPropertyBlock = new MaterialPropertyBlock();

        m_MaterialPropertyBlock.SetTexture(MainTex, m_SpriteRenderer.sprite.texture);
        m_MaterialPropertyBlock.SetFloat(Shine, Active ? 1 : 0);
        m_MaterialPropertyBlock.SetColor(ShineColor, Color);
        m_MaterialPropertyBlock.SetFloat(ShineThickness, Thickness);
        m_MaterialPropertyBlock.SetFloat(ShineSmoothness, Smoothness);
        m_MaterialPropertyBlock.SetFloat(ShineDelay, Delay);
        m_MaterialPropertyBlock.SetFloat(ShineSpeed, Speed);
        m_SpriteRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
    }

#if UNITY_EDITOR
    private const string SHADER_NAME = "Custom/ShineUnlitSprite";
    private Shader m_Shader;

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        if (LogWarningIfNotCompatibleShader())
            return;
        SetPropertyBlock();
    }
    
    private bool LogWarningIfNotCompatibleShader()
    {
        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        
        if (m_Shader == null)
            m_Shader = Shader.Find(SHADER_NAME);

        if (m_SpriteRenderer.sharedMaterial.shader != m_Shader)
        {
            Debug.LogWarning($"Material on GameObject '{transform.name}' is not using correct shader. Please use {SHADER_NAME}");
            return true;
        }

        return false;
    }
#endif
}
