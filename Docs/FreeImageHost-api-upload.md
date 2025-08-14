
# API version 1

Freeimage.host's API v1 allows to upload pictures.

## API call

### Request method

API v1 calls can be done using the POST or GET request methods but since GET request are limited by the maximum allowed length of an URL you should prefer the POST request method.

### Request URL

`https://freeimage.host/api/1/upload`

### Parameters

*   key (required): The API key.
*   action: What you want to do \[values: upload\].
*   source: Either a image URL or a base64 encoded image string. You can also use FILES\["source"\] in your request.
*   format: Sets the return format \[values: json (default), redirect, txt\].

### Example call

`GET http://freeimage.host/api/1/upload/?key=12345&source=http://somewebsite/someimage.jpg&format=json`

Note: Always use POST when uploading local files. Url encoding may alter the base64 source due to encoded characters or just by URL request length limit due to GET request.

## API response

API v1 responses display all the image uploaded information in JSON format.

JSON the response will have headers status codes to allow you to easily notice if the request was OK or not. It will also output the `status` properties.

### Example response (JSON)

```json
{
        "status_code": 200,
        "success": {
            "message": "image uploaded",
            "code": 200
        },
        "image": {
            "name": "example",
            "extension": "png",
            "size": 53237,
            "width": 1151,
            "height": 898,
            "date": "2014-06-04 15:32:33",
            "date_gmt": "2014-06-04 19:32:33",
            "storage_id": null,
            "description": null,
            "nsfw": "0",
            "md5": "c684350d722c956c362ab70299735830",
            "storage": "datefolder",
            "original_filename": "example.png",
            "original_exifdata": null,
            "views": "0",
            "id_encoded": "L",
            "filename": "example.png",
            "ratio": 1.2817371937639,
            "size_formatted": "52 KB",
            "mime": "image/png",
            "bits": 8,
            "channels": null,
            "url": "http://freeimage.host/images/2014/06/04/example.png",
            "url_viewer": "http://freeimage.host/image/L",
            "thumb": {
                "filename": "example.th.png",
                "name": "example.th",
                "width": 160,
                "height": 160,
                "ratio": 1,
                "size": 17848,
                "size_formatted": "17.4 KB",
                "mime": "image/png",
                "extension": "png",
                "bits": 8,
                "channels": null,
                "url": "http://freeimage.host/images/2014/06/04/example.th.png"
            },
            "medium": {
                "filename": "example.md.png",
                "name": "example.md",
                "width": 500,
                "height": 390,
                "ratio": 1.2820512820513,
                "size": 104448,
                "size_formatted": "102 KB",
                "mime": "image/png",
                "extension": "png",
                "bits": 8,
                "channels": null,
                "url": "http://freeimage.host/images/2014/06/04/example.md.png"
            },
            "views_label": "views",
            "display_url": "http://freeimage.host/images/2014/06/04/example.md.png",
            "how_long_ago": "moments ago"
        },
        "status_txt": "OK"
    }
}
```