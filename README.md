# EnterpriseLibrary.TransientFaultHandling.Core
Transient Fault Handling Application Block/Retry patterns for .NET Core / .NET Standard.

[![Build status](https://ci.appveyor.com/api/projects/status/0abc1rtf8qcmyb97?svg=true)](https://ci.appveyor.com/project/Dixin/enterpriselibrary-transientfaulthandling-core)

TransientFaultHandling.Core is retry library for transient error handling. It is ported from Microsoft Enterprise Library’s Transient Fault Handling Application Block, a library widely used with .NET Framework. 

- The retry pattern APIs are ported to .NET 6 & .NET 5 & .NET Core & .NET Standard.
- New functional and fluent APIs to easily implement retry logic.
- The outdated configuration & APIs are replaced by modern .NET JSON/XML/INI configuration.

## Introduction

With this library, the old code with retry logic based on Microsoft Enterprise Library can be ported to .NET 6 & .NET 5 & .NET Core & .NET Standard without modification:
```cs
ITransientErrorDetectionStrategy transientExceptionDetection = new MyDetection();
RetryStrategy retryStrategy = new FixedInterval(retryCount: 5, retryInterval: TimeSpan.FromSeconds(1));
RetryPolicy retryPolicy = new RetryPolicy(transientExceptionDetection, retryStrategy);
retryPolicy.ExecuteAction(() => webClient.DownloadString("https://DixinYan.com"));
```

With this library, it is extremely easy to detect transient exception and implement retry logic. For example, the following code downloads a string, if the exception thrown is transient (a WebException), it retries up to 5 times, with it waits for 1 second between each retry:
```cs
Retry.FixedInterval(
    () => webClient.DownloadString("https://DixinYan.com"),
    isTransient: exception => exception is WebException,
    retryCount: 5, retryInterval: TimeSpan.FromSeconds(1));
```
Fluent APIs are also provided for even better readability: 
```cs
Retry
    .WithIncremental(retryCount: 5, initialInterval: TimeSpan.FromSeconds(1),
        increment: TimeSpan.FromSeconds(1))
    .Catch<OperationCanceledException>()
    .Catch<WebException>(exception =>
        exception.Response is HttpWebResponse { StatusCode: HttpStatusCode.RequestTimeout })
    .ExecuteAction(() => webClient.DownloadString("https://DixinYan.com"));
```

It also supports JSON/XML/INI configuration:
```js
{
  "retryStrategy": {
    "name1": {
      "fastFirstRetry": "true",
      "retryCount": 5,
      "retryInterval": "00:00:00.1"
    },
    "name2": {
      "fastFirstRetry": "true",
      "retryCount": 55,
      "initialInterval": "00:00:00.2",
      "increment": "00:00:00.3"
    }
  }
}
```

## Document

https://weblogs.asp.net/dixin/transientfaulthandling-core-retry-library-for-net-core-net-standard

## Source

https://github.com/Dixin/EnterpriseLibrary.TransientFaultHandling.Core (Partially ported from https://github.com/MicrosoftArchive/transient-fault-handling-application-block, with additional new APIs and redesigned/reimplemented APIs)

## NuGet installation

It can be installed through NuGet using .NET CLI:

```
dotnet add package EnterpriseLibrary.TransientFaultHandling.Core
dotnet add package TransientFaultHandling.Caching
dotnet add package TransientFaultHandling.Configuration
dotnet add package TransientFaultHandling.Data
```
Or in Visual Studio NuGet Package Manager Console:

```
Install-Package EnterpriseLibrary.TransientFaultHandling.Core
Install-Package TransientFaultHandling.Caching
Install-Package TransientFaultHandling.Configuration
Install-Package TransientFaultHandling.Data
```
