/*{
    "CREDIT": "Drew Andre",
    "TAGS": "graphics",
    "VSN": "1.1",
    "DESCRIPTION": "Automatically converted from https://www.shadertoy.com/view/4dBSRK by IQ.",
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
        {
            "LABEL": "Public/Speed",
            "NAME": "mat_speed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.5,
        },
        {
            "LABEL" : "Public/Scale",
            "NAME": "size",
            "TYPE": "float",
            "DEFAULT": 0.25,
            "MIN": 0,
            "MAX": 1
        },
        { "LABEL": "Public/Back Color", "NAME": "backgroundColor", "TYPE": "color", "DEFAULT": [ 0.0, 0.0, 0.0, 1.0 ] },
		{
			"LABEL": "Public/Front Color",
            "NAME": "foregroundColor",
            "TYPE": "color",
            "DEFAULT": [ 1.0, 1.0, 1.0, 1.0 ]
		},
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
            "LABEL": "Private/Speed Y",
            "NAME": "yspeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.05
        },
        {
            "LABEL" : "Private/Dist",
            "NAME": "dist",
            "TYPE": "float",
            "DEFAULT": 0.15,
            "MIN": 0,
            "MAX": 1
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
        {
            "NAME": "ytime",
            "TYPE": "time_base",
            "PARAMS": {
                "speed": "yspeed",
                "speed_curve": 1,
                "bpm_sync": false,
                "link_speed_to_global_bpm": false
            }
        },
        {
            "NAME": "mat_time",
            "TYPE": "time_base",
            "PARAMS": {
                "speed": "mat_speed",
                "speed_curve": 1,
                "reverse": true,
                "bpm_sync": false,
                "link_speed_to_global_bpm":false
            }
        }
    ]
}
*/

// IQ_ColoredGridThingy by mojovideotech
// source : www.shadertoy.com/view/4dBSRK
// created by IQ : www.iquilezles.org/
// interactive mods by DoctorMojo : www.mojovideotech.com/

// Originally Created by inigo quilez - iq/2014
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.


#include "MadCommon.glsl"

vec4 applyColorMods(vec4 color, float c, float s, float b) {
	float a = color.a;
	vec3 output_color = applyContrastSaturationBrightness(color.rgb, c + 1, s + 1, b + 1);
	return vec4(output_color.r, output_color.g, output_color.b, a);
}

vec4 materialColorForPixel( vec2 texCoord ) {

	vec2 seed = vec2(1.0, 1.0);

    vec2  px = (-size * 9)*(4*isf_FragNormCoord.xy);

	float id = 0.5 + 0.5*cos(mat_time + sin(dot(floor(px+0.5),vec2(113.1*seed.x,17.81)))*43758.545*seed.y);

//     vec3  co = 0.5 + 0.5*cos(mat_time + 3.5*id + vec3(0.0,1.57,3.14) );

	vec4 mutableBackgroundColor = backgroundColor.rgba;
//	mutableBackgroundColor.a -= 0.25;
//	mutableBackgroundColor.a *= 1.0 + audio_amplitude_decay * (amplitude * 10);

	mutableBackgroundColor.r *= 1.0 + audio_amplitude_decay * (amplitude * 15);
	mutableBackgroundColor.g *= 1.0 + audio_amplitude_decay * (amplitude * 15);
	mutableBackgroundColor.b *= 1.0 + audio_amplitude_decay * (amplitude * 15);

	vec4  co = mix(foregroundColor.rgba, mutableBackgroundColor.rgba, id);

    vec2  pa = smoothstep( 0.0, 0.01 + (dist / 5), id*(0.5 + 0.5*cos(6.2831*px)) );

    return applyColorMods(co*pa.x*pa.y, contrast, saturation, brightness);
}