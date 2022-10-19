using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Pathoschild.Http.Client;

namespace TdStudio.Taos;

public sealed class TaosConnector
{
    private readonly string _userName;
    private readonly string _password;

    private readonly Type[] _taosDataTypes =
    {
        typeof(object),
        typeof(bool),
        typeof(byte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(string),
        typeof(DateTime),
        typeof(string)
    };

    public TaosConnector(string connectionString)
    {
        var uri = new Uri(connectionString);
        var query = HttpUtility.ParseQueryString(uri.Query);
        Host = uri.Host;
        Port = (ushort)uri.Port;
        _userName = query.Get("username")!;
        _password = query.Get("password")!;
        DbName = query.Get("database")!;
    }

    public string Host { get; }
    public ushort Port { get; }
    public string DbName { get; }

    public async Task<List<T>?> ToListAsync<T>(string sql)
    {
        var client = new FluentClient();
        try
        {
            var res = await client.SetBasicAuthentication(_userName, _password)
                .PostAsync($"http://{Host}:{Port}/rest/sql/{DbName}")
                .WithBody(new StringContent(sql))
                .As<TaosRestReponse>();

            Check(res, sql);

            return MapTo<List<T>>(res.ColumnMeta, res.Data);
        }
        catch (ApiException ex)
        {
            throw new Exception($"The API responded with HTTP {ex.Response.Status}: {await ex.Response.AsString()}");
        }
        finally
        {
            client.Dispose();
        }
    }

    public async Task<long> ExecuteScalarAsync(string sql)
    {
        var client = new FluentClient();
        try
        {
            var res = await client.SetBasicAuthentication(_userName, _password)
                .PostAsync($"http://{Host}:{Port}/rest/sql/{DbName}")
                .WithBody(new StringContent(sql))
                .As<TaosRestReponse>();

            Check(res, sql);

            return res.Data.Length == 0 ? 0L : (long)res.Data[0][0]!;
        }
        catch (ApiException ex)
        {
            throw new Exception($"The API responded with HTTP {ex.Response.Status}: {await ex.Response.AsString()}");
        }
        finally
        {
            client.Dispose();
        }
    }

    public async Task<DataTable> ToDataTableAsync(string sql)
    {
        var client = new FluentClient();
        try
        {
            var res = await client.SetBasicAuthentication(_userName, _password)
                .PostAsync($"http://{Host}:{Port}/rest/sql/{DbName}")
                .WithBody(new StringContent(sql))
                .As<TaosRestReponse>();

            Check(res, sql);

            var dt = new DataTable();
            dt.Columns.AddRange(res.ColumnMeta
                .Select(m => new DataColumn(m[0]!.ToString(), _taosDataTypes[Convert.ToInt32(m[1])])).ToArray());
            foreach (var row in res.Data)
            {
                dt.LoadDataRow(row.ToArray(), LoadOption.Upsert);
            }

            return dt;
        }
        catch (ApiException ex)
        {
            throw new Exception($"The API responded with HTTP {ex.Response.Status}: {await ex.Response.AsString()}");
        }
        finally
        {
            client.Dispose();
        }
    }

    public async Task ExecuteNonQueryAsync(string sql)
    {
        var client = new FluentClient();
        try
        {
            var res = await client.SetBasicAuthentication(_userName, _password)
                .PostAsync($"http://{Host}:{Port}/rest/sql/{DbName}")
                .WithBody(new StringContent(sql))
                .As<TaosRestReponse>();

            Check(res, sql);
        }
        catch (ApiException ex)
        {
            throw new Exception($"The API responded with HTTP {ex.Response.Status}: {await ex.Response.AsString()}");
        }
        finally
        {
            client.Dispose();
        }
    }


    private static T? MapTo<T>(IReadOnlyList<ArrayList> columnMetas, IEnumerable<ArrayList> datas)
    {
        var dicList = new List<Dictionary<string, object>>();
        foreach (var data in datas)
        {
            var dic = new Dictionary<string, object>();

            for (var i = 0; i < columnMetas.Count; i++)
            {
                dic.Add(columnMetas[i][0]!.ToString()!, data[i]!);
            }

            dicList.Add(dic);
        }

        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(dicList));
    }

    private static void Check(TaosRestReponse response, string sql)
    {
        if (response.Status != TaosRestReponse.DefaultResponseStatus &&
            response.Code != TaosRestReponse.DefaultResponseCode)
        {
            throw new TaosRestRuquestException($"Taos rest request failed with sql: {sql}");
        }
    }
}