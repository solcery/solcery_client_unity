Shader "UI/Grayscale"
{
	Properties
	{
        _MainTex ("Texture", 2D) = "white" {}
		_Grayscale ("Grayscale", float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};
			
			sampler2D _MainTex;
			uniform float _Grayscale;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
				float3 brtColor = color.rgb;
				if (_Grayscale == true)
				{
					color.rgb = lerp(brtColor, dot(brtColor, float3(0.3, 0.6, 0.1)), 1);
				}
				return color;
			}
			ENDCG
		}
	}
}