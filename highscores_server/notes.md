Bucket is named `ld52-scores982347347834`

Lambda function is named `ld52-scores`

Lambda's role is named `ld52-scores-role-9ap8jz5q`

```json
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "VisualEditor0",
            "Effect": "Allow",
            "Action": [
                "s3:DeleteObject",
                "s3:PutObject",
                "s3:ListBucket",
                "s3:GetObject"
            ],
            "Resource": "arn:aws:s3:::ld52-scores982347347834"
        }
    ]
}
```


API endpoint is:

    https://k1seztx1s2.execute-api.us-west-2.amazonaws.com/default/ld52-scores

    https://k1seztx1s2.execute-api.us-west-2.amazonaws.com/default/ld52-scores?data={%22level%22:%22level1%22,%22timeSeconds%22:55,%22completionState%22:%22goalReached%22,%22harvested%22:{%22FoodLeaf%22:1}}

