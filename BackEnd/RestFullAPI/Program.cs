using System;
using System.Threading;

namespace RestFullAPI
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            tester a = new tester();


            while (true)
                Thread.Sleep(10);
        }
    }

    class tester
    {
        public tester()
        {
            ConfsCtrl.Instance.OnChange += this.confChanged;
            ConfsCtrl.Instance.RefreshOne(this.confChanged);

        }

        APIHelper api;
        private void httpLoad(int port)
        {
            if (api != null)
                api.Dispose();

            api = new APIHelper(port);
            api.SignResource(@"/carros", "GET", delegate(HttpRequest request) {

                return new HttpResponse();
            });

            api.SignResource(@"/carros*", "GET", delegate (HttpRequest request) {

                return new HttpResponse();
            });

            api.SignResource("/pessoas", "GET", delegate (HttpRequest request) {

                return new HttpResponse();
            });
            




        }

        private void confChanged(Confs conf, VariantVar value)
        {
            if (conf == Confs.API_HTTP_PORT)
            {
                httpLoad(value.AsInt);
            }
        }

    }
}
