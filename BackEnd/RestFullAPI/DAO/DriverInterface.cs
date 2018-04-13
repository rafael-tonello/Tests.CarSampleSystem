using System;
using RestFullAPI.Libs.SharedTypes;

namespace RestFullAPI.DAO
{
    public enum DaoOperations { Insert, Update, Delete, Populate, Search }
    public interface DriverInterface
    {
        Results OperateCar(ref Car item, DaoOperations operation);
    }
}
