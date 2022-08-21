using System.Diagnostics;
using Amazon;
using Xunit;

namespace AWSMessageHelper.Test;

public class BasicTests{
    public static string AccessKey ="accessKey";
    public static string Secret ="secret";

    [Fact]
    public static void Should_SetupConnection(){
        AWSMessageHelper.Config.SetupConnection(AccessKey, Secret, RegionEndpoint.EUCentral1);
    }

    [Fact]
    public async static void Should_PublishMessage(){
        AWSMessageHelper.Config.SetupConnection(AccessKey, Secret, RegionEndpoint.EUCentral1);
        DemoMessageModel message = new DemoMessageModel(){
            UserId = "123",
            UserName = "Emre Ceyhan",
            Email = "junk@emreceyhan.com"};


        await AWSMessageHelper.MessageOps<DemoMessageModel>.Publish(message, "discount-found-sns");
    }

    [Fact]
    public async static void Should_ReceiveMessage(){
        AWSMessageHelper.Config.SetupConnection(AccessKey, Secret, RegionEndpoint.EUCentral1);
        var o = AWSMessageHelper.MessageOps<DemoMessageModel>.GetMessage("notification-listen");

        Assert.NotNull(o);

        await AWSMessageHelper.MessageOps<DemoMessageModel>.DeleteMessage("notification-listen", o.receiptHandle);

    }
}