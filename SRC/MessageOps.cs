using System.Diagnostics;
using System.Text.Json;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS.Model;

namespace AWSMessageHelper;

public static class MessageOps<T>
{
    public static string GetSNSTopicByName(string topicName)
    {
        if (Config._snsClient is null)
            throw new Exception("AWS SNS connection is not established");

        string nextToken = string.Empty;
        List<Topic> topics = new List<Topic>();

        do
        {
            var response = Config._snsClient.ListTopicsAsync(nextToken).GetAwaiter().GetResult();
            topics.AddRange(response.Topics);
            nextToken = response.NextToken;
        } while (!string.IsNullOrEmpty(nextToken));

        var topic = topics.FirstOrDefault(p => p.TopicArn.IndexOf(topicName) != -1);
        if(topic is null)
            throw new Exception("Topic not found");

        return topic.TopicArn;
    }

    public static async Task Publish(T message, string topicName)
    {
       var topicArn = GetSNSTopicByName(topicName);
         
        var request = new PublishRequest
        {
            TopicArn = topicArn,
            Message = JsonSerializer.Serialize(message)
        };

        if(Config._snsClient is null)
            throw new Exception("AWS SNS connection is not established");
        
        var response = await Config._snsClient.PublishAsync(request);

        Debug.WriteLine("Message published");
    }

    public static (T messageObject, string receiptHandle ) GetMessage(string queueName)
    {
        if(Config._client is null)
            throw new Exception("AWS SQS connection is not established");
            
        var qURL = GetQueueUrl(queueName);

        var messages = ReceiveMessageAsync(qURL).GetAwaiter().GetResult();
        
        
        if (messages.Count() is 0) 
            return (default(T), null);

        try
        {
            var messageResponseModel = JsonSerializer.Deserialize<MessageResponseModel>(messages[0].Body);

            var messageObject = JsonSerializer.Deserialize<T>(messageResponseModel.Message);
       
            return (messageObject, messages[0].ReceiptHandle);
        }
        catch (System.Exception)
        {
            throw;
        }

    }
    private static async Task<List<Message>> ReceiveMessageAsync(string queueUrl, int maxMessages = 1)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = maxMessages
        };
        if(Config._client is null)
            throw new Exception("AWS SQS connection is not established");

        var messages = await Config._client.ReceiveMessageAsync(request);
        return messages.Messages;
    }

    public static async Task DeleteMessage(string queueName, string receiptHandle)
    {
        var qURL = GetQueueUrl(queueName);
       
        var request = new DeleteMessageRequest
        {
            QueueUrl = qURL,
            ReceiptHandle = receiptHandle
        };

        await Config._client.DeleteMessageAsync(request);
    }
    public static string GetQueueUrl(string queueName)
    {
        var response = Config._client.GetQueueUrlAsync(new GetQueueUrlRequest
        {
            QueueName = queueName
        }).GetAwaiter().GetResult();

         if(response.HttpStatusCode != System.Net.HttpStatusCode.OK)
            throw new Exception("Queue not found");

        return response.QueueUrl;
    }

}