﻿namespace Gig.Framework.Core.Serilizer;

public interface ISerializer
{
    string Serialize(object value);
    T Deserialize<T>(string value);
}