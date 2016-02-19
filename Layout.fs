module Layout

open System;
open System.Windows.Forms;

open SharpDX;
open SharpDX.DXGI;
open SharpDX.Direct3D;
open SharpDX.Direct3D11;
open SharpDX.Direct2D1;
open SharpDX.Windows;

let form = new RenderForm("test")

let CreateBackbuffer =
  let mutable device = null :> SharpDX.Direct3D11.Device
  let mutable swapChain = null :> SwapChain
  let swapChainDesc =
    new SwapChainDescription
      ( BufferCount = 2
      , Usage = Usage.RenderTargetOutput
      , OutputHandle = form.Handle
      , IsWindowed = new Mathematics.Interop.RawBool(true)
      , ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm)
      , SampleDescription = new SampleDescription(1, 0)
      , Flags = SwapChainFlags.AllowModeSwitch
      , SwapEffect = SwapEffect.Discard
      )

  //let d3d11Device = new SharpDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.Debug);
  let d3d11Device =
    new SharpDX.Direct3D11.Device
      ( DriverType.Hardware
      , DeviceCreationFlags.BgraSupport
      )

  if (d3d11Device <> null)
  then Console.WriteLine("device ok") |> ignore
  else Console.WriteLine("device shitty") |> ignore

  Console.WriteLine(d3d11Device.FeatureLevel.ToString())
  // Create swap chain and Direct3D device
  // The BgraSupport flag is needed for Direct2D compatibility otherwise RenderTarget.FromDXGI will fail!
  // Create swap chain description
  SharpDX.Direct3D11.Device.CreateWithSwapChain
    ( DriverType.Hardware
    , DeviceCreationFlags.BgraSupport
    , [|FeatureLevel.Level_11_0|]
    , swapChainDesc
    , ref device
    , ref swapChain
    ) |> ignore

  if (device <> null)
  then Console.WriteLine("device ok") |> ignore
  else Console.WriteLine("device shitty") |> ignore

  if (swapChain <> null)
  then Console.WriteLine("swapChain ok") |> ignore
  else Console.WriteLine("swapChain shitty") |> ignore

  // Get back buffer in a Direct2D-compatible format (DXGI surface)
  let backBuffer = Surface.FromSwapChain(swapChain, 0)
  device, backBuffer, swapChain

let CreateRenderTarget =
  // Create Direct2D factory
  use factory = new SharpDX.Direct2D1.Factory()
  // Get desktop DPI
  let dpi = factory.DesktopDpi;
  let device, backBuffer, swapChain = CreateBackbuffer

  // Create bitmap render target from DXGI surface
  let renderTarget =
    new RenderTarget
     ( factory
     , backBuffer
     , new RenderTargetProperties
        ( DpiX = dpi.Width
        , DpiY = dpi.Height
        , MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT
        , PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Ignore)
        , Type = RenderTargetType.Default
        , Usage = RenderTargetUsage.None
        )
      )

  renderTarget
