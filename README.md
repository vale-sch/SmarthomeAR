# SmartHome-AR

## General infos
Unity version 2021.3.22 LTS is used.  
The detailed project documentation can be found [here](Documentation/Docs/Documenation.md).

## Project structure
Unity project files can be found in the folders [Assets/](Assets/), [ProjectSettings/](ProjectSettings/) and [Packages/](Packages/).  
General documentation of the whole project can be found under [Documentation/](Documentation/) (e.g. presentations).

## QR Codes
### JSON Data Structure Schema
**In beautiful:**
```json
{
    "header": "SHQR",
    "id": "<id>",
    "type": "<Photo|Video>",
    "url": "https://...",
    "parameters": ["<key>:<param>", ...]
}
```
The `header` is used as a verification and has to be present in supported QR codes.

**In ugly but *efficient:***
```json
{"header":"SHQR","id":"<id>","type":"<Photo|Video>","url":"https://...","parameters":["<key>:<param>",...]}
```

### Type: Photo
Photos consist of 1 or 2 photo QR codes. In the case of 2 they reference each other. This way the size of the photo frame can be determined. The first QR code should be in the top left corner and the other one opposite in the bottom right corner. When only 1 QR code is used, the size of the photo frame is equal to the QR code size.  
Usually, photos are loaded from the web via `url`. If photos should be loaded locally place them into the StreamingAssets folder of the Unity project or build.

`"id": "photo#<a|b>"` (recommended schema)  
`"type": "Photo"`  
`"url": "https://..."` (optional: if not set a placeholder is used and a photo can be selected via UI)  
`parameters:`  
* `link_id:<id>` optional (default: null): `id` of the other photo QR code on the opposite side of the image (if 2 QR codes are used)
* `opposite_origin:<true|false>` optional (default: false): if `true` the origin of the QR code is in the bottom right corner instead of top left (e.g. used for second QR code)
* `width:<cm_width>` optional (default: QR code size induced): if set the width of the photo is predefined and not based on the QR code size. Specifiy the width in centimeters, e.g. `width:50` for a width of 50 centimeters. The photo will expand in its aspect ratio from the origin of the QR code as an anchor.


Examples in beatiful:
```json
{
    "header": "SHQR",
    "id": "photo1",
    "type": "Photo",
    "url": "https://www.hdm-stuttgart.de/centralImages/logo_150.jpg"
}

{
    "header": "SHQR",
    "id": "photo1a",
    "type": "Photo",
    "url": "https://www.hdm-stuttgart.de/centralImages/logo_150.jpg",
    "parameters": ["link_id:photo1b"]
}

{
    "header": "SHQR",
    "id": "photo1b",
    "type": "Photo",
    "url": "https://www.hdm-stuttgart.de/centralImages/logo_150.jpg",
    "parameters": [
        "link_id:photo1a",
        "opposite_origin:true"
    ]
}

{
    "header": "SHQR",
    "id": "photo2",
    "type": "Photo",
    "url": "https://www.hdm-stuttgart.de/centralImages/logo_150.jpg",
    "parameters": [
        "opposite_origin:true",
        "width:50"
    ]
}
```

Examples in ugly but efficient:
```json
{"header":"SHQR","id":"photo1","type":"Photo","url":"https://www.hdm-stuttgart.de/centralImages/logo_150.jpg"}
```
```json
{"header":"SHQR","id":"photo1a","type":"Photo","url":"https://www.hdm-stuttgart.de/centralImages/logo_150.jpg","parameters":["link_id:photo1b"]}
```
```json
{"header":"SHQR","id":"photo1b","type":"Photo","url":"https://www.hdm-stuttgart.de/centralImages/logo_150.jpg","parameters":["link_id:photo1a","opposite_origin:true"]}
```
```json
{"header":"SHQR","id":"photo2","type":"Photo","url":"https://www.hdm-stuttgart.de/centralImages/logo_150.jpg","parameters":["opposite_origin:true","width:50"]}
```

### Type: Video
In contrast to photos, videos require the `url` parameter, since there is no local loading supported.

`"id": "video#"` (recommended schema)  
`"type": "Video"`  
`"url": "https://..."`  
`parameters:`  
* `opposite_origin:<true|false>` optional (default: false): if `true` the origin of the QR code is in the bottom right corner instead of top left
* `width:<cm_width>` optional (default: QR code size induced): if set the width of the video is predefined and not based on the QR code size. Specifiy the width in centimeters, e.g. `width:50` for a width of 50 centimeters. The video will expand in its aspect ratio from the origin of the QR code as an anchor.


Examples in beatiful:
```json
{
    "header": "SHQR",
    "id": "video1",
    "type": "Video",
    "url": "https://www.pexels.com/de-de/download/video/3194277/"
}

{
    "header": "SHQR",
    "id": "video2",
    "type": "Video",
    "url": "https://www.pexels.com/de-de/download/video/3194277/",
    "parameters": [
        "opposite_origin:true",
        "width:50"
    ]
}
```

Examples in ugly but efficient:
```json
{"header":"SHQR","id":"video1","type":"Video","url":"https://www.pexels.com/de-de/download/video/3194277/"}
```
```json
{"header":"SHQR","id":"video2","type":"Video","url":"https://www.pexels.com/de-de/download/video/3194277/","parameters":["opposite_origin:true","width:50"]}
```

### QR Code Generation
Via the following website QR codes can be generated and downloaded:  
https://www.the-qrcode-generator.com/

* Select *Free Text* and enter the JSON
* The color should have a high contrast to its background for the best recognition

Example:  
<img alt="QR code example" src="./Documentation/Images/QR_Code_Example.png" width="256px" />
