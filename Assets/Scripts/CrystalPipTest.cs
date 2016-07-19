using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Galvanic.PipPlugin;

public class CrystalPipTest : MonoBehaviour 
{


	private const int INVALID_PIP_ID = -1;

	GameObject m_panelMain;
	GameObject m_popupPairingRequired;

	private PipDataFilter pipDataFilter;


	/*Text m_textStatus;
	Text m_textVersion;
	Text m_textBatteryLevel;
	Text m_textBatteryStatus;
	Text m_textStressEventType;
	Button m_buttonDiscover;
	Button m_buttonConnect;
	Button m_buttonSetName;
	Button m_buttonPing;
	Button m_buttonPowerOff;
	Button m_buttonReset;*/
	//InputField m_inputName;

	Button m_buttonDiscover;
	Button m_buttonConnect;
	Button m_buttonPowerOff;


	Pip m_pip = null;

	void Start () 
	{
		PipManager.Instance.PipManagerReady += new PipManagerEvent(PipManagerReady);
		PipManager.Instance.PipDiscoveryComplete += new PipManagerEvent(PipDiscoveryComplete);
		PipManager.Instance.PipDiscovered += new PipManagerEvent(PipDiscovered);

		m_buttonDiscover = GameObject.Find("ButtonDiscover").GetComponent<Button>();
		m_buttonConnect = GameObject.Find("ButtonConnect").GetComponent<Button>();
		m_buttonPowerOff = GameObject.Find("ButtonPowerOff").GetComponent<Button>();

		/*m_buttonPing = GameObject.Find("ButtonPing").GetComponent<Button>();
		m_buttonPowerOff = GameObject.Find("ButtonPowerOff").GetComponent<Button>();
		m_buttonReset = GameObject.Find("ButtonReset").GetComponent<Button>();
		m_buttonSetName = GameObject.Find("ButtonSetName").GetComponent<Button>();

		m_textStatus = GameObject.Find("TextStatus").GetComponent<Text>();
		m_textVersion = GameObject.Find("TextVersion").GetComponent<Text>();
		m_textBatteryLevel = GameObject.Find("TextBatteryLevel").GetComponent<Text>();
		m_textBatteryStatus = GameObject.Find("TextBatteryStatus").GetComponent<Text>();
		m_textStressEventType = GameObject.Find("TextStressEventType").GetComponent<Text>();

		m_inputName = GameObject.Find("InputFieldName").GetComponent<InputField>();*/

		ClearInfoText();

		//PopupMessageController.Instance.panel = GameObject.Find("PanelPopup");
		m_panelMain = GameObject.Find("PanelMain");

		DisableInteraction();
		m_buttonDiscover.interactable = true;
		//m_panelMain.SetActive(true);
		PipDebug.Enable(true);


		pipDataFilter = new PipDataFilter();
	}

	void DisableInteraction()
	{
		/*m_buttonDiscover.interactable = false;
		m_buttonConnect.interactable = false;
		m_buttonPing.interactable = false;
		m_buttonPowerOff.interactable = false;
		m_buttonReset.interactable = false;
		m_buttonSetName.interactable = false;*/
	}

	void ClearInfoText()
	{
		/*m_textVersion.text = "";
		m_textBatteryLevel.text = "";
		m_textStressEventType.text = "";
		m_inputName.text = "";
		m_textStatus.text = "";
		m_textBatteryStatus.text = "";*/
	}

	public void OnButtonDiscoverClicked()
	{
		Debug.Log("Discovering Pips...");
		PipManager.Instance.ResetManager();
		m_pip = null;
		DisableInteraction();
		ClearInfoText();
		//m_textStatus.text = "Discovering Pips...";   
		PipManager.Instance.DiscoverPips();
	}

	public void OnButtonConnectClicked()
	{

		Messenger.Broadcast ( "Connected" );

		string text = m_buttonConnect.GetComponentInChildren<Text>().text;
		if ( text == "Connect")
		{
			Debug.Log("Connecting to Pip...");
			//m_textStatus.text = "Connecting to Pip...";
			if (m_pip != null)
			{
				//ClearInfoText();
				DisableInteraction();
				m_pip.Connect();
			}
		}
		else
		{
			Debug.Log("Disconnecting from Pip...");
			if (m_pip != null)
			{
				DisableInteraction();
				m_pip.Disconnect();
			}
		}
	}

	public void OnButtonSetNameClicked()
	{
		/*string name = m_inputName.text;
		if ( !PipManager.Instance.IsValidPipName(name) )
			m_textStatus.text = "Invalid name.";
		else
			m_pip.RequestSetName(name);  */   
	}

	public void OnButtonPingClicked()
	{
		m_pip.RequestPing(true);
	}

	public void OnButtonResetClicked()
	{
		m_pip.RequestReset();
		m_buttonConnect.GetComponentInChildren<Text>().text = "Connect";
		DisableInteraction();
		m_buttonDiscover.interactable = true;
	}

	public void OnButtonPowerOffClicked()
	{
		m_pip.RequestPowerOff();
		m_buttonConnect.GetComponentInChildren<Text>().text = "Connect";
		DisableInteraction();
		m_buttonDiscover.interactable = true;
	}

	public void OnPopupPairingRequired_OKClicked()
	{
	}

	public void PipManagerReady(object sender, PipManagerEventArgs e)
	{
		//m_textStatus.text = "Pip system ready";
	}

	public void PipDiscovered(object sender, PipManagerEventArgs e)
	{
		m_pip = e.Pip;

		m_pip.ConnectCompleted += new PipEvent(PipConnectCompleted);
		m_pip.Disconnected += new PipEvent(PipDisconnected);
		m_pip.VersionAvailable += new PipEvent(PipVersionAvailable);
		m_pip.PingCompleted += new PipEvent(PipPingCompleted);
		m_pip.NameAvailable += new PipEvent(PipNameAvailable);
		m_pip.SetNameCompleted += new PipEvent(PipSetNameCompleted);
		m_pip.BatteryStatusChanged += new PipEvent(PipBatteryStatusChanged);
		m_pip.BatteryLevelAvailable += new PipEvent(PipBatteryLevelAvailable);
		m_pip.AnalyzerOutputAvailable += new PipEvent(PipAnalyzerOutputAvailable);
		m_pip.ActivationChanged += new PipEvent(PipActivationChanged);
	


		PipManager.Instance.CancelPipDiscovery();
	}

	public void PipDiscoveryComplete(object sender, PipManagerEventArgs e)
	{
		List<Pip> pips = PipManager.Instance.DiscoveredPips;
		DisableInteraction();

		if (pips.Count > 0)
		{
			m_pip = pips[0];
			string status = "Discovered Pip, id: " + m_pip.PipID.ToString("x");
			if (m_pip.Name.Length > 0)
				status += ", name: " + m_pip.Name;
			//m_textStatus.text = status;
			m_buttonConnect.interactable = true;
		}
		else
		{
			//m_textStatus.text = "Discovery complete.";

			Debug.Log ( "Discovery Complete" );
		}

		m_buttonDiscover.interactable = true;
	}

	public void PipConnectCompleted(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		switch (e.Result)
		{
		case PipResult.Succeeded:
			//m_textStatus.text = "Connected";
			m_buttonConnect.GetComponentInChildren<Text>().text = "Disconnect";
			m_buttonConnect.interactable = true;
			//m_buttonPing.interactable = true;
			//m_buttonReset.interactable = true;
			//m_buttonPowerOff.interactable = true;
			//m_buttonSetName.interactable = true;

			//m_textBatteryStatus.text = "Normal";
			if (m_pip.Name.Length == 0)
				pip.RequestGetName();

			pip.RequestVersion();
			break;
		case PipResult.PairingFailed:
			PopupMessageController.Instance.AddPopup("Connection failed.", "Please place your Pip in pairing mode.");
			m_buttonDiscover.interactable = true;
			m_buttonConnect.interactable = true;
			//m_textStatus.text = "Pairing failed";
			break;
		default:
			m_buttonDiscover.interactable = true;
			m_buttonConnect.interactable = true;
			//m_textStatus.text = "Connection failed";
			break;
		}
	}

	void PipDisconnected(Pip pip, PipEventArgs e)
	{
		Debug.Log("UnityPipExample.PipDisconnected");
		if (pip != m_pip)
			return;

		//m_textStatus.text = "Disconnected";
		m_buttonConnect.interactable = true;
		m_buttonConnect.GetComponentInChildren<Text>().text = "Connect";
		m_buttonDiscover.interactable = true;
	}

	void PipVersionAvailable(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		//m_textVersion.text = pip.Version;
	}

	public void PipNameAvailable(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		//m_inputName.text = pip.Name;
	}

	public void PipSetNameCompleted(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		//m_textStatus.text = "Pip name changed.";
	}

	public void PipAnalyzerOutputAvailable(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		string trend = "";

		pipDataFilter.setEvent( pip.StressTrend );

		Emotion();

		/*switch (pip.StressTrend)
		{
			case PipStressTrend.Relaxing:
				trend = "Relaxing";

				break;
			
			case PipStressTrend.Stressing:
				trend = "Stressing";

				break;
			
			case PipStressTrend.Steady:
				trend = "Steady";

				break;
			
			case PipStressTrend.None:
				trend = "None";

				break;




		}*/

		/*if ( !trend.Equals ( "None" )) 
		{
			Messenger.Broadcast<string> ( "PipInput", trend );
		}*/

		//m_textStressEventType.text = trend;
	}


	private void Emotion()
	{
		var emotion = ( ( ( pipDataFilter.getCurrentValue() + 1 ) / 2 ) );

		Messenger.Broadcast< float > ( "Emotion", emotion );

		//EvaluateEmotion( emotion );
	}

	private void EvaluateEmotion( float emotion )
	{
		if (emotion < 0.4f)
		{
			
			return;
		}

		if (emotion < 0.7f)
		{
			
			return;
		}

		if (emotion < 0.9f)
		{
			
			return;
		}




	}

	public void PipActivationChanged(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		string status = "Inactive";
		if (pip.Active)
			status = "Active";

		//m_textStatus.text = status;
	}

	public void PipBatteryLevelAvailable(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		//m_textBatteryLevel.text = pip.BatteryLevel.ToString() + "%";
	}

	public void PipBatteryStatusChanged(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		Debug.Log ("Battery status: " + m_pip.BatteryStatus);

		switch (m_pip.BatteryStatus) 
		{
		case PipBatteryStatus.BatteryOK:
			//m_textBatteryStatus.text = "Normal";
			break;
		case PipBatteryStatus.BatteryLow:
			//m_textBatteryStatus.text = "Low"; 
			break;
		case PipBatteryStatus.BatteryCritical:
			//m_textBatteryStatus.text = "Critical";
			break;
		case PipBatteryStatus.ChargerAttached:
			//m_textBatteryStatus.text = "Charging";
			break;
		case PipBatteryStatus.ChargerDetached:
			//m_textBatteryStatus.text = "Not charging";
			break;
		}
	}

	public void PipPingCompleted(Pip pip, PipEventArgs e)
	{
		if (pip != m_pip)
			return;

		//m_textStatus.text = "Ping received.";
	}


	public void SuspendPip()
	{

		Debug.Log ( "PIP SUSPENDED" );
		
	}



	void Update()
	{

		pipDataFilter.update ( Time.deltaTime );

		Emotion ();
		
	
	}





}
