using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.ComponentModel;

namespace DSP_Prototype_GUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private String outHead = "Output : ";
		private String outPORT = "PORT: ";
		private bool isopen_port = false;
		private static bool listening = false;
		private static bool finished = false;
		static SerialPort serialPort = new SerialPort();
		private int head = 0x00;

		private delegate void RcvDatDelegate();
		private delegate void UpdateUIDelegate(String data);
		private static System.Timers.Timer timer;
		private static bool receivedDat_sim = false;

		public MainWindow()
		{
			InitializeComponent();
			portSTA.Content = outPORT;
			BaudRateBox.SelectedIndex = 8;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			listening = false;
			serialPort.Close();
			while(!finished)
			base.OnClosing(e);
		}

		private void initVars(){
			outputText.Content = outHead;
		}

		private void setDisplayText(String text){
			outputText.Content = outHead + text;
		}

		private string getDisplayText()
		{
			return outputText.Content.ToString().TrimStart(outHead.ToCharArray());
		}
		private void sendText_Click(object sender, RoutedEventArgs e)
		{
			sendData(inputText.Text.Trim());
		}

		private void openPORT_Click(object sender, RoutedEventArgs e)
		{
			if(isopen_port){
				//close the port
				isopen_port = !isopen_port;
				serialPort.Close();
				openPORT_button.Content = "Open Port";
				portSTA.Content = outPORT;
			}
			else{
				//open the port
				isopen_port = !isopen_port;
				//SetTimer();
				try
				{
					serialPort.Open();
				}
				catch(Exception ex) 
					when (ex is ArgumentException || ex is InvalidOperationException || ex is UnauthorizedAccessException)
				{
					Console.WriteLine("openPort_Click_Exception: " + ex.Message);
				}
				//RcvDatDelegate listener = new RcvDatDelegate(this.Read);
				//listener.BeginInvoke(null, null);
				openPORT_button.Content = "Close Port";
				portSTA.Content = outPORT + serialPort.PortName;
			}
		}

		private void scanPorts_Click(object sender, RoutedEventArgs e)
		{
			scan();
		}

		private void scan(){
			string[] ports = SerialPort.GetPortNames();
			List<PortInfo> portList = new List<PortInfo>();
			foreach (string port in ports){
				portList.Add(new PortInfo() { PortName = port });
			}
			//GetPortParity();
			portListGUI.ItemsSource = portList;
		}

		private void GetPortParity()
		{
			string parity;
			foreach (string port in Enum.GetNames(typeof(Parity)))
			{
				Console.WriteLine("    {0}", port);
				parity = port;
			}

			//return (Parity)Enum.Parse(typeof(Parity), parity, true);
		}

		private void OnPortSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			setPortValues();
		}


		private void setPortValues()
		{
			if (portListGUI.SelectedItem != null)
			{
				if (!isopen_port)
				{
					PortInfo mPortInfo = (portListGUI.SelectedItem as PortInfo);
					serialPort.PortName = mPortInfo.PortName;
					Console.WriteLine("Selected Port: " + serialPort.PortName + "  baud: " + BaudRateBox.Text);
					serialPort.BaudRate = int.Parse(BaudRateBox.Text);
					serialPort.Parity = Parity.None;
					serialPort.DataBits = 8;
					serialPort.StopBits = StopBits.One;
					serialPort.ReadTimeout = 500;
					serialPort.WriteTimeout = 500;
					serialPort.ReceivedBytesThreshold = 5;
					serialPort.WriteBufferSize = 2048;
					openPORT_button.IsEnabled = true;
					//serialPort.DataReceived += DataReceivedHandler;
					serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
				}
			}
		}

		private void EnterKeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Return)
			{
				sendData(inputText.Text.Trim());
			}
		}

		private void sendData(string data)
		{
			char header = (char)0xAA;
			byte head = 0xAA;
			byte tail = 0xAB;
			int ender = 0xAB;
			byte[] hbyte = Encoding.UTF8.GetBytes(header.ToString());
			byte[] ebyte = Encoding.UTF8.GetBytes(ender.ToString());
			Console.WriteLine("startTrans: \"{0}\",{0:X}:{2}     endTrans: \"{1}\", {1:X}",header,ender,hbyte.Length);
			byte[] dataBytes = Encoding.UTF8.GetBytes(data);
			
			int totallength = dataBytes.Length + 2;
			byte[] sendBuf = new byte[totallength];
			sendBuf[0] = head;
			dataBytes.CopyTo(sendBuf,1);
			sendBuf[sendBuf.Length-1] = tail;
			Console.WriteLine("Message Sent: {4} -> {0} : {2:X}{1}{3:X}",totallength,
				Encoding.UTF8.GetString(dataBytes),Encoding.UTF8.GetString(hbyte),Encoding.UTF8.GetString(ebyte),
				dataBytes,hbyte,ebyte);

			//MessageBox.Show("  data sent on:\n" + serialPort.PortName + " : " + serialPort.BaudRate, "DATA SENT");
			inputText.Text = "";
			serialPort.Write(sendBuf, 0, sendBuf.Length);
		}

		/*private void Read()
		{
			string recData = "";
			listening = true;
			int cntr = 0;
			while(listening)
			{
				try
				{
					if (serialPort.IsOpen && serialPort.BytesToRead>0)
					{
						//recData = serialPort.ReadLine();
						//recData = getPortData();
						if (recData == null)
							continue;
						receivedDat_sim = false;
						//recData = "Message Received! Count = " + cntr;
						if (recData != null && recData.Length > 0)
						{
							openPORT_button.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateUIDelegate(UpdateOutputUI), recData);
							cntr++;
						}
						Console.WriteLine("while thread");
					}
				}
				catch (InvalidOperationException e)
				{
					Console.WriteLine("INVALID OP: " + e.Message);
				}
			}
			finished = true;
		}*/

		private String getPortData(SerialPort sp)
		{
			string rcdat = "";
			int rcv = 0x09;
			Console.WriteLine("head == {0}:{1}",(char)head,head);
			if(head == 0x00)
			{
				try
				{
					rcv = sp.ReadChar();
					head = rcv;
				}
				catch (TimeoutException e)
				{
					Console.WriteLine("ReadChar() Timeout: " + e.Message);
				}
				Console.WriteLine("header: " + ((char)rcv).ToString()+":"+rcv);
			}
			if(head == 0x02)
			{
				Console.WriteLine("serialPort.BytesToRead = {0}", serialPort.BytesToRead);
				int data;
				while (serialPort.BytesToRead > 0)
				{
					try
					{
						data = sp.ReadChar();
						Console.WriteLine("character received: {0}:{1}",((char)data).ToString(),data);
						Console.WriteLine("bytes to read: {0}",serialPort.BytesToRead);
						if (data == 0x03)
						{
							Console.WriteLine("data = {0}   head = {1}",Convert.ToChar(data),((char)head).ToString());
							head = 0x00;
							break;
						}
						rcdat += ((char)data).ToString();
					}catch (InvalidOperationException e)
					{
						Console.WriteLine("INVALID OP: " + e.Message);
						return null;
					}catch (TimeoutException e)
					{
						Console.WriteLine("Timeout Error: \"Failed To Read Error\" " + e.Message);
						return null;
					}
				}
			}
			else
			{
				head = 0x00;
			}
			return rcdat;
		}

		private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				Console.WriteLine("Received Data! Event Type: "+e.EventType);
				if (e.EventType != SerialData.Chars) return;
				//var sp = (SerialPort)sender;
				var sp = serialPort;
				Console.WriteLine("sp.DataInBuffer = {0}", sp.BytesToRead);
				string data = getPortData(sp);
				if (data != null || data.Length > 0)
				{
					if(head == (char)0x00)
					{
						Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateUIDelegate(UpdateOutputUI), data);
					}
					else
					{
						Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateUIDelegate(UpdateOutputUI), data);
					}
				}
			}catch (NullReferenceException err)
			{
				Console.WriteLine("SerialDataReceived: " + err.Message);
			}
		}

		private void UpdateOutputUI(String data)
		{
			setDisplayText(data);
		}

		private static void SetTimer()
		{
			timer = new System.Timers.Timer(10000);
			timer.Elapsed += OnTimedEvent;
			timer.AutoReset = true;
			timer.Enabled = true;
		}

		private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
		{
			Console.WriteLine("Timer Fired! {0:HH:mm:ss.ffff}", e.SignalTime);
			receivedDat_sim=true;
		}
	}

	public class PortInfo
	{
		public string PortName { get; set; }
	}

	public class BaudRateList : ObservableCollection<string>
	{
		public BaudRateList()
		{
			Add("1200");
			Add("2400");
			Add("4800");
			Add("9600");
			Add("14400");
			Add("19200");
			Add("38400");
			Add("57600");
			Add("115200");
			Add("128000");
			Add("256000");
		}
	}

	public class PortHandler
	{

	}


}
