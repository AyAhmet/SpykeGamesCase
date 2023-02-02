using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer))]
public class ShineController : MonoBehaviour
{
    [SerializeField] private bool m_Active = true;
    [SerializeField] private Color m_Color = new Color(1, 1, 1, 0.5f);
    [Range(0, 1)][SerializeField] private float m_Thickness = 0.25f;
    [Range(0, 1)][SerializeField] private float m_Smoothness = 0.25f;
    [Range(0, 10)][SerializeField] private float m_Delay = 2;
    [Range(0, 10)][SerializeField] private float m_Speed = 0.5f;
    [Range(0, 360)] [SerializeField] private float m_Rotation = 45;

    private SpriteRenderer m_SpriteRenderer;
    private MaterialPropertyBlock m_MaterialPropertyBlock;
    
    private static readonly int m_MainTexID = Shader.PropertyToID("_MainTex");
    private static readonly int m_ShineID = Shader.PropertyToID("_Shine");
    private static readonly int m_ShineColorID = Shader.PropertyToID("_ShineColor");
    private static readonly int m_ShineThicknessID = Shader.PropertyToID("_ShineThickness");
    private static readonly int m_ShineSmoothnessID = Shader.PropertyToID("_ShineSmoothness");
    private static readonly int m_ShineDelayID = Shader.PropertyToID("_ShineDelay");
    private static readonly int m_ShineSpeedID = Shader.PropertyToID("_ShineSpeed");
    private static readonly int m_ShineRotationID = Shader.PropertyToID("_ShineRotation");

    private void Awake()
    {
        SetPropertyBlock();
    }

    private void SetPropertyBlock()
    {
        if (m_Active == false) return;
        
        if (m_SpriteRenderer == null)
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        
        if (m_MaterialPropertyBlock == null) 
            m_MaterialPropertyBlock = new MaterialPropertyBlock();

        m_MaterialPropertyBlock.SetTexture(m_MainTexID, m_SpriteRenderer.sprite.texture);
        m_MaterialPropertyBlock.SetFloat(m_ShineID, m_Active ? 1 : 0);
        m_MaterialPropertyBlock.SetColor(m_ShineColorID, m_Color);
        m_MaterialPropertyBlock.SetFloat(m_ShineThicknessID, m_Thickness);
        m_MaterialPropertyBlock.SetFloat(m_ShineSmoothnessID, m_Smoothness);
        m_MaterialPropertyBlock.SetFloat(m_ShineDelayID, m_Delay);
        m_MaterialPropertyBlock.SetFloat(m_ShineSpeedID, m_Speed);
        m_MaterialPropertyBlock.SetFloat(m_ShineRotationID, m_Rotation);
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
