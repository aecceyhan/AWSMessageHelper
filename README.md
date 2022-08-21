# AWS Message Helper
Message Helper for AWS SNS/SQS. This nuget pack helps send and receive messages easily on AWS.


## Installing

#### Package Manager
```shell
  Install-Package AWSMessageHelper
```

#### .NET CLI
```shell
  dotnet add package AWSMessageHelper
```

### Using

```cs
  using AWSMessageHelper;
```

### Setup AWS Credentials

```cs
  AWSMessageHelper.Config.SetupConnection(AccessKey, Secret, RegionEndpoint.EUCentral1);
```

### Send a message to SNS Topic

```cs
  DemoMessageModel message = new DemoMessageModel(){
            UserId = "123",
            UserName = "Emre Ceyhan",
            Email = "junk@emreceyhan.com"};


    await AWSMessageHelper.MessageOps<DemoMessageModel>.Publish(message, "discount-found-sns");
```


### Receive SQS message

```cs
    var message = AWSMessageHelper.MessageOps<DemoMessageModel>.GetMessage("notification-listen");
```




## TODO

* Add more examples
* Publish as a nuget pack



## Licensing

This project is licensed under MIT license. 
