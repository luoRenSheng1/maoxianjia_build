Shader "FairyGUI/CloudEffect"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _CloudTex ("Cloud (A)", 2D) = "white" {}
        _Speed ("Speed", Float) = 1.0
        _Offset ("Offset", Float) = 0.0
        _Alpha ("Alpha", Range(0, 1)) = 1.0  // 新增透明度控制属性
        
        // 添加裁剪所需的Stencil属性
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off
        
        // 添加Stencil测试以实现裁剪
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CloudTex;
            float _Speed;
            float _Offset;
            float _Alpha;  // 新增透明度变量
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed colA = tex2D(_MainTex, i.uv).a;
                
                // 云层动画UV
                // float2 cloudUV = i.uv * 2.0 + float2(_Offset, _Offset * 0.7);

                // 基于时间修改UV坐标
                float2 scrollUV = i.uv;
                // scrollUV.x += _Time.y * 0.1f;//_Speed;  // 从上往下飘
                // scrollUV.y += _Time.y * 0.1f;//_Speed;

                // scrollUV.x += _Time.y * _Speed * 0.1;  // 从右往左飘
                scrollUV.x -= _Time.y * _Speed * 0.01;    // 从左往右飘
                // scrollUV.y -= _Time.y * _Speed * 0.1;  // 从下往上
                
                fixed4 cloud = tex2D(_CloudTex, scrollUV);

                
                // 添加第二层以不同速度移动的云
                float2 scrollUV2 = i.uv;
                scrollUV2.x += -_Time.y * _Speed * 0.05;  // 更慢的速度
                fixed4 cloud2 = tex2D(_CloudTex, scrollUV2);
                cloud = lerp(cloud, cloud2, 0.3);  // 混合两层云
                
                // 添加云层缩放参数
                float _CloudScale = 1.0;
                scrollUV = i.uv * _CloudScale;  // 缩放UV坐标
                
                // 确保云层无缝循环
                scrollUV.x = frac(scrollUV.x);  // 取小数部分实现无缝循环

                
                // 最终混合
                cloud.a = colA * _Alpha;
                
                // 确保透明度不超过1
                cloud.a = saturate(cloud.a);
                
                return cloud;
                
            }
            ENDCG
        }
    }
}
