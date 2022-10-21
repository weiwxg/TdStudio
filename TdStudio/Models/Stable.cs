using System;
using Newtonsoft.Json;

namespace TdStudio.Models;

public sealed class Stable
{
#nullable disable
    public string Name { get; set; }

    [JsonProperty("created_time")]
    public DateTime CreatedTime { get; set; }

    public int Columns { get; set; }
    public int Tags { get; set; }
    public int Tables { get; set; }
}