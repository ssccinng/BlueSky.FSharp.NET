namespace BlueSky.FSharp.NET
open FSharp.Data
module Bsky =
    [<Literal>] 
    let postThreadUrl = "https://public.api.bsky.app/xrpc/app.bsky.feed.getPostThread?uri=%s&depth=%d"
    [<Literal>] 
    //let postData = "at%%3A%%2F%%2Fpiratesoftware.live%%2Fapp.bsky.feed.post%%2F%s"
    let postData = "at://%s/app.bsky.feed.post/%s"
    type Tweet = { 
    //Id: string; 
    Text: string; 
    Title: string;
    Image: string array 
    File: string
    DisplayName: string
    }
    let getHtml (url: string) =
        url |> HtmlDocument.Load

    let getUrlId (url: string) =
        url.Split("/") |> Array.last

    let getUrlUser (url: string) =
        url.Split("/") |> Array.item 4

    //[<Literal>] 
    //let sample = "https://public.api.bsky.app/xrpc/app.bsky.feed.getPostThread?uri=at%3A%2F%2Fpiratesoftware.live%2Fapp.bsky.feed.post%2F3l72jf5egaw2q&depth=10"

    type BskyThread = JsonProvider<"./api.json", SampleIsList=true>


    let getPostThread (url: string) (depth: int) : Tweet array=
        let id = getUrlId url
        let user = getUrlUser url
        let url = sprintf postThreadUrl (sprintf postData user id) depth
        let test = BskyThread.Load(url)

        let getVideoUrl (data: JsonValue) =
            
            if data.TryGetProperty("external") |> Option.isSome then
                data.GetProperty("external").GetProperty("uri").AsString()
            else
                ""
        let getImages (data: JsonValue) =
            if data.TryGetProperty("images") |> Option.isSome then
                data.GetProperty("images").AsArray() |> Array.map (fun x -> x.GetProperty("fullsize").AsString())
            else
                [||]
        
        let main = if test.Thread.Post.JsonValue.TryGetProperty("embed") |> Option.isSome then
                        {
                            Text = test.Thread.Post.Record.Text 
                            Title = test.Thread.Post.Uri |> getUrlId
                            Image = getImages ( match test.Thread.Post.Embed with | Some x -> x.JsonValue | None -> JsonValue.Null)
                            File = getVideoUrl ( match test.Thread.Post.Embed with | Some x -> x.JsonValue | None -> JsonValue.Null)
                            DisplayName = test.Thread.Post.Author.DisplayName
                        }
                    else
                        {
                            Text = test.Thread.Post.Record.Text 
                            Title = test.Thread.Post.Uri |> getUrlId
                            Image = [||]
                            File = ""
                            DisplayName = test.Thread.Post.Author.DisplayName
                        }

        
        let reply = 
            test.Thread.Replies |> Seq.map (fun x -> 
                {
                    Text = x.Post.Record.Text |> Option.defaultValue ""
                    Title = x.Post.Uri |> getUrlId
                    Image = match x.Post.Embed  with
                                | Some x -> x.Images |> Array.map (fun s -> s.Fullsize)
                                | None -> [||]
                    File = getVideoUrl (match x.Post.Embed with
                                        | Some x -> x.JsonValue
                                        | None -> JsonValue.Null)
                    DisplayName = x.Post.Author.DisplayName |> Option.defaultValue ""


                }) |> List.ofSeq
        main :: reply |> List.toArray

    let getUserTweet (url: string) (depth: int) : Tweet option =
        let id = getUrlId url
        let user = getUrlUser url
        let postThread = getPostThread url depth
        postThread |> Array.tryFind (fun x -> x.Title = id)
                    


    let getHtmlAsync (url: string) =
        async {
            let! html = HtmlDocument.AsyncLoad(url)
            return html
        }
    
    let getMetas (html: HtmlDocument) =
        html.Descendants ["meta"]

    let getMetaImages (html: HtmlDocument) = 
        html.Descendants ["meta"]
        |> Seq.filter (fun x -> x.AttributeValue("property") = "og:image")
        |> Seq.map (fun x -> x.AttributeValue("content"))
            


    let getTweetByUrl (url: string) =
        let html = getHtml url
        ()




    let getBskyImage = getHtml >> getMetaImages