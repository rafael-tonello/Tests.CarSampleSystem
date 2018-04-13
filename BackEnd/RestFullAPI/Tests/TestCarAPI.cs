using System;
using RestFullAPI.Mediator;

namespace RestFullAPI.Tests
{
    public class TestCarAPI
    {
        public TestCarAPI()
        {
            APIHelper _api = new APIHelper(8001);
            CarsApi api = new CarsApi(_api);
        }
    }
}
