using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class Client : MonoBehaviour
{
	public static Client instance;
	public static int dataBufferSize = 8192;
	
	public string ip = "127.0.0.1";
	public int port = 8080;
	public int myId = 0;
	public TCP tcp;
	

	private void Awake()
    {
        if (instance == null)
        {
			instance = this;
        }
		else if (instance != this)
        {
			Destroy(this);
        }
    }

	private void Start()
    {
		tcp = new TCP();
    }

	public void ConnectToServer()
    {
		tcp.Connect();
    }

	public class TCP
    {
		public TcpClient socket;

		private NetworkStream stream;
		private byte[] receiveBuffer;

		public void Connect()
		{
			socket = new TcpClient { 
				ReceiveBufferSize = dataBufferSize, 
				SendBufferSize = dataBufferSize 
			};

			receiveBuffer = new byte[dataBufferSize];
			socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);


		}

		private void ConnectCallback(IAsyncResult _result)
        {
			socket.EndConnect(_result);

            if (!socket.Connected)
            {
				return;
            }

			stream = socket.GetStream();

			stream.BeginRead(receiveBuffer, 0, dataBufferSize, RecieveCallback, null);
        }

		private void RecieveCallback(IAsyncResult _result)
        {
            try
            {
				int _byteLength = stream.EndRead(_result);
				if(_byteLength <= 0)
                {
					//TODO: disconnect
					return;
                }

				byte[] _data = new byte[_byteLength];
				Array.Copy(receiveBuffer, _data, _byteLength);

				//TODO: handle data
				stream.BeginRead(receiveBuffer, 0, dataBufferSize, RecieveCallback, null);
            }

            catch
            {
				// TODO: disconnect
            }
        }

	}

}
