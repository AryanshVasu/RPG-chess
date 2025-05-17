Shader "Unlit/boardClock"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _Progress ("progess", float) = 0
        _Col1 ("ForColor", COLOR)  = (1,1,1,1)
        _Col2 ("BackColor", COLOR) = (0,0,0,0)
        _Roundness ("RoundEdges", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            float _Progress, _Roundness;
            float4 _Col1, _Col2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float mask = (i.uv.x < _Progress);   

                // float3 scale = float3(
                //     length(unity_ObjectToWorld._m00_m10_m20),
                //     length(unity_ObjectToWorld._m01_m11_m21),
                //     length(unity_ObjectToWorld._m02_m12_m22)
                // ); 

                // float2 sdfuv = float2(abs(i.uv.x - 0.5),abs(i.uv.y - 0.5));
                // sdfuv = float2(sdfuv.x*scale.x, sdfuv.y*scale.y);
                // // sdfuv *=100;
                // sdfuv -= float2(0.5*scale.x-_Roundness, 0.5*scale.y-_Roundness);
                // sdfuv = saturate(sdfuv);
                // float sdf = length(sdfuv);
                // sdf = sdf<_Roundness; 

                // // return float4(sdfuv,0,1);
                // return float4(sdf,sdf,sdf,1);

                return lerp(_Col2,_Col1, mask);
            }
            ENDCG
        }
    }
}
