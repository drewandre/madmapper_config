/*{
    "CREDIT": "Drew Andre",
    "DESCRIPTION": "Solid Color",
    "TAGS": "graphic",
    "VSN": "1.0",
	"RASTERISATION_SETTINGS": {
	    "DEFAULT_RENDER_TO_TEXTURE": true,
	    "DEFAULT_WIDTH": 1024,
	    "DEFAULT_HEIGHT": 1024,
		"REQUIRES_LAST_FRAME": true,
	},
    "INPUTS": [ 
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
			"MIN": 0,
			"MAX": 1.0,
			"DEFAULT": 0.5
		},
    ]
}*/

vec4 materialColorForPixel( vec2 texCoord )
{
	vec3 mutableForegroundColor = foregroundColor.rgb;
	mutableForegroundColor *= vec3(brightness);
	return vec4(mutableForegroundColor.rgb, 1);
}