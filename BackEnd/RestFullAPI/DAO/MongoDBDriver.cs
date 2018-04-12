using System;
using MongoDB.Driver;
using RestFullAPI.Libs.SharedTypes;

namespace RestFullAPI.DAO
{
    public class MongoDBDriver : DriverInterface
    {

        private void connect()
        {
            /*MongoClient client = new MongoClient("mongodb://localhost:27017");
                IMongoDatabase db = client.GetDatabase("clientservertest");
            }*/
        }

        
        public MongoDBDriver()
        {
            //ConfsCtrl.Instance.RefreshOne(this.confsChanges);
        }



        public Results OperateCar(ref Car item, DaoOperations operation)
        {
            return new Results();
        }


    }
}
