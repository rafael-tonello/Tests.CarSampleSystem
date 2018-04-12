using System;

namespace RestFullAPI
{
	//enum com todas as configurações que podem ser utilizadas no sistema.
	//Esta parte do sistema não é facilmente desaclopável. É uma parte
	//específicia para este aplicativo
	public enum Confs{ API_HTTP_PORT }


	//O sistema de configurações notifica o sistema sobre alterações nas configurações.
	//Os módulos e serviços do sistema podem adicionar um delegate do tipo abaixo
	//para começar a receber as mensagens de alterações das configuraçõess
	public delegate void OnConfChangeDelegate(Confs conf, VariantVar value);

	//A classe abaixo é um Singleton. Ela representa um sistema interno de configurações
	//e sua responsabilidade é a de monitorar alterações de configurações e notificar
	//o sistema sobre isso. 
	public class ConfsCtrl
	{

		//Handle para a instancia única da classe
		private static Confs instance;


		//Manter o construtor como privado, evita que a classe possa ser instanciada
		//de fora dela mesma
		private ConfsCtrl ()
		{
			ThreadHelper.StartNew(FileMonitor, true, 10);
		}


		//A função abaixo monitora o arquivo de configuração do sistema e identifica
		//qualquer alteração de valores.
		private void FileMonitor (ThreadHelper sender)
		{

		}


		//Lista de observadores da classe (note que o sistema de observação não feito
		//com classes, mas sim com um Delegate, ou seja, ponteiro de função)
		public OnConfChangeDelegate OnChange; 
		public static Confs Instance{
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
		public void RefreshOne(OnConfChangeDelegate OnChange)
		{

		}


	}
}

