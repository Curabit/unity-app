using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 

public class Current
{
    [JsonProperty("file-name")]
    public string FileName { get; set; }
    public bool isOnLoop { get; set; }
}

public class Next
{
    [JsonProperty("file-name")]
    public string FileName { get; set; }
    public bool isOnLoop { get; set; }
}

public class Previous
{
    [JsonProperty("file-name")]
    public string FileName { get; set; }
    public bool isOnLoop { get; set; }
}

public class JSONClasses
{
    public bool isStop { get; set; }
    public bool isPaused { get; set; }
    public List<string> videos { get; set; }
    public Current current { get; set; }
    public List<Next> next { get; set; }
    public Previous previous { get; set; }
}





