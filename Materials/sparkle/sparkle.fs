/*{
    "CREDIT": "Drew Andre",
    "DESCRIPTION": "sparkle",
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
            "DEFAULT": 0.167
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
            "DEFAULT": [ 0.0, 0.0, 1.0, 1.0 ]
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
			"DEFAULT": 0
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
            "LABEL": "Private/Offset X",
            "NAME": "offsetX",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 1.0,
            "DEFAULT": 0.040
        },
        {
            "LABEL": "Private/Size",
            "NAME": "size",
            "TYPE": "float",
            "MIN": 0.0,
            "MAX": 0.05,
            "DEFAULT": 0 // 0.007
        },
        {
            "LABEL": "Private/Step",
            "NAME": "stepper",
            "TYPE": "int",
            "MIN": 1,
            "MAX": 5,
            "DEFAULT": 1
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
#include "MadSDF.glsl"

vec4 getColor(vec4 colorRGBA1, vec4 colorRGBA2) {
    float alpha = 1.0 - ((1.0 - colorRGBA1.a) * (1.0 - colorRGBA2.a) / 1.0);
    float red   = (colorRGBA1.r * (1.0 - colorRGBA2.a) + colorRGBA2.r * colorRGBA2.a) / 1.0;
    float green = (colorRGBA1.g * (1.0 - colorRGBA2.a) + colorRGBA2.g * colorRGBA2.a) / 1.0;
    float blue  = (colorRGBA1.b * (1.0 - colorRGBA2.a) + colorRGBA2.b * colorRGBA2.a) / 1.0;
    return vec4(red, green, blue, alpha);
}

float easeInOutQuart(float x) {
	return x < 0.5 ? 8 * x * x * x * x : 1 - pow(-2 * x + 2, 4) / 2;
}

vec4 applyColorMods(vec4 color, float c, float s, float b) {
	float a = color.a;
	vec3 output_color = applyContrastSaturationBrightness(color.rgb, c + 1, s + 1, b + 1);
	return vec4(output_color.r, output_color.g, output_color.b, a);
}

vec4 materialColorForPixel(vec2 texCoord)
{
	float internalScale = 0.5 + (scale * 2.0);
	float internalYOffset = ytime;

    vec2 cellId;
    vec2 p              = texCoord;
    vec2 brickSize      = vec2(0.1, 0.1) * internalScale;
	const float offset  = 0.5;

	texCoord.y -= internalYOffset;

    if(mod(texCoord.y / brickSize.y, stepper) < 1){
        p.x += (offset * brickSize.x);
    }

	p.x += offsetX;
	p.y -= internalYOffset;

    p = repeat(p, brickSize, vec2(0), vec2(2), cellId);
    float dist = rectangle(p, brickSize * 0.5);

	vec4 mutableBackgroundColor = backgroundColor.rgba;

	vec4 fillColor = foregroundColor * billowedNoise(vec3(cellId, ztime));

	fillColor.r *= 1.0 + (1.0 * audio_amplitude_decay * (amplitude * 15));
	fillColor.g *= 1.0 + (1.0 * audio_amplitude_decay * (amplitude * 15));
	fillColor.b *= 1.0 + (1.0 * audio_amplitude_decay * (amplitude * 15));
	fillColor.a = easeInOutQuart(fillColor.a);

    vec4 color = fill(vec4(0), getColor(mutableBackgroundColor.rgba, fillColor.rgba), dist);
	color = stroke(color, mutableBackgroundColor.rgba, dist, size);

	color = applyColorMods(color, 0.5, saturation, 0);
 	color *= vec4(brightness);
	return vec4(color.rgb, 1);
}
