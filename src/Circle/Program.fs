﻿// Learn more about F# at http://fsharp.org

module DotnetCircle.Program

open System
open SixLabors.ImageSharp
open SixLabors.Shapes
open SixLabors.ImageSharp.PixelFormats
open SixLabors.Primitives
open SixLabors.ImageSharp.Processing
open System.IO
open System.Net.Http
open Kurukuru
open SixLabors.ImageSharp.Processing.Transforms
open SixLabors.ImageSharp.Processing.Drawing

let buildCorner width height radius =
    let rect = RectangularPolygon(-0.5f, -0.5f, radius, radius)
    let cornerToptLeft = rect.Clip(EllipsePolygon(radius - 0.5f, radius - 0.5f, radius))
    let rightPos = float32 width - cornerToptLeft.Bounds.Width + 1.0f
    let bottomPos = float32 height - cornerToptLeft.Bounds.Height + 1.0f
    let cornerTopRight = cornerToptLeft.RotateDegree(90.0f).Translate(rightPos, 0.0f);
    let cornerBottomLeft = cornerToptLeft.RotateDegree(-90.0f).Translate(0.0f, bottomPos);
    let cornerBottomRight = cornerToptLeft.RotateDegree(-180.0f).Translate(rightPos, bottomPos);

    PathCollection(cornerToptLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight)

let applyRoundedCorners (img: Image<Rgba32>) radius =
    let corners = buildCorner img.Width img.Height radius
    let mutate (img: IImageProcessingContext<Rgba32>) =
        let opt = GraphicsOptions(true, BlenderMode = PixelBlenderMode.Src)
        // img.Fill(Rgba32.Transparent, corners, opt) |> ignore
        img.Fill(opt,Rgba32.Transparent, corners) |> ignore
        // img.Fill(Rgba32.Transparent) |> ignore
    img.Mutate(fun x -> mutate(x))

let convertToAvatar(img: IImageProcessingContext<Rgba32>) (size: Size) radius =
    let rs = img.Resize(ResizeOptions(Size = size, Mode = ResizeMode.Crop))
    rs.Apply(fun x -> applyRoundedCorners x radius)

let cloneAndConvertToAvatarWithoutApply(img: Image<Rgba32>) (size: Size) radius =
    let result = img.Clone(fun x -> x.Resize(ResizeOptions(Size = size, Mode = ResizeMode.Crop)) |> ignore)
    applyRoundedCorners result radius

let downloadImage httpPath =
    Spinner.Start("Download file => " + httpPath, fun () -> ())

    use client = new HttpClient()
    let rs = client.GetAsync(httpPath: string) |> Async.AwaitTask |> Async.RunSynchronously
    let content = rs.Content.ReadAsByteArrayAsync() |> Async.AwaitTask |> Async.RunSynchronously
    let uri = httpPath |> Uri
    let fileName = uri.LocalPath |> Path.GetFileName
    let targetPath = Path.Combine(Path.GetTempPath(), fileName)
    File.WriteAllBytes(targetPath, content)

    Spinner.Start("Create local temporary => " + targetPath, fun () -> ());

    (targetPath)

let isUrl path = (path: string).StartsWith("http")

let processImage path width height =
    use img = Image.Load(path: string)
    let haft = width / 2 |> float32
    use round = img.Clone(fun x -> convertToAvatar x (Size(width, height)) haft |> ignore)
    let name = Path.ChangeExtension(Path.GetFileName(path), ".png")
    round.Save(name)
    (name)

let processWithSize size org =
    let path = if isUrl org then downloadImage org else org
    Spinner.Start("Processing", fun (spinner: Spinner) ->
        let name = processImage path size size
        spinner.Text <- "Write output => " + name
    )

    Spinner.Start("Clear local temporary => " + path, fun () ->
        if isUrl org then File.Delete path
    )

[<EntryPoint>]
let main argv =
    match argv with
    | [|org|] ->
        processWithSize 200 org
    | [|sizeText ; org|] ->
        let ok, size = Int32.TryParse sizeText
        if ok then
            processWithSize size org
    | _ ->
            Spinner.Start("Invalid argument", fun (spinner: Spinner) ->
                spinner.Fail()
            )
    0