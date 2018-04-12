using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

namespace RestFullAPI
{
	//ação utilizada para criar uma lista de delegates, aonde um sistema de observação 
	//é utilizado para notificar sobre a chegada de novas requisições
	public delegate HttpResponse OnRequesDelegate(HttpRequest request);
	public class HttpServer: IDisposable
	{
		private ThreadHelper listenerThread;

		//O servidor TCP
		TcpListener listener;

		//Deletegate para a observação
		public OnRequesDelegate OnRequest;


		public HttpServer (int port)
		{
			//Instancia o servidor na porta passada por parametro
			listener = new TcpListener(IPAddress.Any, port);

			//inicializa o servidor
            listener.Start();

            //captura as conexões de entrada
            ThreadListen();

		}

		//a classe abaixo é responsável por ficar na escuta de novas
		//conexãoes. Para cada nova conexão, o método Work é execudo.	
		private void ThreadListen()
		{
			bool acceptOk = true;

			listenerThread = ThreadHelper.StartNew(delegate(ThreadHelper sender){

	            if (acceptOk)
	            {
	                acceptOk = false;
	                listener.BeginAcceptTcpClient(delegate (IAsyncResult ar)
	                {
	                    acceptOk = true;
	                    //termina a captura da conexão e pega o cliente
	                    TcpListener listener = (TcpListener)ar.AsyncState;
	                    TcpClient client = listener.EndAcceptTcpClient(ar);

	                    //Inicializa a thread que irá escutar o cliente
	                    Work(client);


	            	}, listener);
	            }
			},true, 1);
		}


		//os estados da máquina de estados 
		enum States {READING_METHOD, READING_RESOURCE, READING_HTTP_VERSION, READING_A_HEADER, 
					PARSING_URL_VARS, PARSING_HEADERS, READING_CONTENT, CALLAPP};

		//A função abaixo mantém uma máquina de estados que interpreta
		//o protocolo http.
		private void Work(TcpClient client)
		{
			byte[] buffer;
			States state = States.READING_METHOD;

			HttpRequest request = new HttpRequest();
			HttpResponse response = new HttpResponse();

			string tempString = "";
			string tempName, tempValue;

			int totalContentLength = 0;

			string[] tempStrArr;
			//instancia a thread que vai fazer a leitura do client
            ThreadHelper.StartNew(delegate (ThreadHelper sender)
            {
            	//verifica se o cliente ainda está connectado
				if  ((client.Client != null) && (client.Client.Connected))
				{
					if (client.Client.Available > 0)
					{
						//instancia um buffer para receber os dados da conexão
						buffer = new byte[client.Client.Available];

						//lê os dados disponíveis na conexão
						client.Client.Receive(buffer, buffer.Length, 0);

						//processa o buffer recebido
						foreach (var cb in buffer)
						{
							char c = Convert.ToChar(cb);
							switch (state)
							{
								case States.READING_METHOD:
									if (c != ' ')
										request.Method += c;
									else
										state = States.READING_RESOURCE;
								break;
								case States.READING_RESOURCE:
									if (c != ' ')
										request.Resource += c;
									else
										state = States.READING_HTTP_VERSION;
								break;
								case States.READING_HTTP_VERSION:
									//a versão do HTTP será ignorada no momento
									if (c == '\n')
										state = States.READING_A_HEADER;

								break;
								case States.READING_A_HEADER:
									if (c != '\n')
									{
										if (c != '\r')
											tempString += c;
									}
									else
									{
										if (tempString == "")
										{
											if (totalContentLength > 0)
												state = States.READING_CONTENT;
											else
												state = States.PARSING_URL_VARS;
										}
										else
										{
											tempName = tempString.Substring(0, tempString.IndexOf(':'));
											tempValue = tempString.Substring(tempString.IndexOf(':'));

											//é muito comun que os browser enviem um space char entre o nome do cabeçalho e seu valor
											if ((tempValue !="") && ( tempValue[0] == ' '))
												tempValue = tempValue.Substring(1);

											request.Headers[tempName] = tempValue;

											tempString = "";

											//verifica se o cabeçalho atual é o content-lenth
											if (tempName.ToLower() == "content-length")
											{
												totalContentLength = int.Parse(tempValue);
												request.Body = new byte[totalContentLength];
											}

											request.ContentLength = 0;
										}
									}

								break;
								case States.READING_CONTENT:
									


									//lê o conteúdo
									request.Body[request.ContentLength++] = cb;
									if (request.ContentLength == totalContentLength)
										state = States.PARSING_URL_VARS;

									
								break;
							}
						}



					}

					//processa outros estados que trabalham com os dados após o recebimento
					switch (state)
					{
						case States.PARSING_URL_VARS:
							if (request.Resource.Contains("?"))
							{
								//separa as variáveis de url do recurso solicitado
								tempString = request.Resource.Substring(request.Resource.IndexOf('?')+1);
								request.Resource = request.Resource.Substring(0, request.Resource.IndexOf('?'));

								tempStrArr = SplitString(tempString, "&");
								foreach (var curr in tempStrArr)
								{
									//verifica se é uma variável válida
									if (curr.Contains("="))
									{
										tempName = curr.Substring(0, curr.IndexOf('='));
										tempValue = curr.Substring(curr.IndexOf('=')+1);

										request.uUrlVars[tempName] = tempValue;
									}
								}
							}

							state = States.CALLAPP;
						break;
						case States.CALLAPP:

							//cria alguns cabeçalhos que podem ser modifiados pelo app
							response.Headers["Server"] = "SomeServer 1.0 (by Rafael Tonello)";
	                        response.Headers["Content-Type"] = "text/html; charset=UTF-8";
	                        response.Headers["Accept-Ranges"] = "bytes";
	                        response.Headers["Connection"] = "Close";

							if (this.OnRequest != null)
								response = this.OnRequest(request);

							//envia a resposta para o browser
							response.Headers["Content-Length"] = response.Body.Length.ToString();

							//linha de status da resposta
							buffer = StringToBuffer("HTTP/1.1 "+response.httpStatusCode + " "+response.httpStatusMessage+"\r\n");
							client.Client.Send(buffer, buffer.Length, 0);

							//cabeçalhos
							foreach (var curr in response.Headers)
							{
								buffer = StringToBuffer(curr.Key+": "+curr.Value+"\r\n");
								client.Client.Send(buffer, buffer.Length, 0);
							}

							//linha de separação entre o cabeçalho e o conteúdo
							buffer = StringToBuffer("\r\n");
							client.Client.Send(buffer, buffer.Length, 0);

							//conteúdo
							client.Client.Send(response.Body, response.Body.Length, 0);

							//encerra a conexão
							client.Client.Disconnect(false);
							sender.Stop();


						break;
					}
				}
				else
					sender.Stop();
            });
		}

		public void Dispose()
		{
			listenerThread.Stop();
		}

		public string[] SplitString (string text, string seps)
		{
			List<string> result = new List<string> ();
			string temp = "";
			foreach (var c in text) {
				if (seps.Contains(c+""))
				{
					result.Add(temp);
					temp = "";
				}
				else
					temp += c;

			}
			result.Add(temp);
			return result.ToArray();

		}

		public byte[] StringToBuffer (string data)
		{
			byte[] result = new byte[data.Length];
			int index = 0;
			foreach (var c in data)
				result [index++] = Convert.ToByte(c);

			return result;
		}
	}
}

