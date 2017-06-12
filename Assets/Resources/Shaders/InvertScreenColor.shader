// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:1,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:6,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33655,y:32955,varname:node_2865,prsc:2|emission-4578-OUT,voffset-4177-OUT;n:type:ShaderForge.SFN_TexCoord,id:6793,x:31733,y:33224,varname:node_6793,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ProjectionParameters,id:8707,x:31733,y:33437,varname:node_8707,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:9496,x:31932,y:33224,varname:node_9496,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-6793-UVOUT;n:type:ShaderForge.SFN_Append,id:6538,x:31932,y:33396,varname:node_6538,prsc:2|A-5896-OUT,B-8707-SGN;n:type:ShaderForge.SFN_Vector1,id:5896,x:31733,y:33378,varname:node_5896,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:4177,x:32131,y:33294,varname:node_4177,prsc:2|A-9496-OUT,B-6538-OUT;n:type:ShaderForge.SFN_Tex2d,id:8553,x:31763,y:33065,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_8553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_OneMinus,id:4950,x:31956,y:33065,varname:node_4950,prsc:2|IN-8553-RGB;n:type:ShaderForge.SFN_Multiply,id:2328,x:31943,y:32805,varname:node_2328,prsc:2|A-8553-RGB,B-4323-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4323,x:31943,y:32750,ptovrint:False,ptlb:node_4323,ptin:_node_4323,varname:node_4323,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Add,id:2526,x:32156,y:32816,varname:node_2526,prsc:2|A-2328-OUT,B-2781-OUT;n:type:ShaderForge.SFN_Vector1,id:2781,x:32156,y:32750,varname:node_2781,prsc:2,v1:-1;n:type:ShaderForge.SFN_Multiply,id:3275,x:32482,y:32859,varname:node_3275,prsc:2|A-2526-OUT,B-4462-OUT;n:type:ShaderForge.SFN_Slider,id:4462,x:31983,y:32981,ptovrint:False,ptlb:InvertMultiplier,ptin:_InvertMultiplier,varname:node_4462,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Add,id:2028,x:32532,y:33020,varname:node_2028,prsc:2|A-3275-OUT,B-4950-OUT;n:type:ShaderForge.SFN_Color,id:1166,x:32274,y:33364,ptovrint:False,ptlb:node_9885_copy,ptin:_node_9885_copy,varname:_node_9885_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:4578,x:33380,y:33056,varname:node_4578,prsc:2|A-2028-OUT,B-1166-RGB,T-1872-OUT;n:type:ShaderForge.SFN_Add,id:942,x:32559,y:33178,varname:node_942,prsc:2|A-4462-OUT,B-9728-OUT;n:type:ShaderForge.SFN_Vector1,id:9728,x:32379,y:33260,varname:node_9728,prsc:2,v1:-0.5;n:type:ShaderForge.SFN_Abs,id:189,x:32734,y:33160,varname:node_189,prsc:2|IN-942-OUT;n:type:ShaderForge.SFN_Multiply,id:2737,x:32934,y:33160,varname:node_2737,prsc:2|A-189-OUT,B-6091-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6091,x:32824,y:32956,ptovrint:False,ptlb:node_4323_copy,ptin:_node_4323_copy,varname:_node_4323_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-2;n:type:ShaderForge.SFN_Add,id:1872,x:33034,y:32885,varname:node_1872,prsc:2|A-9307-OUT,B-2737-OUT;n:type:ShaderForge.SFN_Vector1,id:9307,x:32824,y:32866,varname:node_9307,prsc:2,v1:1;proporder:8553-4323-4462-1166-6091;pass:END;sub:END;*/

Shader "Shader Forge/InvertScreenColor" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _node_4323 ("node_4323", Float ) = 2
        _InvertMultiplier ("InvertMultiplier", Range(0, 1)) = 0.5
        _node_9885_copy ("node_9885_copy", Color) = (0,0,0,1)
        _node_4323_copy ("node_4323_copy", Float ) = -2
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _node_4323;
            uniform float _InvertMultiplier;
            uniform float4 _node_9885_copy;
            uniform float _node_4323_copy;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                v.vertex.xyz = float3(((o.uv0*2.0+-1.0)*float2(1.0,_ProjectionParams.r)),0.0);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = v.vertex;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float3 node_2028 = ((((_MainTex_var.rgb*_node_4323)+(-1.0))*_InvertMultiplier)+(1.0 - _MainTex_var.rgb));
                float3 node_4578 = lerp(node_2028,_node_9885_copy.rgb,(1.0+(abs((_InvertMultiplier+(-0.5)))*_node_4323_copy)));
                float3 emissive = node_4578;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
