using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjFravega.Repositories
{
    public class MongoDBRepository
    {
        public MongoClient client;
        public IMongoDatabase db;

        public MongoDBRepository()
        {
            //BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            client = new MongoClient("mongodb://127.0.0.1:27017");            

            db = client.GetDatabase("PromocionesFravega");
        }
    }
}