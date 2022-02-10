using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Diagnostics;
using System.Timers;

namespace UnitTestProject1
{
    [TestClass]
    public class GetInternetSpeed
    {
        [TestMethod]
        public void TestMethod1()
        {
			NM_Monitor nmMonitor = new NM_Monitor();
			NM_Adapter[] arrAdapters = nmMonitor.arrAdapters;


			if (arrAdapters.Length == 0)
			{
				Console.WriteLine("No network adapters found.");
				return;
			}

			nmMonitor.Start();

			for (int i = 0; i < 10; i++)
			{
				foreach (NM_Adapter tmpAdapter in arrAdapters)
				{

					Console.WriteLine(tmpAdapter.AdapterName);

					Console.WriteLine("Download: " + tmpAdapter.DownloadSpeedKbps*8 + " kbps " + Environment.NewLine +
					   "Upload: " + tmpAdapter.UploadSpeedKbps*8 + " kbps ");
				}

				System.Threading.Thread.Sleep(1000);
			}

			nmMonitor.Stop();
		}
    }

	public class NM_Adapter
	{

		internal NM_Adapter(string strName)
		{
			strAdapterName = strName;
		}

		private long lngDownloadSpeed;
		private long lngUploadSpeed;
		private long lngDownloadValue;
		private long lngUploadValue;
		private long lngOldDownloadValue;
		private long lngOldUploadValue;

		internal string strAdapterName;
		internal PerformanceCounter pcDownloadCounter;
		internal PerformanceCounter pcUploadCounter;

		internal void Initialize()
		{
			lngOldDownloadValue = pcDownloadCounter.NextSample().RawValue;
			lngOldUploadValue = pcUploadCounter.NextSample().RawValue;
		}

		internal void Update()
		{
			lngDownloadValue = pcDownloadCounter.NextSample().RawValue;
			lngUploadValue = pcUploadCounter.NextSample().RawValue;

			lngDownloadSpeed = lngDownloadValue - lngOldDownloadValue;
			lngUploadSpeed = lngUploadValue - lngOldUploadValue;

			lngOldDownloadValue = lngDownloadValue;
			lngOldUploadValue = lngUploadValue;
		}

		public override string ToString()
		{
			return this.strAdapterName;
		}

		public string AdapterName
		{
			get
			{
				return strAdapterName;
			}
		}

		public long DownloadSpeed
		{
			get
			{
				return lngDownloadSpeed;
			}
		}

		public long UploadSpeed
		{
			get
			{
				return lngUploadSpeed;
			}
		}

		public double DownloadSpeedKbps
		{
			get
			{
				return lngDownloadSpeed / 1024.0;
			}
		}

		public double UploadSpeedKbps
		{
			get
			{
				return lngUploadSpeed / 1024.0;
			}
		}
	}

	public class NM_Monitor
	{
		private Timer tmrTime;
		private ArrayList alAdapters;
		private ArrayList alAdaptersMonitored;

		public NM_Monitor()
		{
			alAdapters = new ArrayList();
			alAdaptersMonitored = new ArrayList();

			LoopAdapters();

			tmrTime = new Timer(1000);
			tmrTime.Elapsed += new ElapsedEventHandler(tmrTime_Elapsed);
		}

		private void LoopAdapters()
		{

			PerformanceCounterCategory pcNetworkInterface = new PerformanceCounterCategory("Network Interface");

			foreach (string tmpName in pcNetworkInterface.GetInstanceNames())
			{

				if (tmpName == "MS TCP Loopback interface")
					continue;

				NM_Adapter netAdapter = new NM_Adapter(tmpName);

				netAdapter.pcDownloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", tmpName);
				netAdapter.pcUploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", tmpName);

				alAdapters.Add(netAdapter);
			}
		}

		private void tmrTime_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (NM_Adapter tmpAdapter in alAdaptersMonitored)
				tmpAdapter.Update();
		}

		public NM_Adapter[] arrAdapters
		{
			get
			{
				return (NM_Adapter[])alAdapters.ToArray(typeof(NM_Adapter));
			}
		}

		public void Start()
		{
			if (this.alAdapters.Count > 0)
			{
				foreach (NM_Adapter currAdapter in alAdapters)

					if (!alAdaptersMonitored.Contains(currAdapter))
					{
						alAdaptersMonitored.Add(currAdapter);
						currAdapter.Initialize();
					}

				tmrTime.Enabled = true;
			}
		}

		public void Start(NM_Adapter nmAdapter)
		{
			if (!alAdaptersMonitored.Contains(nmAdapter))
			{
				alAdaptersMonitored.Add(nmAdapter);
				nmAdapter.Initialize();
			}

			tmrTime.Enabled = true;
		}

		public void Stop()
		{
			alAdaptersMonitored.Clear();
			tmrTime.Enabled = false;
		}

		public void Stop(NM_Adapter currAdapter)
		{
			if (alAdaptersMonitored.Contains(currAdapter))
				alAdaptersMonitored.Remove(currAdapter);

			if (alAdaptersMonitored.Count == 0)
				tmrTime.Enabled = false;
		}
	}
}
