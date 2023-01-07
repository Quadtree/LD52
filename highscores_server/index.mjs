import * as AWS from '@aws-sdk/client-s3';

const s3 = new AWS.S3({ region: "us-west-2" });

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

    console.log(s3.getObject);

    try {
        object = await s3.getObject({
            Bucket: BUCKET_NAME,
            Key: KEY_NAME,
        });
    } catch (err) { }

    if (typeof (object.levelScores[incomingData.level]) === "undefined") object.levelScores[incomingData.level] = [];

    object.levelScores[incomingData.level].push(incomingData);

    await s3.putObject({
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
