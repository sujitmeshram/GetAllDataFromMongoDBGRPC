using Grpc.Core;
using mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mongo.Services
{

    public class MongoServiceImpl : DatabaseService.DatabaseServiceBase
    {
        public override async Task<CollectionData> GetDatabaseData(GetDatabaseRequest request, ServerCallContext context)
        {
            var client = new MongoClient(request.ConnectionString);
            var database = client.GetDatabase(request.DatabaseName);

            var collections = await database.ListCollectionNamesAsync();
            var collectionData = new CollectionData();

            await collections.ForEachAsync(async collectionName =>
            {
                var collection = database.GetCollection<BsonDocument>(collectionName);
                var documents = await collection.Find(new BsonDocument()).ToListAsync();
                var collectionEntry = new Collection();

                foreach (var doc in documents)
                {
                    var dataField = new DataField();
                    foreach (var element in doc.Elements)
                    {
                        dataField.Fields.Add(element.Name, element.Value.ToString());
                    }
                    collectionEntry.Data.Add(dataField);
                }

                collectionData.CollectionsData[collectionName] = collectionEntry;
            });

            return collectionData;
        }
    }
}


/*
using MongoDB.Driver;

    MongoClient c = new MongoClient("mongodb+srv://srileymusicofficial:@examples.jxp8c.mongodb.net/?retryWrites=true&w=majority&appName=examples");

List<string> list = c.ListDatabaseNames().ToList();

foreach (string data in list)
{
    Console.WriteLine(data);
}

*/
/*
sample_mflix
admin
local*/