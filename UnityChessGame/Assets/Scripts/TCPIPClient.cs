using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TCPIPClient : MonoBehaviour
{
    [SerializeField]
    private Button connectToRoboDKButton;

    [SerializeField]
    private GameObject connectedText;

    [SerializeField]
    private GameObject roboDkIsWorkingPanel;

    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private StreamReader clientStreamReader;
    private StreamWriter clientStreamWriter;

    public bool serverReady { get; private set; }

    private void Awake()
    {
        connectToRoboDKButton.onClick.AddListener(() => Connect());
    }

    private void Connect()
    {
        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 65432);
            networkStream = tcpClient.GetStream();
            clientStreamReader = new StreamReader(networkStream);
            clientStreamWriter = new StreamWriter(networkStream);

            Task.Run(() =>
            {
                WaitForServerMsg();
            });
            connectToRoboDKButton.gameObject.SetActive(false);
            connectedText.SetActive(true);
        }
        catch
        {
            ConnectionErrorHandler();
        }
    }

    public void sendCommand(float x, float y, string command)
    {
        while (!serverReady) ;

        serverReady = false;
        clientStreamWriter.Write(x.ToString() + ";" + y.ToString() + ";" + command);
        clientStreamWriter.Flush();
    }

    private void Update()
    {
        if (serverReady || tcpClient == null) roboDkIsWorkingPanel.SetActive(false);
        else roboDkIsWorkingPanel.SetActive(true);
    }

    private void WaitForServerMsg()
    {
        while (true)
        {
            if (clientStreamReader.ReadLine().Contains("done"))
            {
                serverReady = true;
            }
        }
    }

    public void ConnectionErrorHandler()
    {
        connectToRoboDKButton.gameObject.SetActive(true);
        connectedText.SetActive(false);
        tcpClient = null;
    }
}
