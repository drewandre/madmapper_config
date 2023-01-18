/*{
    "CREDIT": "Drew Andre",
    "DESCRIPTION": "Lava-style noise",
    "TAGS": "noise",
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
		{ "LABEL": "Public/Speed", "NAME": "mat_speed", "TYPE": "float", "MIN": 0.0, "MAX": 2.0, "DEFAULT": 1.0 }, 
		{ "LABEL": "Public/Scale", "NAME": "mat_scale", "TYPE": "float", "MIN": 0.1, "MAX": 2.0, "DEFAULT": 1.0 }, 
        { "LABEL": "Private/Transition Size", "NAME": "mat_trans", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
		{ "LABEL": "Private/Granularity", "NAME": "mat_gran", "TYPE": "float", "MIN": 0.0, "MAX": 1.0, "DEFAULT": 0.5 },
		{ "LABEL": "Public/Front Color", "NAME": "foregroundColor", "TYPE": "color", "DEFAULT": [ 1.0, 1.0, 1.0, 1.0 ] },
		{ "LABEL": "Public/Back Color", "NAME": "backgroundColor", "TYPE": "color", "DEFAULT": [ 1.0, 1.0, 1.0, 1.0 ] },
		{
			"LABEL": "Public/Brightness",
			"NAME": "brightness",
			"TYPE": "float",
			"MIN": -1.0,
			"MAX": 1.0,
			"DEFAULT": 0
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
            "DEFAULT": 0.1,
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
        {"NAME": "mat_animation_time", "TYPE": "time_base", "PARAMS": {"speed": "mat_speed", "speed_curve":1,"bpm_sync":false, "link_speed_to_global_bpm":false}},
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
#include "MadNoise.glsl"
#include "MadSDF.glsl"	

#define s smoothstep

vec4 getColor(vec4 colorRGBA1, vec4 colorRGBA2) {
    float alpha = 1.0 - ((1.0 - colorRGBA1.a) * (1.0 - colorRGBA2.a) / 1.0);
    float red   = (colorRGBA1.r * (1.0 - colorRGBA2.a) + colorRGBA2.r * colorRGBA2.a) / 1.0;
    float green = (colorRGBA1.g * (1.0 - colorRGBA2.a) + colorRGBA2.g * colorRGBA2.a) / 1.0;
    float blue  = (colorRGBA1.b * (1.0 - colorRGBA2.a) + colorRGBA2.b * colorRGBA2.a) / 1.0;
    return vec4(red, green, blue, alpha);
}

vec4 applyColorMods(vec4 color, float c, float s, float b) {
	float a = color.a;
	vec3 output_color = applyContrastSaturationBrightness(color.rgb, c + 1, s + 1, b + 1);
	return vec4(output_color.r, output_color.g, output_color.b, a);
}

// book of shaders mathemagic
// https://thebookofshaders.com/11/
// https://thebookofshaders.com/edit.php#11/iching-03.frag

vec3 random3(vec3 c) {
    float j = 4096.0*sin(dot(c,vec3(17.0, 59.4, 15.0)));
    vec3 r;
    r.z = fract(512.0*j);
    j *= .125;
    r.x = fract(512.0*j);
    j *= .125;
    r.y = fract(512.0*j);
    return r-0.5;
}

const float F3 =  0.333333;
const float G3 =  0.1666667;
float snoise(vec3 p) {

    vec3 s = floor(p + dot(p, vec3(F3)));
    vec3 x = p - s + dot(s, vec3(G3));

    vec3 e = step(vec3(0.0), x - x.yzx);
    vec3 i1 = e*(1.0 - e.zxy);
    vec3 i2 = 1.0 - e.zxy*(1.0 - e);

    vec3 x1 = x - i1 + G3;
    vec3 x2 = x - i2 + 2.0*G3;
    vec3 x3 = x - 1.0 + 3.0*G3;

    vec4 w, d;

    w.x = dot(x, x);
    w.y = dot(x1, x1);
    w.z = dot(x2, x2);
    w.w = dot(x3, x3);

    w = max(0.6 - w, 0.0);

    d.x = dot(random3(s), x);
    d.y = dot(random3(s + i1), x1);
    d.z = dot(random3(s + i2), x2);
    d.w = dot(random3(s + 1.0), x3);

    w *= w;
    w *= w;
    d *= w;

    return dot(d, vec4(52.0));
}

vec4 materialColorForPixel( vec2 texCoord )
{
	// get texture coordinates
	vec2 uv = texCoord;
	uv.y = 1. - uv.y;
	// modify uv with material inputs
	uv += vec2(0.5,0.);
	
	uv *= mat_scale;

	vec2 m = vec2(mat_trans,mat_gran);

    vec2 c = vec2(.5, .5);
    float transitionSize = m.y;
    float granularity = m.x*5.;
    float t = mat_animation_time*m.y;

    float noiseInX = uv.x*granularity;
    float noiseInY = uv.y*granularity-t;
    float n = snoise(vec3(noiseInX, noiseInY, 0));
    float pct = 1 - s(0., 1., uv.y+transitionSize*n);

	vec4 color = vec4(pct);

	color *= foregroundColor.rgba;

	color.r *= 1.0 + (1.0 * audio_amplitude_decay * (amplitude * 15));
	color.g *= 1.0 + (1.0 * audio_amplitude_decay * (amplitude * 15));
	color.b *= 1.0 + (1.0 * audio_amplitude_decay * (amplitude * 15));

	color = applyColorMods(color, contrast, saturation, brightness);

	color = mix(color, backgroundColor.rgba, 0.6);
	
	return color;
}
