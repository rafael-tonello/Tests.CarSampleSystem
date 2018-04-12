using System;

namespace RestFullAPI
{
	public class HttpResponse: HttpRequest
	{
		public int httpStatusCode = 200;
		public string httpStatusMessage = "OK";
	}
}

