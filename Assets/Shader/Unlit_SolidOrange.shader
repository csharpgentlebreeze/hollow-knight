Shader "Custom/Unlit_SolidOrange"
{
     Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OrangeColor ("深橙色", Color) = (0.8,0.3,0,1) // 深橙#CC4D00
        _FadeFactor ("褪色系数(0=橙/1=原图)", Range(0,1)) = 0 // 核心控制系数
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha // 仅保留原图透明，无其他混合
        Cull Off ZWrite Off // Sprite通用设置

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _OrangeColor;
            fixed _FadeFactor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texCol = tex2D(_MainTex, i.uv) * i.color; // 原图颜色
                // 核心混合：FadeFactor=0→纯橙，FadeFactor=1→纯原图，无中间透明
                fixed3 finalRgb = lerp(_OrangeColor.rgb, texCol.rgb, _FadeFactor);
                return fixed4(finalRgb, texCol.a); // 始终用原图的Alpha，无透明变化
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}
