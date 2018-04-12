using System;
namespace RestFullAPI.Mediator
{
    //os mediadores (todas as classes do namespace Mediador) não são desaclopávies.
    //Estas classes contém a regra de negócio do sistema.
    public class CarsApi
    {
        public CarsApi(APIHelper api)
        {
            api.SignResource("/cars", "GET");
            
        }
    }
}
