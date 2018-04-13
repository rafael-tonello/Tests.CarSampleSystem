using System;
using System.Collections.Generic;

namespace RestFullAPI
{
	public class HttpRequest
	{
		public string Method = "";
		public string Resource = "";
		public Dictionary<string, string> Headers = new Dictionary<string, string>();
		public Dictionary<string, string> uUrlVars = new Dictionary<string, string>();
		public int ContentLength = 0;
		public byte[] Body = new byte[]{};


		public void setBodyString (string content)
		{
			this.Body = new byte[content.Length];
			int index = 0;
			foreach (var c in content)
				this.Body[index++] = Convert.ToByte(c);
		}

        public void setHeader(string name, string value)
        {
            if (!this.Headers.ContainsKey(name))
                this.Headers.Add(name, value);
            else
                this.Headers[name] = value;
                
        }
	}
}

