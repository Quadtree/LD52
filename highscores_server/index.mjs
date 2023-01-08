import * as AWS from '@aws-sdk/client-s3';

const s3 = new AWS.S3({ region: "us-west-2" });

const BUCKET_NAME = "ld52-scores982347347834";


export const handler = async (event) => {

    if (!event.queryStringParameters.data) throw new Error();

    const incomingData = JSON.parse(event.queryStringParameters.data);

    if (!incomingData.level) throw new Error();
    if (!incomingData.timeSeconds) throw new Error();

    let object = {
        levelScores: {}
    };

    console.log(s3.getObject);

    const keyName = `${incomingData.level}.json`;

    if (/[^A-Za-z0-9_.]/.exec(keyName)) throw new Error();
    if (keyName.length > 20) throw new Error();

    try {
        const response = await s3.getObject({
            Bucket: BUCKET_NAME,
            Key: keyName,
        });
        const rawText = await response.Body.toArray();
        console.log(rawText);
        object = JSON.parse(rawText);
    } catch (err) {
        console.log(err);
    }

    if (typeof (object.levelScores[incomingData.level]) === "undefined") object.levelScores[incomingData.level] = [];

    object.levelScores[incomingData.level].push(incomingData);

    await s3.putObject({
        Bucket: BUCKET_NAME,
        Key: keyName,
        Body: JSON.stringify(object),
    });

    object.levelScores[incomingData.level].sort((a, b) => a.timeSeconds - b.timeSeconds)

    const response = {
        statusCode: 200,
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ scoresForThisLevel: object.levelScores[incomingData.level].slice(0, 10) }),
    };
    return response;
};
