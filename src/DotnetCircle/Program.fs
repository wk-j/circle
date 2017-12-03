// Learn more about F# at http://fsharp.org

module DotnetCircle.Program

open System
open SixLabors.ImageSharp
open SixLabors.Shapes
open SixLabors.ImageSharp.PixelFormats
open SixLabors.Primitives
open SixLabors.ImageSharp.Processing
open System.IO
open System.Net.Http
open System.ComponentModel.Design

let buildCorner width height radius = 
    let rect = RectangularePolygon(-0.5f, -0.5f, radius, radius)
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
        img.Fill(Rgba32.Transparent, corners, opt) |> ignore
    img.Mutate(fun x -> mutate(x))

let convertToAvatar(img: IImageProcessingContext<Rgba32>) (size: Size) radius = 
    let rs = img.Resize(ResizeOptions(Size = size, Mode = ResizeMode.Crop))
    rs.Apply(fun x -> applyRoundedCorners x radius)

let cloneAndConvertToAvatarWithoutApply(img: Image<Rgba32>) (size: Size) radius = 
    let result = img.Clone(fun x -> x.Resize(ResizeOptions(Size = size, Mode = ResizeMode.Crop)) |> ignore)
    applyRoundedCorners result radius

let downloadImage httpPath = 
    printfn "-- downlaod | %s" httpPath

    use client = new HttpClient()
    let rs = client.GetAsync(httpPath: string) |> Async.AwaitTask |> Async.RunSynchronously
    let content = rs.Content.ReadAsByteArrayAsync() |> Async.AwaitTask |> Async.RunSynchronously
    let uri = httpPath |> Uri
    let fileName = uri.LocalPath |> Path.GetFileName
    let targetPath = Path.Combine(Path.GetTempPath(), fileName)
    File.WriteAllBytes(targetPath, content)

    printfn "-- save as | %s" targetPath
    (targetPath)

let isUrl path = (path: string).StartsWith("http")

let processImage path = 
    use img = Image.Load(path: string)
    use round = img.Clone(fun x -> convertToAvatar x (Size(300, 300)) 150.0f |> ignore) 
    let name = Path.GetFileName(path)
    round.Save(name + ".png")

[<EntryPoint>]
let main argv =
    if argv.Length <> 1 then
        printfn "-- invalid argument"
        -1
    else 
        let org = argv.[0]
        let path = if isUrl org then downloadImage org else org
        processImage path

        // remove temp
        if isUrl org then File.Delete path
        0