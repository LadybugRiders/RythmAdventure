using System;
using System.Collections;
using System.Collections.Generic;

public class JSONData
{
    public string Id;

    public JSONData() {}

    public JSONData(JSONObject _json)
    {
        BuildJSONData(_json);
    }

    public virtual void BuildJSONData(JSONObject _json)
    {
        Id = _json.GetField("id").str;
        if( Id == null )
            Id = _json.GetField("id").ToString();
    }
}

public interface IJSONDataCollection : IEnumerable
{
    void AddElement(JSONObject element);
}

/// <summary>
/// Implementation of a IJSONDataCollection, using a Dictionary indexed by a string ( Id ) as its collection 
/// </summary>
public class IJSONDataDicoCollection<T> : IJSONDataCollection where T : JSONData, new()
{
    protected Dictionary<string, T> items = new Dictionary<string, T>();

    public void AddElement(JSONObject _element) {         
        T t = new T();
        t.BuildJSONData(_element);
        items[t.Id] = t;
    }

    public IEnumerator GetEnumerator()
    {
        return items.GetEnumerator();
    }

    public T this[string i]
    {
        get
        {
            if (items.ContainsKey(i))
                return items[i];
            return default(T);
        }
    }
}

/// <summary>
/// Implementation of a IJSONDataCollection, using a List as its collection 
/// </summary>
public class IJSONDataListCollection<T> : IJSONDataCollection where T : JSONData, new()
{
    protected List<T> items = new List<T>();

    public void AddElement(JSONObject element)
    {
        T t = new T();
        t.BuildJSONData(element);
        items.Add(t);
    }

    public IEnumerator GetEnumerator()
    {
        return items.GetEnumerator();
    }
    
    public T this[int i]
    {
        get
        {
            if (i < items.Count)
                return items[i];
            return default(T);
        }
    }
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
