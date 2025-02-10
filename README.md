# BlueSky.FSharp.NET

C# 调用方法

``` C#
// See https://aka.ms/new-console-template for more information
using BlueSky.FSharp.NET;
using System.Reflection.Emit;
using static BlueSky.FSharp.NET.Bsky;

//var data = getUserTweet("https://bsky.app/profile/sarahcandersen.bsky.social/post/3l72pmuz6z322", 2);
//var data = getUserTweet("https://bsky.app/profile/piratesoftware.live/post/3l72jf5egaw2q", 2);
var data = getUserTweet("https://bsky.app/profile/13thsquad.bsky.social/post/3l72qgq7y3p2q", 2);

if (data != null)
{
    foreach (var item in data.Value.Image)
    {
        Console.WriteLine(item);
    }
    Console.WriteLine(data.Value.File);
    Console.WriteLine(data.Value.Text);

}
```