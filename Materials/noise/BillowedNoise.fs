/*{
    "CREDIT": "Drew Andre",
    "DESCRIPTION": "noise",
    "TAGS": "audio,reactive,noise",
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
        {
            "LABEL": "Public/Scale",
            "NAME": "scale",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.33
        },
        {
            "LABEL": "Public/Speed",
            "NAME": "zspeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.33
        },
        {
            "LABEL": "Public/Front Color",
            "NAME": "foregroundColor",
            "TYPE": "color",
            "DEFAULT": [ 1.0, 1.0, 1.0, 1.0 ]
        },
        {
            "LABEL": "Public/Back Color",
            "NAME": "backgroundColor",
            "TYPE": "color",
            "DEFAULT": [ 0.0, 0.0, 0.0, 1.0 ]
        },
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
            "LABEL": "Private/Speed Y",
            "NAME": "yspeed",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.225
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
            "NAME": "xtime",
            "TYPE": "time_base",
            "PARAMS": {
                "speed": "xspeed",
                "speed_curve": 1,
                "bpm_sync": false,
                "link_speed_to_global_bpm": false
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
            "NAME": "ztime",
            "TYPE": "time_base",
            "PARAMS": {
                "speed": "zspeed",
                "speed_curve": 1,
                "bpm_sync": false,
                "link_speed_to_global_bpm": false
            }
        },
    ]
}*/

#define SDF_ANTIALIASING_NONE 

#include "MadCommon.glsl"
#include "MadNoise.glsl"

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

vec4 materialColorForPixel(vec2 texCoord)
{
	float internalScale = scale * 3.0;

    // Calculate UVs
    vec3 uv = vec3(vec2(0.5) + (texCoord-vec2(0.5)) * internalScale * internalScale + vec2(0.0, -ytime), ztime);

    // Simplex Noise
	// billowed noise
    float n = ridgedNoise(uv);

	vec4 mutableForegroundColor = foregroundColor.rgba;
	mutableForegroundColor = applyColorMods(mutableForegroundColor, contrast, saturation, brightness);

	mutableForegroundColor.a -= 0.25;
	mutableForegroundColor.a *= 1.0 + audio_amplitude_decay * (amplitude * 10);

	vec4 frontColor = getColor(mutableForegroundColor.rgba, backgroundColor.rgba);

	vec4 backColor  = getColor(backgroundColor.rgba, mutableForegroundColor.rgba);
	backColor = applyColorMods(backColor, contrast, saturation, 1);

    // Interpolate Color
    vec4 color      = mix(backColor.rgba, frontColor.rgba, n);
 	color *= vec4(brightness);
	return vec4(color.rgb, 1);
}
