using UnityEngine;
using System.Collections;

public class FireFlyGameManager : MonoBehaviour 
{



	public GameObject[] fireFlies;

	public int fireFlyCount;

	void OnEnable()
	{
	
		Messenger.AddListener( "FireFlyDrained" , FireFlyCount  );
	
	}


	void OnDisable()
	{
	
		Messenger.RemoveListener( "FireFlyDrained" , FireFlyCount  );
	
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
	

}