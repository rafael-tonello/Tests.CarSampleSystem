using System;
using System.Threading;

namespace RestFullAPI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			HttpServer s = new HttpServer(8001);
			s.OnRequest += delegate(HttpRequest reuqest)
			{
				var ret = new HttpResponse();
				ret.setBodyString("Working!");

				return ret;
			};

			while (true)
				Thread.Sleep (10);
		}
	}
}
