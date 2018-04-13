using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RestFullAPI
{
	//enum com todas as configurações que podem ser utilizadas no sistema.
	//Esta parte do sistema não é facilmente desaclopável. É uma parte
	//específicia para este aplicativo
	public enum Confs{ API_HTTP_PORT, MONGODB_HOST, MONGODB_PORT, MONGODB_CARS_COLLECTION_NAME }


	//O sistema de configurações notifica o sistema sobre alterações nas configurações.
	//Os módulos e serviços do sistema podem adicionar um delegate do tipo abaixo
	//para começar a receber as mensagens de alterações das configuraçõess
	public delegate void OnConfChangeDelegate(Confs conf, VariantVar value);

	//A classe abaixo é um Singleton. Ela representa um sistema interno de configurações
	//e sua responsabilidade é a de monitorar alterações de configurações e notificar
	//o sistema sobre isso. 
	public class ConfsCtrl
	{
        //lista de configurações do sistema. Quando o arquivo é alterado, as 
        //configurações que estão nele são comparadas com esta lista. Para cad
        //alterações, os observadores são notificados.
        Dictionary<string, VariantVar> confsBuffer = new Dictionary<string, VariantVar>();

        //a variável abaixo é utilizada para fazer a identificação de alteração
        //do arquivo. A data da última gravação do arquivo é comparada com esta
        //variável e, em caso de diferença, sabe-se que houve uma alteração.
        DateTime confsBufferTime;

        //endereço do arquivo de configurações
        string filename = "/etc/CarsSample";

		//Handle para a instancia única da classe
        private static ConfsCtrl instance = new ConfsCtrl();


		//Manter o construtor como privado, evita que a classe possa ser instanciada
		//de fora dela mesma
		private ConfsCtrl ()
		{
            //caso o arquivo não exista, cria um arquivo padrão
            if (!File.Exists(filename))
                CreateDefaultFile();

            //inicializa a data de gravação como sendo a atual. Na thread de verificação
            //de alterações, esta data será diferente da última gravaçã do arquivo e
            //as configurações serão carregadas.
            confsBufferTime = DateTime.Now;

            //inicializa a thread que irá monitorar o arquivo
			ThreadHelper.StartNew(FileMonitor, true, 100);


		}


        //A função abaixo monitora o arquivo de configuração do sistema e identifica
        //qualquer alteração de valores.
        private void FileMonitor(ThreadHelper sender)
        {
            waitOne();
            string name;
            VariantVar value = new VariantVar();
            DateTime fileWriteTime = File.GetLastWriteTime(filename);
            if (fileWriteTime != confsBufferTime)
            {
                //Quando há uma alteração do arquivo, carrega-o e analiza as suas linhas 
                //em busca das configurações. Para cada uma, verifica se está diferente 
                //do valor equivalente em "confsBuffer". Se estiver, atualiza o valor
                //em "confsBuffer" e notifica os observadores.

                string[] lines = File.ReadAllLines(filename);
                for (int cont = 0; cont < lines.Length; cont++)
                {
                    string currLine = lines[cont];
                    //verifica se é uma linha de configuração válida
                    if (currLine.Contains("="))
                    {
                        //pega o nome da variável
                        name = currLine.Substring(0, currLine.IndexOf("="));

                        //pega o valor da variável
                        value.AsString = currLine.Substring(currLine.IndexOf("=") + 1);

                        //verifica se houve alteração da variável

                        if (!confsBuffer.ContainsKey(name))
                        {
                            confsBuffer.Add(name, new VariantVar("--invalid--value--"));
                        }

                        if (confsBuffer[name].AsString != value.AsString)
                        {
                            confsBuffer[name].AsString = value.AsString;
                            //notifica os observadores sobre a alteração da variável
                            if (OnChange != null)
                                OnChange.Invoke((Confs)Enum.Parse(typeof(Confs), name), value);
                        }
                    }
                }
            }
        }

        private void CreateDefaultFile()
        {
            string file =   "#Arquivo de configuração do sistema com API Restful\r\n"+
                            "\r\n" +
                            "#Porta em que o servidor HTTP deverá escutar\r\n" +
                            "API_HTTP_PORT=8001\r\n" +
                            "\r\n"+
                            "#Porta em que o servidor HTTP deverá escutar\r\n" +
                            "MONGODB_HOST=127.0.0.1\r\n"+
                            "\r\n" +
                            "#Porta em que o servidor HTTP deverá escutar\r\n" +
                            "MONGODB_PORT=\r\n"+
                            "\r\n" +
                            "#Porta em que o servidor HTTP deverá escutar\r\n" +
                            "MONGODB_CARS_COLLECTION_NAME=\r\n";

            File.WriteAllText(filename, file);


        }


		//Lista de observadores da classe (note que o sistema de observação não feito
		//com classes, mas sim com um Delegate, ou seja, ponteiro de função)
		private event OnConfChangeDelegate OnChange; 
        public static ConfsCtrl Instance{
			get{ return instance; }
		}

		//Quando o sistema de configuração é instanciado, ele cria uma imagem do 
		//arquivo de configurações e depois disso passa a monitorar alterações.
		//Os serviços do sistema só recebem notificaçõe de alterações de variáveis e 
		//tem como acessar uma configuração individualmente. Para que os demais sistemas
		//possam receber configurações a qualquer momento, eles podem executar o método
		//abaixo. 
		//
		//Este método irá executar o callback passado em OnChange para cada configuração
		//do sistema
		public void RefreshOne(OnConfChangeDelegate _OnChange)
		{
            waitOne();
            Parallel.ForEach(confsBuffer, delegate(KeyValuePair<string, VariantVar> curr){
                _OnChange((Confs)Enum.Parse(typeof(Confs), curr.Key), curr.Value);
            });
            releaseOne();

		}

        public void ObservateChanges(OnConfChangeDelegate _OnChange)
        {
            this.OnChange += _OnChange;
        }


        private int waiting = 1;
        private void waitOne()
        {
            while (waiting == 0){Thread.Sleep(1);}

            waiting--;
            return;
        }

        private void releaseOne()
        {
            waiting++;
            return;
        }


	}
}

