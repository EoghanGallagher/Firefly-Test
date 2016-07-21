using UnityEngine;
using System.Collections;
using Galvanic.PipPlugin;
using UnityEngine.UI;

public class FireFlyGameManager : MonoBehaviour 
{



	public GameObject[] fireFlies;

	public int fireFlyCount;

	public float remainingTime = 40.0f;


	public int stressEvent;
	public int steadyEvent;
	public int relaxEvent;

	public bool isGameStarted = false; 

	public Text timer;


	void OnEnable()
	{

		Messenger.AddListener ( "StartCountdown" , StartCountDown ); //Message from PipCrystal.cs
		Messenger.AddListener ( "StopCountdown" , StopCountDown ); //Message from PipCrystal.cs
		Messenger.AddListener < float > ( "Emotion", RecordPipEvent ); //Message From CrystalPipTest.cs
	
	
	}


	void OnDisable()
	{
	
		Messenger.RemoveListener ( "StartCountdown" , StartCountDown );
		Messenger.RemoveListener ( "StopCountdown" , StopCountDown ); 
		Messenger.RemoveListener < float > ( "Emotion", RecordPipEvent );

	
	}

	// Use this for initialization
	IEnumerator Start () 
	{
	
		//fireFlies = GameObject.FindGameObjectsWithTag ( "FireFly" );

		//fireFlyCount = fireFlies.Length;

		yield return new WaitForSeconds ( 2.0f );



		Messenger.Broadcast ( "StartGame" );


	}

	private void DiscoverPip()
	{

	}


	private void ConnectPip()
	{

	}


	void FireFlyCount()
	{

		fireFlyCount--;

		if (fireFlyCount <= 0) 
		{

			GameOver ( );


		}


	}


	void StartCountDown()
	{
		StartCoroutine ( CountDown( ) );
	}


	void StopCountDown()
	{
		StopCoroutine ( CountDown( ) );
	}



	IEnumerator CountDown( )
	{

		isGameStarted = true;

		while (remainingTime > 0) 
		{
			yield return new WaitForSeconds ( 1.0f );
			remainingTime--;

			timer.text = remainingTime.ToString();
		}


		GameOver ( );

	}



	void GameOver()
	{

		Debug.Log ( "Game Over" );

		Messenger.Broadcast ( "GAMEOVER" );  
		//Recipient(s)
		//PipCrystal
		//FireFly

 	
	
	}

	void RecordPipEvent( float emotion )
	{

		if (!isGameStarted) 
		{

			return;

		}

		if (emotion <= 0.4f) //Stress Event
		{
			stressEvent++;
		} 
		else if (emotion > 0.4f && emotion <= 0.7f) //Steady Event
		{
			steadyEvent++;
		} 
		else //Relaxed Event
		{
			relaxEvent++;
		}


	}
	

}