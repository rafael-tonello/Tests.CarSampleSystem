using System;
using System.Threading;
using RestFullAPI.Mediator;

namespace RestFullAPI
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ConfsCtrl.Instance.Initialize();
            //Tests.TestMongoDB tmdb = new Tests.TestMongoDB();
            Tests.TestCarAPI tcapi = new Tests.TestCarAPI();

            MediatorCtrl mediator = new MediatorCtrl();
            while (true)
                Thread.Sleep(10);
        }
    }
}
