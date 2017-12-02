// Learn more about F# at http://fsharp.org

open System
open SixLabors.ImageSharp
open SixLabors.Shapes
open SixLabors.ImageSharp.PixelFormats
open SixLabors.Primitives
open SixLabors.ImageSharp.Processing

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

[<EntryPoint>]
let main argv =
    use img = Image.Load("images/jw.jpg")
    use round = img.Clone(fun x -> convertToAvatar x (Size(200, 200)) 20.0f |> ignore) 
    round.Save("images/output.png")
    0 // return an integer exit code
