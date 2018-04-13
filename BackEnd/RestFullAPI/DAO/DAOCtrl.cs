using System;
using RestFullAPI.Libs.SharedTypes;

namespace RestFullAPI.DAO
{
    public class DAOCtrl: DriverInterface
    {
        DriverInterface loadedDriver = new MongoDBDriver();
        public DAOCtrl()
        {
            
        }

        public Results OperateCar(ref Car item, DaoOperations operation)
        {
            return loadedDriver.OperateCar(ref item, operation);
        }
    }
}
