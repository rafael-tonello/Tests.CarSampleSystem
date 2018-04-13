using System;
using System.Text.RegularExpressions;

namespace RestFullAPI.Mediator
{
    //os mediadores (todas as classes do namespace Mediador) não são desaclopávies.
    //Estas classes contém a regra de negócio do sistema.
    public class CarsApi
    {
        DAO.DAOCtrl pDao;
        public CarsApi(APIHelper api, DAO.DAOCtrl dao = null)
        {
            if (dao != null)
                pDao = dao;
            else
                pDao = new DAO.DAOCtrl();
            
            
            api.SignResource("/veiculos", "GET", this.CarsResource);

            api.SignResource("/veiculos/find", "GET", this.FindCarsResource);

            api.SignResource(new Regex("\\/veiculos\\/[^find]"), "GET", this.GetCarResource);
            api.SignResource(new Regex("\\/veiculos\\/[^find]"), "POST", this.PostCarResource);
            api.SignResource(new Regex("\\/veiculos\\/[^find]"), "PUT", this.PutCarResource);
            api.SignResource(new Regex("\\/veiculos\\/[^find]"), "PATH", this.PathCarResource);
            api.SignResource(new Regex("\\/veiculos\\/[^find]"), "DELETE", this.DeleteCarResource);
        }

        private HttpResponse CarsResource(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            return response;
        }

        private HttpResponse FindCarsResource(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            return response;
        }

        private HttpResponse GetCarResource(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            return response;
        }

        private HttpResponse PostCarResource(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            return response;
        }
        private HttpResponse PutCarResource(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            return response;
        }
        private HttpResponse PathCarResource(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            return response;
        }
        private HttpResponse DeleteCarResource(HttpRequest request)
        {
            HttpResponse response = new HttpResponse();

            return response;
        }
    }
}
