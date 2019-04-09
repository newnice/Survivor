Shader "Hidden/OverlayScreen"
{
    Properties
    {
        _MainTex ("Screen Texture", 2D) = "white" {}
        _OverlayTexture ("OverlayTexture", 2D) = "black" {}
        _Color("Color", Color)=(1.0, 0.0, 0.0, 1.0)
        _Alpha("Alpha", Range (0,1))=1.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _OverlayTexture;
            sampler2D _MainTex;
            fixed4 _Color;
            float _Alpha;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 overlay = tex2D(_OverlayTexture, i.uv);
                fixed4 tex_screen = tex2D(_MainTex, i.uv);
                overlay =  (1 - overlay.rgba)*_Color;
                return fixed4(overlay.rgb*_Alpha, _Alpha)+ tex_screen.rgba;
            }
            ENDCG
        }
    }
}
