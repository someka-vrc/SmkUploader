###   
Upload

画像をアップロードするAPI。

##### NOTICE

*   multipart/form-data を使うこと
*   URLが他のAPIとは異なるため注意

##### URL

POST https://upload.gyazo.com/api/upload

##### parameters

| Key                | Type                     | Required | Default | Remarks                                                                                                                                                                  |
| ------------------ | ------------------------ | -------- | ------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| access_token       | string                   | ◯        |         | ユーザーのアクセストークン                                                                                                                                               |
| imagedata          | binary                   | ◯        |         | multipart/form-dataのContent-Dispositionにおける`filename`を忘れずに付与してください。`filename`は必須です。                                                             |
| access_policy      | string anyone or only_me |          | anyone  | 画像の公開範囲を指定する文字列。 デフォルトは anyone で、リンクを知っている全員が閲覧可能です。 only_me を指定すると、アップロードしたユーザーのみが閲覧可能になります。 |
| metadata_is_public | string 'true' or 'false' |          |         | URLやタイトルなどのメタデータを公開するか否かの真偽値の文字列                                                                                                            |
| referer_url        | string                   |          |         | キャプチャをしたサイトのURL                                                                                                                                              |
| app                | string                   |          |         | キャプチャをしたアプリケーション名                                                                                                                                       |
| title              | string                   |          |         | キャプチャをしたサイトのタイトル                                                                                                                                         |
| desc               | string                   |          |         | 任意のコメント                                                                                                                                                           |
| created_at         | float                    |          |         | 画像の作られた日時（Unix time）                                                                                                                                          |
| collection_id      | string                   |          |         | ユーザーが所有している/参加しているコレクションにのみ追加できます                                                                                                        |

##### response

{
   "image\_id" : "8980c52421e452ac3355ca3e5cfe7a0c",
   "permalink\_url": "http://gyazo.com/8980c52421e452ac3355ca3e5cfe7a0c",
   "thumb\_url" : "https://i.gyazo.com/thumb/180/afaiefnaf.png",
   "url" : "https://i.gyazo.com/8980c52421e452ac3355ca3e5cfe7a0c.png",
   "type": "png"
}