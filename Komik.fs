module Komik

open Layout

open System
open FSharp.Core
open System.IO
open System.Net
open System.Threading.Tasks
open FSharp.Data
open System.Drawing

type Card =
  { SetName : string
    Name : string
    Number : int
    Text : Option<string>
  }

type CardSet = JsonProvider<"http://mtgjson.com/json/OGW.json">
type SetCodes = JsonProvider<"http://mtgjson.com/json/SetCodes.json">

let Set (name : string) =
    "http://mtgjson.com/json/" + name + ".json" |> CardSet.Load

let GetData (location : string) =
  use wc = new WebClient()
  wc.DownloadData(location)

let ImageUrl (card : Card) =
  sprintf @"http://magiccards.info/scans/en/%s/%i.jpg" (card.SetName.ToLower()) card.Number

let CardId (card : Card) =
  sprintf @"%s_%i" card.SetName card.Number

let ImageName (card : Card) =
  CardId card |> sprintf @"./%s.jpg"

let GetCards (name : string) =
  let set = Set name
  set.Cards |> Array.map
    ( fun c ->
      { SetName = set.Code
      ; Name = c.Name
      ; Number = c.Number
      ; Text = c.Text
      }
    )

let GetImageUrls (cards : Card[]) =
  cards |> Array.map ImageUrl

let GetBitmaps (imageUrls : string[]) =
  imageUrls |> Array.map
    ( GetData >> fun i ->
      use m = new MemoryStream(i)
      new Bitmap(m)
    )

let Safe (cards : Card[]) =
  GetImageUrls cards
  |> GetBitmaps
  |> Array.zip cards
  |> Array.map (fun (c, b) -> c |> ImageName |> b.Save)
  |> ignore

[<EntryPoint>]
let main argv =
  //let cards = GetCards "OGW"
  //Safe cards
  Layout.CreateRenderTarget |> ignore
  0 // return an integer exit code
