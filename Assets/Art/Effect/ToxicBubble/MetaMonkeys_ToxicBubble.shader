// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MetaMonkeys/ToxicBubble"
{
	Properties
	{
		[HDR]_maincolor("main color", Color) = (1,1,1,0)
		[HDR]_edgecolor("edge color", Color) = (0,0,0,0)
		[NoScaleOffset]_dissolvetexture("dissolve texture", 2D) = "white" {}
		_dissolvetilling("dissolve tilling", Vector) = (1,1,0,0)
		_vertexoffset("vertex offset", Float) = 1
		_noisescale("noise scale", Float) = 0
		_noisespeed("noise speed", Float) = 0
		_edge("edge", Range( 0 , 1)) = 0
		_noiseopacity("noise opacity", Range( 0 , 1)) = 0
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float4 uv_tex4coord;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform half _noisespeed;
		uniform half _noisescale;
		uniform half _vertexoffset;
		uniform half _noiseopacity;
		uniform half4 _maincolor;
		uniform sampler2D _dissolvetexture;
		uniform half2 _dissolvetilling;
		uniform half _edge;
		uniform half4 _edgecolor;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			half3 ase_vertexNormal = v.normal.xyz;
			float3 ase_vertex3Pos = v.vertex.xyz;
			half mulTime34 = _Time.y * ( _noisespeed * 0.01 );
			half2 appendResult45 = (half2(0.0 , mulTime34));
			half simplePerlin3D49 = snoise( ( ( ase_vertex3Pos + half3( appendResult45 ,  0.0 ) ) * _noisescale ) );
			simplePerlin3D49 = simplePerlin3D49*0.5 + 0.5;
			v.vertex.xyz += ( ase_vertexNormal * 0.1 * ( simplePerlin3D49 * _vertexoffset ) );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			half mulTime34 = _Time.y * ( _noisespeed * 0.01 );
			half2 appendResult45 = (half2(0.0 , mulTime34));
			half simplePerlin3D49 = snoise( ( ( ase_vertex3Pos + half3( appendResult45 ,  0.0 ) ) * _noisescale ) );
			simplePerlin3D49 = simplePerlin3D49*0.5 + 0.5;
			half temp_output_53_0 = (_noiseopacity + (simplePerlin3D49 - 0.0) * (1.0 - _noiseopacity) / (1.0 - 0.0));
			half temp_output_12_0_g1 = ( 1.0 - i.uv_tex4coord.w );
			half2 temp_cast_1 = (i.uv_tex4coord.z).xx;
			float2 uv_TexCoord15 = i.uv_texcoord * _dissolvetilling + temp_cast_1;
			half2 panner17 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_TexCoord15);
			half temp_output_1_0_g1 = tex2D( _dissolvetexture, panner17 ).r;
			half temp_output_3_0_g2 = ( temp_output_12_0_g1 - temp_output_1_0_g1 );
			half temp_output_8_0_g1 = saturate( ( temp_output_3_0_g2 / fwidth( temp_output_3_0_g2 ) ) );
			half temp_output_3_0_g3 = ( temp_output_12_0_g1 - ( temp_output_1_0_g1 + _edge ) );
			o.Emission = ( ( temp_output_53_0 * _maincolor ) + ( ( temp_output_8_0_g1 - saturate( ( temp_output_3_0_g3 / fwidth( temp_output_3_0_g3 ) ) ) ) * _edgecolor ) ).rgb;
			o.Alpha = ( i.vertexColor.a * temp_output_8_0_g1 * temp_output_53_0 );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float2 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xyzw = customInputData.uv_tex4coord;
				o.customPack1.xyzw = v.texcoord;
				o.customPack2.xy = customInputData.uv_texcoord;
				o.customPack2.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_tex4coord = IN.customPack1.xyzw;
				surfIN.uv_texcoord = IN.customPack2.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.vertexColor = IN.color;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
0;73;1203;550;2828.576;-345.4178;3.025531;True;False
Node;AmplifyShaderEditor.CommentaryNode;58;-1775.773,267.24;Inherit;False;1847.009;459.0361;Comment;12;35;38;34;45;32;33;37;36;54;43;49;53;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-1725.773,557.1959;Inherit;False;Property;_noisespeed;noise speed;7;0;Create;True;0;0;0;False;0;False;0;1.13;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-1551.46,549.4488;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;57;-1912.606,956.3051;Inherit;False;1135.564;508.7126;Comment;6;18;14;15;17;13;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;34;-1401.261,546.16;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;32;-1292.74,317.2399;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;45;-1200.05,580.5995;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;14;-1843.858,1020.352;Inherit;False;Property;_dissolvetilling;dissolve tilling;3;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexCoordVertexDataNode;18;-1862.606,1158;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-860.636,389.5159;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-897.4496,592.6158;Inherit;False;Property;_noisescale;noise scale;6;0;Create;True;0;0;0;False;0;False;0;34.77;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;55;-660.717,1080.131;Inherit;False;1259.428;552.4575;Comment;6;28;26;24;19;22;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-1577.423,1006.305;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexCoordVertexDataNode;28;-610.717,1249.068;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-679.9541,466.0345;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;17;-1302.423,1085.305;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;13;-1097.042,1235.018;Inherit;True;Property;_dissolvetexture;dissolve texture;2;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;8802c1fac4f7fed4084200c2cd7c7c25;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;54;-488.8248,610.2763;Inherit;False;Property;_noiseopacity;noise opacity;9;0;Create;True;0;0;0;False;0;False;0;0.12;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;26;-121.412,1130.131;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-297.2796,1241.72;Inherit;False;Property;_edge;edge;8;0;Create;True;0;0;0;False;0;False;0;0.025;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;60;271.6177,-65.37626;Inherit;False;676.7094;304;Comment;3;11;20;12;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;59;158.2915,601.4755;Inherit;False;704.2198;445.1642;Comment;5;31;29;42;56;7;;1,1,1,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;49;-517.3135,392.8481;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;12;321.6177,4.423759;Inherit;False;Property;_maincolor;main color;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;0.04705882,0.7490196,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;53;-218.7635,449.021;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;19;99.70995,1136.777;Inherit;True;F_dissolve_edge;-1;;1;a1dc6ace39b4a794c93a3c3dad34faf0;0;3;1;FLOAT;0;False;12;FLOAT;0;False;15;FLOAT;0;False;3;FLOAT;0;FLOAT;16;FLOAT;13
Node;AmplifyShaderEditor.ColorNode;22;115.069,1420.588;Inherit;False;Property;_edgecolor;edge color;1;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;1.648686,9.667048,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;208.2915,888.6678;Inherit;False;Property;_vertexoffset;vertex offset;5;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;61;267.4092,276.2045;Inherit;False;454.0705;257;Comment;2;27;6;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;27;317.4092,326.2045;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;363.7114,1170.927;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;31;470.1935,651.4755;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;42;689.5113,856.4366;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;521.9193,-15.37626;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;455.1103,792.6396;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;559.4797,369.2587;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;674.2811,737.6918;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;-1213.043,463.7567;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;16;-1576.423,1172.305;Inherit;False;Property;_dissolvepanning;dissolve panning;4;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;20;796.3271,38.70754;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;981.1396,15.36294;Half;False;True;-1;6;ASEMaterialInspector;0;0;Standard;MetaMonkeys/ToxicBubble;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;50;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;38;0;35;0
WireConnection;34;0;38;0
WireConnection;45;1;34;0
WireConnection;33;0;32;0
WireConnection;33;1;45;0
WireConnection;15;0;14;0
WireConnection;15;1;18;3
WireConnection;36;0;33;0
WireConnection;36;1;37;0
WireConnection;17;0;15;0
WireConnection;13;1;17;0
WireConnection;26;0;28;4
WireConnection;49;0;36;0
WireConnection;53;0;49;0
WireConnection;53;3;54;0
WireConnection;19;1;13;1
WireConnection;19;12;26;0
WireConnection;19;15;24;0
WireConnection;21;0;19;13
WireConnection;21;1;22;0
WireConnection;11;0;53;0
WireConnection;11;1;12;0
WireConnection;56;0;49;0
WireConnection;56;1;7;0
WireConnection;6;0;27;4
WireConnection;6;1;19;16
WireConnection;6;2;53;0
WireConnection;29;0;31;0
WireConnection;29;1;42;0
WireConnection;29;2;56;0
WireConnection;43;0;34;0
WireConnection;20;0;11;0
WireConnection;20;1;21;0
WireConnection;0;2;20;0
WireConnection;0;9;6;0
WireConnection;0;11;29;0
ASEEND*/
//CHKSM=256B61ECE8824D72688BE3AA94420127580B7016