Task("Publish").Does(() => {
    DotNetCorePublish("src/DotnetCircle/DotnetCircle.fsproj", new DotNetCorePublishSettings {
        OutputDirectory = "./publish"
    });
});

var target = Argument("target", "default");
RunTarget(target);