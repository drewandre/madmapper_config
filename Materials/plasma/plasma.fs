/*{
    "CREDIT": "Drew Andre",
    "DESCRIPTION": "Noise",
    "TAGS": "plasma",
    "VSN": "1.0",
	"RASTERISATION_SETTINGS": {
	    "DEFAULT_RENDER_TO_TEXTURE": true,
	    "DEFAULT_WIDTH": 1024,
	    "DEFAULT_HEIGHT": 1024,
		"REQUIRES_LAST_FRAME": true,
	},
    "INPUTS": [
        {
            "NAME": "mat_waveform",
            "TYPE": "audio"
        },
        { "LABEL": "Public/Speed", "NAME": "uSpeed", "TYPE": "float", "MIN": 0, "MAX": 1.0, "DEFAULT": 0.15 },
		{ "LABEL": "Public/Scale", "NAME": "uScale", "TYPE": "float", "MIN": 0.0, "MAX": 4.0, "DEFAULT": 1.0 },
		{ "LABEL": "Private/Milky", "NAME": "uMilk", "TYPE": "float", "MIN": 0.0, "MAX": 1.2, "DEFAULT": 0.0 },
		{
			"LABEL": "Public/Brightness",
			"NAME": "brightness",
			"TYPE": "float",
			"MIN": 0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		},
		{
			"LABEL": "Public/Saturation",
			"NAME": "saturation",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
			"LABEL": "Public/Contrast",
			"NAME": "contrast",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0.0
		},
		{
            "LABEL": "Private/Amplitude",
            "NAME": "amplitude",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.2
        },
        {
            "LABEL": "Private/Audio Attack",
            "NAME": "attack",
            "TYPE": "float",
            "DEFAULT": 0.5,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "LABEL": "Private/Audio Decay",
            "NAME": "decay",
            "TYPE": "float",
            "DEFAULT": 0.1,
            "MIN": 0.0,
            "MAX": 1.0
        },
        {
            "LABEL": "Private/Audio Release",
            "NAME": "release",
            "TYPE": "float",
            "DEFAULT": 0.7,
            "MIN": 0.0,
            "MAX": 1.0
        },
      ],
	 "GENERATORS": [
		 {"NAME": "animation_time", "TYPE": "time_base", "PARAMS": {"speed": "uSpeed", "speed_curve":1, "bpm_sync":false, "link_speed_to_global_bpm":false}},
        {
            "NAME": "audio_amplitude",
            "TYPE": "pass_thru",
            "PARAMS": {
                "input_value": "/audioin/MadMapper/amplitude"
            }
        },
        {
            "NAME": "audio_amplitude_decay",
            "TYPE": "adsr",
            "PARAMS": {
                "input_value": "audio_amplitude",
                "attack": "attack",
                "decay": "decay",
                "release": "release"
            }
        },
     ],
}*/

#include "MadCommon.glsl"

float hash( float n )
{
	return fract(sin(n)*43758.5453);
}

float xnoise( in vec3 x )
{
	vec3 p = floor(x);
	vec3 f = fract(x);

	f = f*f*(3.0-2.0*f);
	float n = p.x + p.y*57.0 + 113.0*p.z;
	return mix(mix(mix( hash(n+  0.0), hash(n+  1.0),f.x),
				   mix( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y),
			   mix(mix( hash(n+113.0), hash(n+114.0),f.x),
				   mix( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
}

vec3 xnoise3( in vec3 x)
{
	return vec3( xnoise(x+vec3(123.456,.567,.37)),
				xnoise(x+vec3(.11,47.43,19.17)),
				xnoise(x) );
}

mat3 rotation(float angle, vec3 axis)
{
	float s = sin(-angle);
	float c = cos(-angle);
	float oc = 0.15 - c;
	vec3 sa = axis * s;
	vec3 oca = axis * oc;
	return mat3(	
		oca.x * axis + vec3(	c,	-sa.z,	sa.y),
		oca.y * axis + vec3( sa.z,	c,		-sa.x),		
		oca.z * axis + vec3(-sa.y,	sa.x,	c));	
}

// https://code.google.com/p/fractalterraingeneration/wiki/Fractional_Brownian_Motion
vec3 fbm(vec3 x, float H, float L)
{
	vec3 v = vec3(0);
	float f = 1.;

	for (int i=0; i<7; i++)
	{
		float w = pow(f,-H);
		v += xnoise3(x)*w;
		x *= L;
		f *= L;
	}
	return v;
}
float easeInOutQuart(float x) {
	return x < 0.5 ? 8 * x * x * x * x : 1 - pow(-2 * x + 2, 4) / 2;
}


vec4 materialColorForPixel( vec2 texCoord )
{
	vec2 uv = texCoord*uScale*0.1;
	float time = animation_time*0.1;

	vec3 p = vec3(uv,time) / (1 - audio_amplitude_decay);
	vec3 axis = 4. * fbm(p, 0.5, 1.6);
	
	vec3 color = 0.5 * 5. * fbm(p*0.3,0.5,1.6);	
	color = rotation(3.*length(axis),normalize(axis))*color;
	color = applyContrastSaturationBrightness(color, 1 + contrast, 1 + saturation, 1);

	vec3 color_2  = color * 0.05;
	color_2 = applyContrastSaturationBrightness(color_2, 1 + contrast, 1 + saturation, 1);

	color_2 = pow(color_2,vec3(0.12));
	color_2 *= 2.0 * color_2;
	color = mix(color,color_2,uMilk);

	color *= vec3(brightness);

	return vec4(color.rgb, 1);
}
