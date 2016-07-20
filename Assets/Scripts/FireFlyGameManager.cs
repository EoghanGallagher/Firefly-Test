using UnityEngine;
using System.Collections;

public class FireFlyGameManager : MonoBehaviour 
{



	public GameObject[] fireFlies;

	public int fireFlyCount;

	public float remainingTime = 40.0f;


	public int stressEvent;
	public int steadyEvent;
	public int relaxEvent;

	public bool isGameStarted = false; 


	void OnEnable()
	{

		Messenger.AddListener( "FireFlyDrained" , FireFlyCount  ); //Message fromFirefly.cs
		Messenger.AddListener ( "StartCountdown" , StartCountDown ); //Message from PipCrystal.cs
		Messenger.AddListener ( "StopCountdown" , StopCountDown ); //Message from PipCrystal.cs
		Messenger.AddListener < float > ( "Emotion", RecordPipEvent ); //Message From CrystalPipTest.cs
	
	}


	void OnDisable()
	{
	
		Messenger.RemoveListener( "FireFlyDrained" , FireFlyCount  );
		Messenger.RemoveListener ( "StartCountdown" , StartCountDown );
		Messenger.RemoveListener ( "StopCountdown" , StopCountDown ); 
		Messenger.RemoveListener < float > ( "Emotion", RecordPipEvent );

	
	}

	// Use this for initialization
	void Start () 
	{
	

		fireFlies = GameObject.FindGameObjectsWithTag ( "FireFly" );

		fireFlyCount = fireFlies.Length;

	}


	void FireFlyCount()
	{


		fireFlyCount--;

		if (fireFlyCount <= 0) 
		{

			Debug.Log ( "GAME OVER" );

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

		while (remainingTime >= 0) 
		{
			yield return new WaitForSeconds ( 1.0f );
			remainingTime--;
		}


		GameOver ( );

	}



	void GameOver()
	{

		Debug.Log ( "Game Over" );
	
	
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