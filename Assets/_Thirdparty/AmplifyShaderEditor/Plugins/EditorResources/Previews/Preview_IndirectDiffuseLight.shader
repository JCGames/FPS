Shader "Hidden/IndirectDiffuseLight"
{
	Properties
	{
		_Intensity ("Intensity", Float) = 1
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Preview.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"

			float _Intensity;

			float4 frag(v2f_img i) : SV_Target
			{
				float3 vertexPos = PreviewFragmentPositionOS( i.uv );
				float3 worldPos = mul(unity_ObjectToWorld, float4(vertexPos,1)).xyz;
				float4 back = lerp(float4(0.4117,0.3843,0.3647,1),float4(0.4117,0.5059,0.6470,1),worldPos.y * 0.5 + 0.5);
				return float4(GammaToLinearSpace(back.rgb * _Intensity),1);
			}
			ENDCG
		}

		Pass // connected tangent
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Preview.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"

			float _Intensity;
			sampler2D _A;

			float4 frag(v2f_img i) : SV_Target
			{
				float3 vertexPos = PreviewFragmentPositionOS( i.uv );
				float3 normal = PreviewFragmentNormalOS( i.uv );
				float3 worldNormal = UnityObjectToWorldNormal(normal);

				float3 tangent = PreviewFragmentTangentOS( i.uv );
				float3 worldPos = mul(unity_ObjectToWorld, float4(vertexPos,1)).xyz;
				float3 worldTangent = UnityObjectToWorldDir(tangent);
				float tangentSign = -1;
				float3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
				float4 tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				float4 tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				float4 tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				float2 sphereUVs = i.uv;

				sphereUVs.x = (atan2(vertexPos.x, -vertexPos.z) / (UNITY_PI) + 0.5);
				// Needs further checking
				//float3 tangentNormal = tex2Dlod(_A, float4(sphereUVs,0,0)).xyz;
				float3 tangentNormal = tex2D(_A, sphereUVs).xyz;

				worldNormal = fixed3( dot( tSpace0.xyz, tangentNormal ), dot( tSpace1.xyz, tangentNormal ), dot( tSpace2.xyz, tangentNormal ) );

				float4 back = lerp(float4(0.4117,0.3843,0.3647,1),float4(0.4117,0.5059,0.6470,1),worldNormal.y * 0.5 + 0.5);

				return float4(GammaToLinearSpace(back.rgb * _Intensity),1);
			}
			ENDCG
		}

		Pass // connected world
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Preview.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"

			float _Intensity;
			sampler2D _A;

			float4 frag( v2f_img i ) : SV_Target
			{
				float3 vertexPos = PreviewFragmentPositionOS( i.uv );
				float3 normal = PreviewFragmentNormalOS( i.uv );
				float3 worldNormal = tex2D( _A, i.uv );

				float4 back = lerp( float4( 0.4117,0.3843,0.3647,1 ),float4( 0.4117,0.5059,0.6470,1 ),worldNormal.y * 0.5 + 0.5 );

				return float4( GammaToLinearSpace( back.rgb * _Intensity ),1 );
			}
			ENDCG
		}
	}
}
