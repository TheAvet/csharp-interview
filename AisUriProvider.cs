using System;
using System.Collections.Generic;
using System.Linq;

public class AisUriProvider
{
    private static readonly List<string> FileNames = new List<string>()
    {
        "1.mp3",
        "10.jpg",
        "11.jpg",
        "12.html",
        "13.css",
        "14.doc",
        "15.xls",
        "16.js",
        "2.mp4",
        "3.jpg",
        "4.jpg",
        "5.jpg",
        "6.jpg",
        "7.jpg",
        "8.jpg",
        "9.jpg"
    };

    public IEnumerable<Uri> Get()
    {
        Random randomGenerator = new Random();
        return AisUriProvider.FileNames.OrderBy<string, int>((Func<string, int>)(_ => randomGenerator.Next())).Take<string>(10).Select<string, Uri>((Func<string, Uri>)(p => new Uri("https://bitstore.blob.core.windows.net/test/" + p)));
    }
}
