Shader "Custom/Shader_1"
{
    Properties
    {
		_Radius ("Radius(圆半径)",Float)= 0.5   //圆半径
        _Color1 ("Color1", Color) = (1,1,1,1)
    	_Color2 ("Color2", Color) = (1,1,1,1)
	    _Color3 ("Color3", Color) = (1,1,1,1)
	    _Color4 ("Color4", Color) = (1,1,1,1)
       	_Color5 ("Color5", Color) = (1,1,1,1)
    	
    	_OffsetX("OffsetXr",float) = 1
    	_OffsetY("OffsetYr",float) = 1
		_ThresholdAlpha("ThresholdAlpha",float) = 1
    	_ShadowColor("ShadowColor",Color) = (0,0,0,1)

    	_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
    	Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
		Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        pass
		{
			CGPROGRAM
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag
			#include "unitycg.cginc"
			
			float _Radius;
			sampler2D _MainTex;
			fixed4 _Color1;
			fixed4 _Color2;
			fixed4 _Color3;
			fixed4 _Color4;
			fixed4 _Color5;

			half _OffsetX;
			half _OffsetY;
			fixed _ThresholdAlpha;
			fixed4 _ShadowColor;


			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 ModeUV: TEXCOORD0;
				float2 RadiusBuceVU : TEXCOORD1;
			};
			
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos=UnityObjectToClipPos(v.vertex); //v.vertex;
				o.ModeUV=v.texcoord;
				o.RadiusBuceVU=v.texcoord-float2(0.5,0.5);       //将模型UV坐标原点置为中心原点,为了方便计算  原本坐标原点在左下角
				return o;
			}
			
			//内部上色
			fixed4 prefrag(v2f i)
			{
				fixed4 col;

				if(tex2D(_MainTex,i.ModeUV).a <= _ThresholdAlpha)   //去除不着色点
				{
					col=fixed4 (1,1,1,0);
					return col;
				}

				if(length(abs(i.RadiusBuceVU)) >_Radius)  //在圆外的像素 坐标到圆心的距离是否大于半径
				{
					col=fixed4 (1,1,1,1);
					return col;
				}
				////环一
				if(_Radius <= 0.05)
				{
					col=_Color1;
					return col;
				}
				_Radius -= 0.05;
				if(length(abs(i.RadiusBuceVU)) >_Radius)  //在圆外的像素 坐标到圆心的距离是否大于半径
				{
					col=_Color1;
					return col;
				}
				
				//环二
				if(_Radius <= 0.05)
				{
					col=_Color2;
					return col;
				}
				_Radius -= 0.05;
				if(length(abs(i.RadiusBuceVU)) >_Radius)  //在圆外的像素 坐标到圆心的距离是否大于半径
				{
					col=_Color2;
					return col;
				}
				
				//环3
				if(_Radius <= 0.05)
				{
					col=_Color3;
					return col;
				}
				_Radius -= 0.05;
				if(length(abs(i.RadiusBuceVU)) >_Radius)  //在圆外的像素 坐标到圆心的距离是否大于半径
				{
					col=_Color3;
					return col;
				}
				
				//环4
				if(_Radius <= 0.05)
				{
					col=_Color4;
					return col;
				}
				_Radius -= 0.05;
				if(length(abs(i.RadiusBuceVU)) >_Radius)  //在圆外的像素 坐标到圆心的距离是否大于半径
				{
					col=_Color4;
					return col;
				}
				//剩余部分
				col=_Color5;
				return col;	
			}

			//最终处理
			fixed4 frag(v2f i):COLOR
			{
				//预处理
				fixed4 col = prefrag(i);
				//确定阴影颜色
				fixed shadowAlpha = tex2D(_MainTex, i.ModeUV + half2(_OffsetX,_OffsetY)).a;
				fixed4 shadowColor = fixed4(_ShadowColor.rgb, _ShadowColor.a * shadowAlpha);
				//确定是否有阴影
				fixed stepVal = step(_ThresholdAlpha, col.a);
				fixed4 colResult = (stepVal) * col + (1-stepVal) * shadowColor;

				if(colResult.a<0.5)
				{
					discard;
				}
				return colResult;
			}

			
			
			ENDCG
		}
    }
    FallBack "VertexLit"
}
