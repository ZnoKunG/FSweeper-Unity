using MongoDB.Bson;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

[System.Serializable]
public class Player
{
    [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
    [BsonRepresentation(BsonType.ObjectId)]
    public string _id;
    public string name;
    public float score;
}

public class PlayerList
{
    public List<Player> list;
}