using System;
using System.Collections.Generic;
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


        private void confsChanges(Confs conf, VariantVar value)
        {
            switch (conf)
            {
                case Confs.MONGODB_CARS_COLLECTION_NAME:
                    reconnect = true;
                    this.conf_host = value.AsString;
                    break;
                case Confs.MONGODB_HOST:
                    reconnect = true;
                    this.conf_port = value.AsString;
                    break;
                case Confs.MONGODB_PORT:
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
                    JObject jm = new JObject();
                    if (item.veichle == "")
                        jm.Add(new JProperty("veichle", item.veichle));
                    if (item.vendor == "")
                        jm.Add(new JProperty("vendor", item.vendor));
                    if (item.description == "")
                        jm.Add(new JProperty("description", item.description));


                    //realiza a busca no banco de dados
                    List<BsonDocument> cursor = carsCollection.Find(jm.ToJson()).ToList();


                    //converte os dados retornados do banco de dados
                    foreach (BsonDocument curr in cursor){
                        JObject currJson = JObject.Parse(curr.ToJson());
                        Car currCar = new Car();
                        if (currJson.ContainsKey("veichle"))
                            currCar.veichle = currJson.GetValue("veicle").ToString();
                        
                        if (currJson.ContainsKey("vendor"))
                            currCar.vendor = currJson.GetValue("vendor").ToString();

                        if (currJson.ContainsKey("year"))
                            currCar.year = int.Parse(currJson.GetValue("vendor").ToString());

                        if (currJson.ContainsKey("description"))
                            currCar.description = currJson.GetValue("description").ToString();

                        if (currJson.ContainsKey("sold"))
                            currCar.sold = !"0false".Contains(currJson.GetValue("sold").ToString().ToLower());

                        if (currJson.ContainsKey("createdAt"))
                            currCar.createdAt = DateTime.Parse(currJson.GetValue("createdAt").ToString());

                        if (currJson.ContainsKey("updatedAt"))
                            currCar.updatedAt = DateTime.Parse(currJson.GetValue("updatedAt").ToString());

                        result.Add(currCar);
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
