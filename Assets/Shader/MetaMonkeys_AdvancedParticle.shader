// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MetaMonkeys/AdvancedParticle"
{
	Properties
	{
		[Header(Additional)][Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 2
		_depthfade("depth fade", Float) = 1
		[Toggle]_animblend("animblend", Float) = 0
		[HDR]_brightness("brightness", Color) = (1,1,1,0)
		[Header(Distortion)][NoScaleOffset]_distortiontexture("distortion texture", 2D) = "white" {}
		[Toggle]_distortionpolar("distortion polar", Float) = 0
		_distortiontilling("distortion tilling", Vector) = (1,1,0,0)
		_distortionpanning("distortion panning", Vector) = (0,0,0,0)
		[Header(Mask)][NoScaleOffset]_masktexture("mask texture", 2D) = "white" {}
		[Toggle(_STEP_ON)] _step("step", Float) = 0
		_maskstep("mask step", Range( 0 , 1)) = 0
		_maskdistortion("mask distortion", Range( 0 , 1)) = 0
		[Toggle]_maskpolar("mask polar", Float) = 0
		_masktilling("mask tilling", Vector) = (1,1,0,0)
		_maskpanning("mask panning", Vector) = (0,0,0,0)
		[Header(Noise)][NoScaleOffset]_noisetexture("noise texture", 2D) = "white" {}
		[Toggle]_noisepolar("noise polar", Float) = 0
		_noiseintensity("noise intensity", Float) = 1
		_noisedistortion("noise distortion", Range( 0 , 1)) = 0
		_noisetilling("noise tilling", Vector) = (1,1,0,0)
		_noisepanning("noise panning", Vector) = (0,0,0,0)
		[Header(Dissolve)][NoScaleOffset]_dissolvetexture("dissolve texture", 2D) = "black" {}
		[Toggle(_USECVS_ON)] _usecvs("use cvs?", Float) = 0
		[HDR]_edgecolor("edge color", Color) = (1,1,1,0)
		[Toggle]_dissolvepolar("dissolve polar", Float) = 0
		_dissolve("dissolve", Range( 0 , 1)) = 0
		_dissolvepanning("dissolve panning", Vector) = (0,0,0,0)
		_dissolvetilling("dissolve tilling", Vector) = (1,1,0,0)
		_dissolvedistortion("dissolve distortion", Range( 0 , 1)) = 0
		_dissolveedge("dissolve edge", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.0
		#pragma shader_feature_local _USECVS_ON
		#pragma shader_feature_local _STEP_ON
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 uv_tex4coord;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
			float2 uv2_texcoord2;
			float4 screenPos;
		};

		uniform float _CullMode;
		uniform float _dissolve;
		uniform sampler2D _dissolvetexture;
		uniform float2 _dissolvepanning;
		uniform float2 _dissolvetilling;
		uniform float _dissolvepolar;
		uniform sampler2D _distortiontexture;
		uniform float2 _distortiontilling;
		uniform float _distortionpolar;
		uniform float2 _distortionpanning;
		uniform float _dissolvedistortion;
		uniform float _dissolveedge;
		uniform float4 _edgecolor;
		uniform sampler2D _noisetexture;
		uniform float2 _noisepanning;
		uniform float2 _noisetilling;
		uniform float _noisepolar;
		uniform float _noisedistortion;
		uniform float _noiseintensity;
		uniform float4 _brightness;
		uniform float _maskstep;
		uniform sampler2D _masktexture;
		uniform float2 _maskpanning;
		uniform float2 _masktilling;
		uniform float _maskpolar;
		uniform float _maskdistortion;
		uniform float _animblend;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _depthfade;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			#ifdef _USECVS_ON
				float staticSwitch141 = i.uv_tex4coord.z;
			#else
				float staticSwitch141 = ( 1.0 - _dissolve );
			#endif
			float temp_output_12_0_g33 = staticSwitch141;
			float2 uv_TexCoord60 = i.uv_texcoord * _dissolvetilling;
			float2 CenteredUV15_g31 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g31 = CenteredUV15_g31;
			float2 appendResult23_g31 = (float2(( length( CenteredUV15_g31 ) * _dissolvetilling.x * 2.0 ) , ( atan2( break17_g31.x , break17_g31.y ) * ( 1.0 / 6.28318548202515 ) * _dissolvetilling.y )));
			float2 lerpResult61 = lerp( uv_TexCoord60 , appendResult23_g31 , _dissolvepolar);
			float2 uv_TexCoord36 = i.uv_texcoord * _distortiontilling;
			float2 CenteredUV15_g8 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g8 = CenteredUV15_g8;
			float2 appendResult23_g8 = (float2(( length( CenteredUV15_g8 ) * _distortiontilling.x * 2.0 ) , ( atan2( break17_g8.x , break17_g8.y ) * ( 1.0 / 6.28318548202515 ) * _distortiontilling.y )));
			float2 lerpResult57 = lerp( uv_TexCoord36 , appendResult23_g8 , _distortionpolar);
			float distortionTex75 = tex2D( _distortiontexture, ( lerpResult57 + ( _distortionpanning * _Time.y ) ) ).r;
			float2 temp_cast_0 = (distortionTex75).xx;
			float2 lerpResult74 = lerp( lerpResult61 , temp_cast_0 , _dissolvedistortion);
			float2 panner169 = ( 1.0 * _Time.y * _dissolvepanning + lerpResult74);
			float temp_output_1_0_g33 = tex2D( _dissolvetexture, panner169 ).r;
			float temp_output_3_0_g34 = ( temp_output_12_0_g33 - temp_output_1_0_g33 );
			float temp_output_8_0_g33 = saturate( ( temp_output_3_0_g34 / fwidth( temp_output_3_0_g34 ) ) );
			float temp_output_3_0_g35 = ( temp_output_12_0_g33 - ( temp_output_1_0_g33 + _dissolveedge ) );
			float temp_output_65_13 = ( temp_output_8_0_g33 - saturate( ( temp_output_3_0_g35 / fwidth( temp_output_3_0_g35 ) ) ) );
			float4 dissolveedge70 = ( temp_output_65_13 * _edgecolor );
			float2 uv_TexCoord81 = i.uv_texcoord * _noisetilling;
			float2 CenteredUV15_g32 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g32 = CenteredUV15_g32;
			float2 appendResult23_g32 = (float2(( length( CenteredUV15_g32 ) * _noisetilling.x * 2.0 ) , ( atan2( break17_g32.x , break17_g32.y ) * ( 1.0 / 6.28318548202515 ) * _noisetilling.y )));
			float2 lerpResult85 = lerp( uv_TexCoord81 , appendResult23_g32 , _noisepolar);
			float2 temp_cast_1 = (( distortionTex75 * _noisedistortion )).xx;
			float2 panner82 = ( 1.0 * _Time.y * _noisepanning + ( lerpResult85 - temp_cast_1 ));
			o.Emission = ( dissolveedge70 + ( ( tex2D( _noisetexture, panner82 ).r * _noiseintensity ) * _brightness * i.vertexColor ) ).rgb;
			float2 uv_TexCoord116 = i.uv_texcoord * _masktilling;
			float2 CenteredUV15_g30 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g30 = CenteredUV15_g30;
			float2 appendResult23_g30 = (float2(( length( CenteredUV15_g30 ) * _masktilling.x * 2.0 ) , ( atan2( break17_g30.x , break17_g30.y ) * ( 1.0 / 6.28318548202515 ) * _masktilling.y )));
			float2 lerpResult120 = lerp( uv_TexCoord116 , appendResult23_g30 , _maskpolar);
			float temp_output_126_0 = ( distortionTex75 * _maskdistortion );
			float2 temp_cast_3 = (temp_output_126_0).xx;
			float2 panner123 = ( 1.0 * _Time.y * _maskpanning + ( lerpResult120 - temp_cast_3 ));
			float2 maskUV124 = panner123;
			float2 appendResult143 = (float2(i.uv_tex4coord.x , i.uv_tex4coord.y));
			float2 temp_cast_4 = (temp_output_126_0).xx;
			float2 animblendUV146 = ( appendResult143 - temp_cast_4 );
			float2 lerpResult157 = lerp( maskUV124 , animblendUV146 , _animblend);
			float4 tex2DNode2 = tex2D( _masktexture, lerpResult157 );
			float2 appendResult144 = (float2(i.uv_tex4coord.z , i.uv_tex4coord.w));
			float2 temp_cast_5 = (temp_output_126_0).xx;
			float lerpResult151 = lerp( tex2DNode2.r , tex2D( _masktexture, ( appendResult144 - temp_cast_5 ) ).r , i.uv2_texcoord2.x);
			float animBlendTex153 = lerpResult151;
			float lerpResult155 = lerp( tex2DNode2.r , animBlendTex153 , _animblend);
			float smoothstepResult167 = smoothstep( _maskstep , 1.0 , lerpResult155);
			float temp_output_3_0_g37 = ( ( 1.0 - _maskstep ) - lerpResult155 );
			#ifdef _STEP_ON
				float staticSwitch166 = saturate( ( temp_output_3_0_g37 / fwidth( temp_output_3_0_g37 ) ) );
			#else
				float staticSwitch166 = smoothstepResult167;
			#endif
			float temp_output_73_0 = ( temp_output_8_0_g33 + temp_output_65_13 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth109 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth109 = saturate( abs( ( screenDepth109 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _depthfade ) ) );
			float temp_output_80_0 = saturate( ( staticSwitch166 * i.vertexColor.a * temp_output_73_0 * distanceDepth109 ) );
			o.Alpha = temp_output_80_0;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.0
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
				float4 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				float4 screenPos : TEXCOORD4;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xyzw = customInputData.uv_tex4coord;
				o.customPack1.xyzw = v.texcoord;
				o.customPack2.xy = customInputData.uv_texcoord;
				o.customPack2.xy = v.texcoord;
				o.customPack2.zw = customInputData.uv2_texcoord2;
				o.customPack2.zw = v.texcoord1;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.uv2_texcoord2 = IN.customPack2.zw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
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
73;81;1733;912;1759.204;-1426.903;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;52;-2840.197,-765.5655;Inherit;False;2330.234;797.707;Comment;10;37;36;58;48;35;57;50;49;17;75;distortion tex;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;37;-2633.34,-585.8804;Inherit;False;Property;_distortiontilling;distortion tilling;10;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-2317.38,-557.9783;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;58;-2095.863,-352.1941;Inherit;False;Property;_distortionpolar;distortion polar;9;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;48;-2412.688,-419.7222;Inherit;True;Polar Coordinates;-1;;8;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;35;-2107.911,-221.286;Inherit;False;Property;_distortionpanning;distortion panning;11;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;33;-2085.723,-78.85838;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1841.901,-200.0527;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;57;-1916.57,-472.0765;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-1660.472,-391.6242;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;113;917.4344,646.9218;Inherit;False;1640.224;592.3333;Comment;13;123;122;121;120;119;118;117;116;115;114;124;127;126;mask uv;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;17;-1446.801,-450.049;Inherit;True;Property;_distortiontexture;distortion texture;8;2;[Header];[NoScaleOffset];Create;True;1;Distortion;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;114;967.4344,757.7701;Inherit;False;Property;_masktilling;mask tilling;17;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-1067.513,-290.8099;Inherit;False;distortionTex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;115;1325.195,1001.178;Inherit;False;Property;_maskpolar;mask polar;16;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;119;1365.438,1117.783;Inherit;False;Property;_maskdistortion;mask distortion;15;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;116;1199.997,731.7896;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;117;1215.567,853.0994;Inherit;False;Polar Coordinates;-1;;30;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;152;753.3993,1434.674;Inherit;False;1379.021;611.3053;Comment;10;153;151;147;158;146;159;144;143;142;160;anim blend;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;1429.808,899.2286;Inherit;False;75;distortionTex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;142;781.4803,1551.059;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;120;1548.534,696.9217;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;1682.755,904.0447;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;127;1825.755,846.0447;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;121;1813.679,968.5183;Inherit;False;Property;_maskpanning;mask panning;18;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;143;1041.808,1534.706;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;159;1254.771,1516.495;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;123;2043.99,731.1521;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;162;-996.5016,766.4496;Inherit;False;1430.013;650.0468;Comment;12;154;155;2;157;125;149;156;164;165;166;167;168;mask tex;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;124;2199.677,983.6581;Inherit;False;maskUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;146;1460.182,1529.871;Inherit;False;animblendUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;78;-1823.885,1586.021;Inherit;False;2413.081;955.9361;Comment;22;63;62;59;60;76;61;77;23;74;66;105;9;69;65;68;70;73;94;140;141;169;171;dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;144;1036.969,1682.783;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-576.6673,1163.228;Inherit;False;Property;_animblend;animblend;2;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;149;-943.2631,1084.163;Inherit;False;146;animblendUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;125;-946.5016,995.2071;Inherit;False;124;maskUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;63;-1773.885,1636.021;Inherit;False;Property;_dissolvetilling;dissolve tilling;31;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;160;1198.086,1663.34;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;88;-1974.848,87.89024;Inherit;False;1640.224;592.3333;Comment;10;82;89;83;91;85;90;84;81;86;87;noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;157;-596.3509,870.047;Inherit;True;3;0;FLOAT2;1,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-1344.87,1982.439;Inherit;False;Property;_dissolvepolar;dissolve polar;28;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;60;-1493.393,1644.024;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;59;-1462.393,1807.024;Inherit;False;Polar Coordinates;-1;;31;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;61;-1086.329,1786.556;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;147;1392.834,1655.663;Inherit;True;Property;_TextureSample0;Texture Sample 0;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;2;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-349.8569,816.4496;Inherit;True;Property;_masktexture;mask texture;12;2;[Header];[NoScaleOffset];Create;True;1;Mask;0;0;False;0;False;-1;None;542b1d1b73cc2c44886b7c7861c2a05c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;87;-1924.848,198.7386;Inherit;False;Property;_noisetilling;noise tilling;23;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;77;-1107.659,2080.65;Inherit;False;Property;_dissolvedistortion;dissolve distortion;32;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;158;1388.467,1854.082;Inherit;False;1;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;76;-1103.916,1907.261;Inherit;False;75;distortionTex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;74;-831.186,1817.014;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-1567.087,442.1466;Inherit;False;Property;_noisepolar;noise polar;20;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-765.5226,2027.734;Inherit;False;Property;_dissolve;dissolve;29;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;171;-1202.204,1650.903;Inherit;False;Property;_dissolvepanning;dissolve panning;30;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;91;-1469.664,537.7517;Inherit;False;Property;_noisedistortion;noise distortion;22;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;151;1702.887,1624.039;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-1692.285,172.7581;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;84;-1676.716,294.0679;Inherit;False;Polar Coordinates;-1;;32;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;90;-1353.474,254.1971;Inherit;False;75;distortionTex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;105;-671.4856,1946.154;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-1153.491,391.1389;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;140;-895.6593,2179.127;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;1873.252,1719.207;Inherit;False;animBlendTex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;85;-1343.748,137.8902;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;169;-940.5875,1604.316;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-570.8472,1293.18;Inherit;False;153;animBlendTex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-648.0276,1704.62;Inherit;True;Property;_dissolvetexture;dissolve texture;25;2;[Header];[NoScaleOffset];Create;True;1;Dissolve;0;0;False;0;False;-1;None;15b4ac9d60961794eaffcb9fe086816f;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;66;-658.8437,2177.972;Inherit;False;Property;_dissolveedge;dissolve edge;33;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-397.9171,1331.388;Inherit;False;Property;_maskstep;mask step;14;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;141;-462.8507,1924.768;Inherit;False;Property;_usecvs;use cvs?;26;0;Create;True;0;0;0;False;0;False;0;0;1;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;83;-1111.866,503.4537;Inherit;False;Property;_noisepanning;noise panning;24;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;128;-1025.491,310.1389;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;82;-726.756,226.3917;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;155;-348.0415,1096.925;Inherit;True;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;168;-121.0176,1265.261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;69;-210.8186,2076.562;Inherit;False;Property;_edgecolor;edge color;27;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;65;-238.9003,1887.054;Inherit;False;F_dissolve_edge;-1;;33;a1dc6ace39b4a794c93a3c3dad34faf0;0;3;1;FLOAT;0;False;12;FLOAT;0;False;15;FLOAT;0;False;3;FLOAT;0;FLOAT;16;FLOAT;13
Node;AmplifyShaderEditor.RangedFloatNode;111;291.481,580.3691;Inherit;False;Property;_depthfade;depth fade;1;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;165;37.20832,994.6429;Inherit;True;Step Antialiasing;-1;;37;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-220.1251,108.156;Inherit;True;Property;_noisetexture;noise texture;19;2;[Header];[NoScaleOffset];Create;True;1;Noise;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-138.8011,339.1035;Inherit;False;Property;_noiseintensity;noise intensity;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;167;44.46339,1201.921;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;105.9477,2105.274;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;109;335.2889,477.6585;Inherit;False;True;True;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;78.08592,1660.007;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;389.2203,-244.1898;Inherit;False;Property;_brightness;brightness;5;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;12.23613,12.23613,12.23613,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;95;341.1429,268.4285;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;166;235.2558,853.5966;Inherit;False;Property;_step;step;13;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;366.7019,2054.961;Inherit;False;dissolveedge;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;114.5988,141.3034;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;749.0932,316.9383;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;93;955.3879,-476.7144;Inherit;False;224;167;Comment;1;92;cull;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;649.0646,93.12926;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;634.0205,-23.30679;Inherit;False;70;dissolveedge;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;96;-2247.695,-1331.517;Inherit;False;1296.245;539.3605;Comment;9;97;99;100;56;24;53;101;55;28;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1861.207,-1281.517;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;71.35999,-7.173865;Inherit;False;Property;_mainpow;main pow;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-396.9437,-169.1496;Inherit;False;28;UV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;99;-2224.749,-1283.652;Inherit;False;Property;_maintillingpanning;main tilling panning;7;0;Create;True;0;0;0;False;0;False;1,1,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;122;1769.033,714.288;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-106.5239,-201.5385;Inherit;True;Property;_maintexture;main texture;3;2;[Header];[NoScaleOffset];Create;True;1;Main;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;71;839.0205,77.69321;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;80;925.8786,273.9964;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;100;-1999.459,-1260.156;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;106;298.1485,-79.45855;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1753.437,-932.2567;Inherit;False;Property;_mainpolar;main polar;4;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;101;-1966.638,-1138.705;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-1188.89,-1032.211;Inherit;True;UV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;92;1005.388,-426.7144;Inherit;False;Property;_CullMode;CullMode;0;2;[Header];[Enum];Create;True;1;Additional;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;89;-1123.249,155.2565;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;53;-1787.76,-1081.806;Inherit;False;Polar Coordinates;-1;;38;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0.5,0.5;False;3;FLOAT;1;False;4;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;94;341.177,1693.888;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;130;1109.076,168.3405;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;97;-1293.709,-1185.05;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;108;481.7057,13.25609;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;55;-1573.827,-1266.753;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;138;1307.463,67.7491;Float;False;True;-1;4;ASEMaterialInspector;0;0;Standard;MetaMonkeys/AdvancedParticle;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;92;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;37;0
WireConnection;48;3;37;1
WireConnection;48;4;37;2
WireConnection;50;0;35;0
WireConnection;50;1;33;0
WireConnection;57;0;36;0
WireConnection;57;1;48;0
WireConnection;57;2;58;0
WireConnection;49;0;57;0
WireConnection;49;1;50;0
WireConnection;17;1;49;0
WireConnection;75;0;17;1
WireConnection;116;0;114;0
WireConnection;117;3;114;1
WireConnection;117;4;114;2
WireConnection;120;0;116;0
WireConnection;120;1;117;0
WireConnection;120;2;115;0
WireConnection;126;0;118;0
WireConnection;126;1;119;0
WireConnection;127;0;120;0
WireConnection;127;1;126;0
WireConnection;143;0;142;1
WireConnection;143;1;142;2
WireConnection;159;0;143;0
WireConnection;159;1;126;0
WireConnection;123;0;127;0
WireConnection;123;2;121;0
WireConnection;124;0;123;0
WireConnection;146;0;159;0
WireConnection;144;0;142;3
WireConnection;144;1;142;4
WireConnection;160;0;144;0
WireConnection;160;1;126;0
WireConnection;157;0;125;0
WireConnection;157;1;149;0
WireConnection;157;2;156;0
WireConnection;60;0;63;0
WireConnection;59;3;63;1
WireConnection;59;4;63;2
WireConnection;61;0;60;0
WireConnection;61;1;59;0
WireConnection;61;2;62;0
WireConnection;147;1;160;0
WireConnection;2;1;157;0
WireConnection;74;0;61;0
WireConnection;74;1;76;0
WireConnection;74;2;77;0
WireConnection;151;0;2;1
WireConnection;151;1;147;1
WireConnection;151;2;158;1
WireConnection;81;0;87;0
WireConnection;84;3;87;1
WireConnection;84;4;87;2
WireConnection;105;0;23;0
WireConnection;129;0;90;0
WireConnection;129;1;91;0
WireConnection;153;0;151;0
WireConnection;85;0;81;0
WireConnection;85;1;84;0
WireConnection;85;2;86;0
WireConnection;169;0;74;0
WireConnection;169;2;171;0
WireConnection;9;1;169;0
WireConnection;141;1;105;0
WireConnection;141;0;140;3
WireConnection;128;0;85;0
WireConnection;128;1;129;0
WireConnection;82;0;128;0
WireConnection;82;2;83;0
WireConnection;155;0;2;1
WireConnection;155;1;154;0
WireConnection;155;2;156;0
WireConnection;168;0;164;0
WireConnection;65;1;9;1
WireConnection;65;12;141;0
WireConnection;65;15;66;0
WireConnection;165;1;155;0
WireConnection;165;2;168;0
WireConnection;4;1;82;0
WireConnection;167;0;155;0
WireConnection;167;1;164;0
WireConnection;68;0;65;13
WireConnection;68;1;69;0
WireConnection;109;0;111;0
WireConnection;73;0;65;16
WireConnection;73;1;65;13
WireConnection;166;1;167;0
WireConnection;166;0;165;0
WireConnection;70;0;68;0
WireConnection;8;0;4;1
WireConnection;8;1;7;0
WireConnection;22;0;166;0
WireConnection;22;1;95;4
WireConnection;22;2;73;0
WireConnection;22;3;109;0
WireConnection;3;0;8;0
WireConnection;3;1;21;0
WireConnection;3;2;95;0
WireConnection;24;0;100;0
WireConnection;122;0;120;0
WireConnection;122;1;118;0
WireConnection;122;2;119;0
WireConnection;1;1;31;0
WireConnection;71;0;72;0
WireConnection;71;1;3;0
WireConnection;80;0;22;0
WireConnection;100;0;99;1
WireConnection;100;1;99;2
WireConnection;106;0;1;1
WireConnection;106;1;107;0
WireConnection;101;0;99;3
WireConnection;101;1;99;4
WireConnection;28;0;97;0
WireConnection;89;0;85;0
WireConnection;89;1;90;0
WireConnection;89;2;91;0
WireConnection;53;3;99;1
WireConnection;53;4;99;2
WireConnection;94;0;73;0
WireConnection;130;3;80;0
WireConnection;97;0;55;0
WireConnection;97;2;101;0
WireConnection;108;0;106;0
WireConnection;55;0;24;0
WireConnection;55;1;53;0
WireConnection;55;2;56;0
WireConnection;138;2;71;0
WireConnection;138;9;80;0
ASEEND*/
//CHKSM=E08CDCED14DEB6A550B1DA6F5C1FABE2C77C1D81