using System;

namespace TdStudio.Taos;

public sealed class TaosRestRuquestException : Exception
{
    public TaosRestRuquestException(string message) : base(message)
    {
    }
}