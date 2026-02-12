Shader "XSJ/VFX/FxStandard"
{
    Properties
    {
        [HDR]_Color ("基础颜色", Color) = (1,1,1,1)
        
        _MainTex ("基础贴图", 2D) = "white" {}
        _AlphaValue ("Alpha值", Range(0,10)) = 1
        _USpeed ("Uspeed", Float ) = 0
        _VSpeed ("Vspeed", Float ) = 0
        [HDR]_LerpColor ("LerpColor", Color) = (1,1,1,1)
        _LerpValue ("LerpValue",Range(0,1)) = 1

        [Toggle] _DiffuseMask ("基础遮罩",Float)=0

        [HideInInspector]_MaskTexPopUp("",Float) = 0
        [HideInInspector]_MaskTexRGBA("",Vector) = (1,1,1,1)
        
        _DiffuseMaskTex ("遮罩贴图", 2D) = "white" {}
        _Mask_USpeed ("Mask_USpeed", Float ) = 0
        _Mask_VSpeed ("Mask_VSpeed", Float ) = 0

        [HideInInspector] _Comp("Comp",Float) = 1
        [HideInInspector] _CompMode ("__Compmode",Float) = 1
		
        _ZTest("ZTest",Float) = 0  
        [HideInInspector] _ZTestMode ("__Zmode",Float) = 0.0 

        [HideInInspector] _BlendMode ("__mode",Float) = 0.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _FaceMode ("__face", Float) = 0.0
        [HideInInspector] _CullMode ("__cull", Float) = 2.0
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane" 
        }

        Pass
        {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha [_DstBlend]
            Cull [_CullMode]
            Lighting Off
            ZWrite Off
            ZTest [_ZTest]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #pragma multi_compile _ _DIFFUSEMASK_ON

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST,_MaskTexRGBA;
            uniform float4 _Color;
            uniform half _AlphaValue;
            uniform half _USpeed;
            uniform half _VSpeed;
            uniform float4 _LerpColor;
            uniform half _LerpValue;

            #if _DIFFUSEMASK_ON
            uniform sampler2D _DiffuseMaskTex;
            uniform half4 _DiffuseMaskTex_ST;
            uniform half _Mask_USpeed;
            uniform half _Mask_VSpeed;
            #endif

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                #if _DIFFUSEMASK_ON
                float2 uv1 :TEXCOORD1;
                #endif
            };

            v2f vert (appdata_t v)
            {
                v2f o;

                UNITY_INITIALIZE_OUTPUT(v2f, o);
        
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.texcoord += fixed2(_USpeed*_Time.y,_VSpeed*_Time.y);

                #if _DIFFUSEMASK_ON
                o.uv1 = TRANSFORM_TEX(v.texcoord,_DiffuseMaskTex);
                o.uv1 += fixed2(_Mask_USpeed*_Time.y,_Mask_VSpeed*_Time.y);
                #endif

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);

                #if _DIFFUSEMASK_ON
                fixed4 DiffuseMask = tex2D(_DiffuseMaskTex,i.uv1);
                fixed diffuseMaskDot = dot(DiffuseMask,_MaskTexRGBA);
                col.a = col.a * diffuseMaskDot;
                #endif

                col.rgb = col.rgb * _Color.rgb;
                fixed value = saturate(col.a);
                float lerpdegree = saturate(1 - value - _LerpValue);
                col.rgb = lerp(col.rgb,_LerpColor.rgb,lerpdegree) * i.color;
                col.a = clamp((col.a * _AlphaValue),0,1)*i.color.a;
                
                return col;
            }
            ENDCG
        }
    }
    CustomEditor "FxStandardGUI"
}
