using System.Runtime.Diagnostics;
using System.Diagnostics;

Task("Publish").Does(() => {
    DotNetCorePublish("src/DotnetCircle/DotnetCircle.fsproj", new DotNetCorePublishSettings {
        OutputDirectory = "./publish/dotnet-circle",
        Configuration = "Release"
    });
});

Task("Zip")
    .IsDependentOn("Publish")
    .Does(() => {
        Zip("publish/dotnet-circle", "publish/dotnet-circle.0.3.0.zip");
    });


var target = Argument("target", "default");
RunTarget(target);

/* 
cake build.cake -target=Zip
*/