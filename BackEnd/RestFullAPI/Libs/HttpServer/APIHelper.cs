using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestFullAPI
{
    //Esta classe utiliza os servidor http e serve para facilitar a criação
    //de APIS RestFul
    public class APIHelper: IDisposable
    {
        
        //pointeiro para um HttpServer
        private HttpServer server;

        //construtor que não inicializa o servidor. Caso a classe seja contruída
        //com este construtor, e necessário que o método StartServer seja executado
        //para que a API fique disponível
        public APIHelper(){}

        //Construtor que inicializa o servidor
        public APIHelper(int port)
        {
            server = new HttpServer(port);
            server.OnRequest = this.OnServerRequest;

        }


        private HttpResponse OnServerRequest(HttpRequest request)
        {
            HttpResponse result = new HttpResponse();
            result.httpStatusCode = 404;
            result.httpStatusMessage = "Resource "+request.Resource+" not found for verb "+request.Method;
            //percore a lista de assinantes de recursos e verifica se alguem está assinando o método
            Parallel.ForEach(this.SignsRegex, delegate (Tuple<Regex, string, OnRequesDelegate> curr)
            {
                if ((curr.Item1.IsMatch(request.Resource)) && 
                    (curr.Item2 == request.Method))
                {
                    result = curr.Item3(request);
                }
            });

            Parallel.ForEach(this.SignsStrings, delegate (Tuple<string, string, OnRequesDelegate> curr)
            {
                if ((curr.Item1 == request.Resource) &&
                    (curr.Item2 == request.Method))
                {
                    result = curr.Item3(request);
                }
            });

            return result;
        }
        //Método utilizado para parar o servidor
        public void StopServer()
        {
            server.Dispose();
            server = null;
        }


        //método utilizado para inicializar o servidor
        public void StartServer(int port)
        {
            if (server != null)
                this.StopServer();
        }

        //Lista de assinaturas de recursos
        private List<Tuple<string, string, OnRequesDelegate>> SignsStrings = new List<Tuple<string, string, OnRequesDelegate>>();
        private List<Tuple<Regex, string, OnRequesDelegate>> SignsRegex = new List<Tuple<Regex, string, OnRequesDelegate>>();

        //este método é utilizado para que as demais partes do sistema possa
        //"ouvir" recursos determinados.
        public void SignResource(string resource, string method, OnRequesDelegate request)
        {
            SignsStrings.Add(new Tuple<string, string, OnRequesDelegate>(resource, method, request));
        }

        public void SignResource(Regex resource, string method, OnRequesDelegate request)
        {
            SignsRegex.Add(new Tuple<Regex, string, OnRequesDelegate>(resource, method, request));
        }


        public void Dispose()
        {
            if (server != null)
                server.Dispose();
        }
    }
}
