using System.Collections;
using Newtonsoft.Json;

namespace TdStudio.Taos;

public sealed class TaosRestReponse
{
    public const string DefaultResponseStatus = "succ";
    public const int DefaultResponseCode = 0;

#nullable disable
    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("desc")]
    public string Desc { get; set; }

    [JsonProperty("column_meta")]
    public ArrayList[] ColumnMeta { get; set; }

    [JsonProperty("data")]
    public ArrayList[] Data { get; set; }

    [JsonProperty("rows")]
    public int Rows { get; set; }
}