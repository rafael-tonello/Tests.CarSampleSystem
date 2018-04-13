using System;
using System.Collections.Generic;
using RestFullAPI.DAO;
using RestFullAPI.Libs.SharedTypes;

namespace RestFullAPI.Tests
{
    public class TestMongoDB
    {
        public TestMongoDB()
        {
            Car car = new Car
            {
                veichle = "Fiesta",
                vendor = "Ford",
                year = 2008,
                description = "Ford Fiesta 2008 1.6",
                sold = false,
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now
            };

            MongoDBDriver db = new MongoDBDriver();
            db.OperateCar(ref car, DaoOperations.Insert);

            List<Car> result = (List<Car>)(db.OperateCar(ref car, DaoOperations.Search).data);
            bool ok = result.Count > 0;
            string a = ok.ToString();
            ok = bool.Parse(a);
        }
    }
}
