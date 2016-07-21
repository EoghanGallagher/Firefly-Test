using UnityEngine;
using System.Collections;

public class SpawnControlller : MonoBehaviour {


	public GameObject fireFly;

	public float minX, maxX, minY, maxY;


	public int fireFlyCount;
	public int tmpFireFlyCount;


	void OnEnable()
	{
		Messenger.AddListener ( "StartGame" , StartGame );
		Messenger.AddListener( "FireFlyDrained" , UpdateFireFlyCount  ); //Message fromFirefly.cs
		Messenger.AddListener( "EndGame" , EndGame );

	}

	void OnDisable()
	{
		Messenger.RemoveListener ( "StartGame" , StartGame );
		Messenger.RemoveListener( "FireFlyDrained" , UpdateFireFlyCount  ); //Message fromFirefly.cs
		Messenger.RemoveListener( "EndGame" , EndGame );
	}

	// Use this for initialization
	void Start () 
	{
	
		if ( !fireFly ) 
		{

			Debug.Log ( "Firefly Gameobject is missing" );
			return;

		}

	}




	void StartGame()
	{
		StartCoroutine ( Spawner() );
	}


	void EndGame()
	{
	
		StopCoroutine ( Spawner() );
	
	}

	private IEnumerator Spawner()
	{

		int tmpCount;

	

		for ( int i = 0; i < fireFlyCount; i++ ) 
		{

			CreateFireFly ();

			yield return new WaitForSeconds( 1.0f );


		}


		tmpFireFlyCount = fireFlyCount;



		while ( true ) 
		{

			if ( fireFlyCount > 1 && fireFlyCount < 8  ) 
			{

				CreateFireFly ();

				fireFlyCount ++;

			}

			yield return new WaitForSeconds ( 1.0f );
		}

	}


	private void CreateFireFly()
	{

		float x, y, z = 0;

		//Get Random Spawnpoint Coords
		x = Random.Range ( minX, maxX );
		y = Random.Range ( minY , maxY );

		GameObject clone = Instantiate ( fireFly , new Vector3( x , y , 0 ) , fireFly.transform.rotation  ) as GameObject;

		FireFly fly = clone.GetComponent<FireFly> ();

		fly.state = FireFly.State.Moving;
		fly.isMoving = false;

		clone.gameObject.SetActive ( true );

	}


	private void UpdateFireFlyCount()
	{
	
		fireFlyCount--;
	
	}
	

}
