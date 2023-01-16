/*{
    "CREDIT": "Do something cool",
    "DESCRIPTION": "describe your material here",
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
    ]
}*/

vec4 materialColorForPixel( vec2 texCoord )
{
	return foregroundColor.rgba;
}