import * as s3 from '@aws-sdk/client-s3';

//const s3 = new aws.S3({apiVersion: "2006-03-01"});

const BUCKET_NAME = "ld52-scores982347347834";
const KEY_NAME = "scores.json";


export const handler = async (event) => {
    let object = {};

    try {
        object = await s3.getObject({
            Bucket: BUCKET_NAME,
            Key: KEY_NAME,
        });
    } catch (err) { }

    const response = {
        statusCode: 200,
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ data: object }),
    };
    return response;
};
