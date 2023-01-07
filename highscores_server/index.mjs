import { S3Client as s3 } from '@aws-sdk/client-s3';

//const s3 = new aws.S3({apiVersion: "2006-03-01"});

const BUCKET_NAME = "ld52-scores982347347834";
const KEY_NAME = "scores.json";


export const handler = async (event) => {

    if (!event.queryStringParameters.data) throw new Error();

    const incomingData = JSON.parse(event.queryStringParameters.data);

    if (!incomingData.level) throw new Error();
    if (!incomingData.timeSeconds) throw new Error();

    let object = {
        levelScores: {}
    };

    try {
        object = await s3.getObject({
            Bucket: BUCKET_NAME,
            Key: KEY_NAME,
        });
    } catch (err) { }

    if (typeof (object.levelScores[incomingData.level]) === "undefined") object.levelScores[incomingData.level] = [];

    object.levelScores[incomingData.level].push(incomingData);

    await s3.upload({
        Bucket: BUCKET_NAME,
        Key: KEY_NAME,
        Body: JSON.stringify(object),
    });

    const response = {
        statusCode: 200,
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ scoresForThisLevel: object.levelScores[incomingData.level] }),
    };
    return response;
};
