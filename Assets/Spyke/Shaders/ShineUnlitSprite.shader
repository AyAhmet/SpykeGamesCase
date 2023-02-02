// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/ShineUnlitSprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        [Toggle] _Shine ("Shine", Float) = 1
        _ShineColor ("Shine Color", Color) = (1,1,1,1)
        _ShineSmoothness ("Shine Smoothness", Range(0, 1)) = 0.25
        _ShineThickness ("Shine Thickness", Range(0, 1)) = 0.25
        _ShineDelay ("Shine Delay", Range(0, 10)) = 2
        _ShineSpeed ("Shine Speed", Range(0, 10)) = 0.5
        _ShineRotation ("Shine Rotation", Range(0, 360)) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            struct vert_input
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct frag_input
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Shine)
            UNITY_DEFINE_INSTANCED_PROP(bool, _Shine)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _ShineColor)
            UNITY_DEFINE_INSTANCED_PROP(float, _ShineThickness)
            UNITY_DEFINE_INSTANCED_PROP(float, _ShineSmoothness)
            UNITY_DEFINE_INSTANCED_PROP(float, _ShineDelay)
            UNITY_DEFINE_INSTANCED_PROP(float, _ShineSpeed)
            UNITY_DEFINE_INSTANCED_PROP(float, _ShineRotation)
            UNITY_INSTANCING_BUFFER_END(Shine)

            inline float inv_lerp(float from, float to, float value)
            {
                return (value - from) / (to - from);
            }

            inline float remap(float orig_from, float orig_to, float target_from, float target_to, float value)
            {
                float rel = inv_lerp(orig_from, orig_to, value);
                return lerp(target_from, target_to, rel);
            }

            frag_input vert(vert_input IN)
            {
                frag_input o;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, o);

                o.vertex = UnityFlipSprite(IN.vertex, _Flip);
                o.vertex = UnityObjectToClipPos(o.vertex);
                o.texcoord = IN.texcoord;
                o.color = IN.color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                    o.vertex = UnityPixelSnap (o.vertex);
                #endif

                return o;
            }

            float2 rotateUV(float2 uv, float rotation, float2 mid)
            {
                return float2(
                    cos(rotation) * (uv.x - mid.x) + sin(rotation) * (uv.y - mid.y) + mid.x,
                    cos(rotation) * (uv.y - mid.y) - sin(rotation) * (uv.x - mid.x) + mid.y
                );
            }

            fixed4 frag(frag_input IN) : SV_Target
            {
                // Fuat hocam tamamen elle yazilmistir :))
                UNITY_SETUP_INSTANCE_ID(IN)

                fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;;
                c.rgb *= c.a;

                bool _shine = UNITY_ACCESS_INSTANCED_PROP(Shine, _Shine);

                if (_shine == false)
                    return c;

                float _shineDelay = UNITY_ACCESS_INSTANCED_PROP(Shine, _ShineDelay);
                float _shineSpeed = UNITY_ACCESS_INSTANCED_PROP(Shine, _ShineSpeed);
                float _shineRotation = UNITY_ACCESS_INSTANCED_PROP(Shine, _ShineRotation) * UNITY_PI / 180;

                IN.texcoord.xy = rotateUV(IN.texcoord.xy, -_shineRotation, float2(0.5, 0.5));

                float time = _Time[1];
                float shine_duration = 1 / _shineSpeed;

                float t = time % (_shineDelay + shine_duration);
                if (t > shine_duration)
                    return c;

                fixed4 _shineColor = UNITY_ACCESS_INSTANCED_PROP(Shine, _ShineColor);
                float _shineSmoothness = UNITY_ACCESS_INSTANCED_PROP(Shine, _ShineSmoothness);
                float _shineThickness = UNITY_ACCESS_INSTANCED_PROP(Shine, _ShineThickness);

                t *= _shineSpeed;
                t = remap(0, 1, -_shineThickness, 1 + _shineThickness, t);

                float x = IN.texcoord.x;
                float dif = abs(x - t);
                float shine = 1 - smoothstep(_shineThickness - _shineSmoothness, _shineThickness, dif);

                fixed4 shine_color = _shineColor;
                shine_color *= c.a;
                fixed4 c2 = ((1 - shine) * c) + (shine * shine_color);

                return lerp(c, c2, c2.a);
            }
            ENDCG
        }
    }
}