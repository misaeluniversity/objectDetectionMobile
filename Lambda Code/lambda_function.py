{\rtf1\ansi\ansicpg1252\cocoartf2511
\cocoatextscaling0\cocoaplatform0{\fonttbl\f0\fswiss\fcharset0 Helvetica;}
{\colortbl;\red255\green255\blue255;}
{\*\expandedcolortbl;;}
\margl1440\margr1440\vieww10800\viewh8400\viewkind0
\pard\tx720\tx1440\tx2160\tx2880\tx3600\tx4320\tx5040\tx5760\tx6480\tx7200\tx7920\tx8640\pardirnatural\partightenfactor0

\f0\fs24 \cf0 # Library boto3 that enable the connection with the AWS Rekognition Service\
import boto3\
\
# Library base64 that enable the process of the image to bytes\
import base64\
\
#Initializing the AWS Rekognition service\
rekognitionService = boto3.client('rekognition')\
\
# --------------- Main handler ------------------\
def lambda_handler(event, context):\
    \
    # Initializing labels variable \
    labels = ''\
    \
    # Block try that check for possible erros in the recogniton process\
    try:\
        \
        # Retrive the image from the trigger event\
        imageToRecognize = event['body']\
        \
        # Decoding image from the message wrapper\
        imageToRecognize = base64.b64decode(imageToRecognize)\
        \
        # Removing extra data\
        imageToRecognize = str(base64.b64decode(event['body']))[7:len(str(base64.b64decode(event['body'])))-1]\
        \
        # Replacing codes from utf-8\
        imageToRecognize = imageToRecognize.replace('%2f', '/')\
        imageToRecognize = imageToRecognize.replace('%2b', '+')\
        imageToRecognize = imageToRecognize.replace('%3d', '=')\
    \
        # Encoding image to bytes\
        imageToRecognize = base64.b64decode(imageToRecognize)\
\
        # Sending image for objects recognition to AWS Rekognition Service\
        recognizedObjects = rekognitionService.detect_labels(Image = \{'Bytes':imageToRecognize\},MaxLabels=10)\
        \
        # Merging all the labels in one string\
        for label in recognizedObjects['Labels']:\
            labels = labels +' '+label['Name']\
\
    # Exception handler in case of error in the recognition process\
    except:\
        response = None\
        labels = "Error on recognition"\
    \
    # Returning json object with the success code and the labels of the objects recognized\
    return \{\
        'statusCode': 200,\
        'body': labels\
    \}\
}