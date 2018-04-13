using System;
using System.Collections.Generic;
using System.Globalization;
using JsonMaker;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using RestFullAPI.Libs.SharedTypes;

namespace RestFullAPI.DAO
{
    public class MongoDBDriver : DriverInterface
    {
        
        //conexão com o mongoDB
        private MongoClient client = null;

        //rereferencia para o base de dados no MongoDB
        private IMongoDatabase db = null;

        //variável utilizada para indicar à função "connect" se a coxão com o 
        //banco deve ser refeita
        private bool reconnect = true;

        string conf_host = "";
        string conf_port = "";
        string conf_collection = "";

        bool connect()
        {
                
            if (reconnect)
            {
                string connectionString = "mongodb://"+conf_host+":"+conf_port;
                client = new MongoClient(connectionString);

                db = client.GetDatabase(conf_collection);
            }

            //tenta verificar se o banco está ativo
            try
            {
                db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(5000);
                return true;
            }
            catch { }

            return false;
        }

        
        public MongoDBDriver()
        {
            ConfsCtrl.Instance.RefreshOne(this.confsChanges);
        }

        private Car BsonDocumentToCar(BsonDocument doc)
        {
            JSON currJson = new JSON();
            currJson.parseJson(doc.ToJson());
            Car currCar = new Car();

            if (currJson.contains("_id"))
                currCar.id = currJson.getString("_id");

            if (currJson.contains("veichle"))
                currCar.veichle = currJson.getString("veicle");

            if (currJson.contains("vendor"))
                currCar.vendor = currJson.getString("vendor");

            if (currJson.contains("year"))
                currCar.year = currJson.getInt("vendor");

            if (currJson.contains("description"))
                currCar.description = currJson.getString("description");

            if (currJson.contains("sold"))
                currCar.sold = currJson.getBoolean("sold");

            /*if (currJson.contains("createdAt"))
                currCar.createdAt = DateTime.Parse()currJson.getDateTime("createdAt");

            if (currJson.contains("updatedAt"))
                currCar.updatedAt = currJson.getDateTime("updatedAt");*/

            return currCar;
        }


        private void confsChanges(Confs conf, VariantVar value)
        {
            switch (conf)
            {
                case Confs.MONGODB_HOST:
                    reconnect = true;
                    this.conf_host = value.AsString;
                    break;
                case Confs.MONGODB_PORT:
                    reconnect = true;
                    this.conf_port = value.AsString;
                    break;
                case Confs.MONGODB_CARS_COLLECTION_NAME:
                    reconnect = true;
                    this.conf_collection = value.AsString;
                    break;
            }
        }

        public Results OperateCar(ref Car item, DaoOperations operation)
        {
            //Resolve a conexão com o banco de dados
            if (this.connect())
            {
                //pega a coleção do mongoDB
				var carsCollection = db.GetCollection<BsonDocument>("cars");

                //verifica se está pesquisando
                if (operation == DaoOperations.Search)
                {
                    List<Car> result = new List<Car>();


                    //prepara os campos da pesquisa
                    JSON jm = new JSON();

                    if (item.veichle != "")
                        jm.setString("$or[" + jm.getChildsNames("$or").Count + "].veichle", "__regexs__"+item.veichle+ "__regexe__");
                    if (item.vendor != "")
                        jm.setString("$or[" + jm.getChildsNames("$or").Count + "].vendor", "__regexs__" +item.vendor+ "__regexe__");
                    if (item.description != "")
                        jm.setString("$or[" + jm.getChildsNames("$or").Count + "].description", "__regexs__" +item.description + "__regexe__");
                    

                    string json = jm.ToJson();
                    json = json.Replace("\"__regexs__", "/.*");
                    json = json.Replace("__regexe__\"", "*./");

                    if (!jm.contains("$or"))
                        json = "{}";


                    //realiza a busca no banco de dados
                    List<BsonDocument> cursor = carsCollection.Find(json).ToList();


                    //converte os dados retornados do banco de dados
                    foreach (BsonDocument curr in cursor){
                        result.Add(BsonDocumentToCar(curr));
                    }

                    return new Results
                    {
                        sucess = true,
                        message = "ok",
                        data = result
                    };


                }
                //verifica se está solicitando a população do objeto Item
                else if (operation == DaoOperations.Populate)
                {
                    List<Car> result = new List<Car>();


                    //prepara os campos da pesquisa
                    JObject jm = new JObject();
                    jm.Add(new JProperty("_id", item.id));
                    //realiza a busca no banco de dados
                    List<BsonDocument> cursor = carsCollection.Find(jm.ToJson()).ToList();

                    if (cursor.Count > 0)
                    {
                        item = BsonDocumentToCar(cursor[0]);
                        return new Results
                        {
                            sucess = true,
                            message = "ok",
                            data = item
                        };
                    }
                    else
                    {
                        return new Results
                        {
                            sucess = false,
                            message = "Object not found"
                        };   
                    }
                    
                    
                }
                //verifica se está tentando inserir
                else if (operation == DaoOperations.Insert)
                {
                    //serializa o objeto
                    string jsonItem = Newtonsoft.Json.JsonConvert.SerializeObject(item);

                    //insere os dados no banco
                    carsCollection.InsertOne(BsonDocument.Parse(jsonItem));

                    return new Results
                    {
                        sucess = true,
                        message = "ok"
                    };
                }
                else if (operation == DaoOperations.Update)
                {
                    
                }
                else if (operation == DaoOperations.Delete)
                {
                    
                }


                return new Results
                {
                    sucess = false,
                    message = "Internal error"
                };
            }
            else
            {
                return new Results
                {
                    sucess = false,
                    message = "MongoDB Connection error"
                };
            }
        }


    }
}
