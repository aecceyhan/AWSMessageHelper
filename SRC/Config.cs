using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;

namespace AWSMessageHelper;
    
public static class Config
{
    public static IAmazonSQS _client { get; set; }
    public static AmazonSimpleNotificationServiceClient _snsClient { get; set; }

    public static void SetupConnection(string AccessKey, string Secret, RegionEndpoint region)
    {

        var credentials = new BasicAWSCredentials(AccessKey, Secret);

        _client = new AmazonSQSClient(credentials, region);
        if (_client is null)
            throw new Exception("AWS SQS connection is established");

        _snsClient = new AmazonSimpleNotificationServiceClient(credentials, region);
        if (_snsClient is null)
            throw new Exception("AWS SNS connection is established");

    }
}