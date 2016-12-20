using System;
using System.Collections;
using System.Collections.Generic;

public interface IJSONDataCollection : IEnumerable
{
    void AddElement(JSONObject element);
}

public class JSONLoaderLR {

    public static T LoadTable<T>(JSONObject _jsonObject) where T : IJSONDataCollection, new()
    {
        T t = new T();
        for (int i = 0; i < _jsonObject.Count; ++i)
        {
            var element = _jsonObject[i];
            t.AddElement(element);
        }
        return t;
    }
}
