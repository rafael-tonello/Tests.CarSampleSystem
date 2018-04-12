using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RestFullAPI
{
	public delegate void ThreadHelperAction(ThreadHelper sender);


	public class ThreadHelper: IDisposable
	{
		private static List<ThreadHelper> instances = new List<ThreadHelper>();

		private bool running = false;
		public ThreadHelper (){}
		public ThreadHelper (ThreadHelperAction action, bool whileTrue = true, int loopSleepInterval = 0)
		{
			this.Start(action, whileTrue, loopSleepInterval);
		}

		public void Start (ThreadHelperAction action, bool whileTrue = true, int loopSleepInterval = 0)
		{
			instances.Add (this);
			this.running = whileTrue;
			Thread th = new Thread(delegate() {
				do
				{
					action(this);

					if (loopSleepInterval > 0)
						Thread.Sleep(loopSleepInterval);
				}
				while (this.running);
			});

			th.Start();
		}


		public void Stop()
		{
			this.running = false;
		}

		public static void StopAllThreads()
		{
			Parallel.ForEach(instances, delegate(ThreadHelper currInstance)
			{
				currInstance.Stop();
			});
		}

		public void Sleep (int ms)
		{
			Thread.Sleep(ms);
		}

		public Dictionary<string, object> Tags = new Dictionary<string, object>();


		public static ThreadHelper StartNew (ThreadHelperAction action, bool whileTrue = true, int loopSleepInterval = 0)
		{
			return new ThreadHelper(action, whileTrue, loopSleepInterval);
		}

		public void Dispose()
		{
			this.Stop();
		}

	}
}

