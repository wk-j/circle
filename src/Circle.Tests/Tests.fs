module Tests

open System
open Xunit
open DotnetCircle.Program
open System.IO

//[<Fact>]
let shouldDownloadImage() = 
    let path = "https://pbs.twimg.com/profile_images/933785720451342336/D4TWqrCW_400x400.jpg"
    downloadImage path

[<Fact>]
let shouldExtractFileName() = 
    let path = "http://p3.music.126.net/HbTCYoO60AUaJFZG6k3XfA==/3242459794792202.jpg?param=640y300"
    let uri = Uri path
    let fileName = uri.LocalPath |> Path.GetFileName
    Assert.Equal("3242459794792202.jpg", fileName)
