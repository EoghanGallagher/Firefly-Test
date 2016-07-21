using UnityEngine;
using System.Collections;



public enum UserStatus
{
	Stressed,
	Relaxed,
	Steady,
	None,

}





public class PipCrystal : MonoBehaviour 
{


	public GameObject[] fireFlies;
	public GameObject crystalModel;

	public int userState;  //1 = Relaxed  2 = Steady 3 = Stressed


	public bool isFirstRun = true;
	public bool isUserActive = true;
	public bool isAscended = false;
	public bool isDescended = true;

	public bool isStressed = false;
	public bool isRelaxed = false;
	public bool isSteady = false;


	public GameObject crystal;
	public GameObject topEnergySource;
	public GameObject bottomEnergySource;


	public Transform target;
	public Vector3 origin;

	private Transform m_Transform;

	[SerializeField]
	private float emotion = 0.0f;

	[SerializeField]
	private float crystalRotationSpeed = 0.0f;

	[SerializeField]
	private float rotationSpeed = 0.0f;

	[SerializeField]
	private float min_emotion_limit;

	[SerializeField]
	private float max_emotion_limit;



	Renderer rend;


	void OnEnable()
	{
		Messenger.AddListener( "Connected" , Connected ); //Message from CrystalPipTest.cs
		Messenger.AddListener<float> ( "Emotion" , SetEmotion );
		Messenger.AddListener<string> ( "PipInput" , ProcessPipInput );
		Messenger.AddListener ( "GAMEOVER" , GameOver );

	}



	void OnDisable()
	{
		Messenger.RemoveListener( "Connected" , Connected );
		Messenger.RemoveListener<float> ( "Emotion" , SetEmotion );
		Messenger.RemoveListener<string> ( "PipInput" , ProcessPipInput );
		Messenger.RemoveListener ( "GAMEOVER" , GameOver );
	
	}


	void Start()
	{



		if ( !crystal ) 
		{

			Debug.LogError ( "Crystal GameObject Not Set" );
			return;

		}

		if ( !target ) 
		{

			Debug.LogError ( "Target GameObject Not Set" );
			return;

		}

		if (!crystalModel) 
		{

			Debug.LogError ( "Crystal Model GameObject Not Set" );
			return;

		}


		if (!topEnergySource || !bottomEnergySource ) 
		{

			Debug.LogError ("Energy Source GameObject(s) Not Set");
			return;

		}

		m_Transform = crystal.transform;

		origin = m_Transform.position;


		rend = crystalModel.GetComponent<Renderer>();
		rend.material.shader = Shader.Find( "Standard" );

		rend.material.color = Color.white;

		fireFlies = GameObject.FindGameObjectsWithTag ( "FireFly" );

	
	}


	void Connected()
	{
			
		StartCoroutine ( "Ascend" );
	
	}

	void ProcessPipInput( string pipInput )
	{
	

		if ( isFirstRun ) 
		{
			StartCoroutine ( "Ascend" );

			isFirstRun = false;
		}


		if ( isAscended ) 
		{
			StartCoroutine ( "RotateCrystal" );
			StartCoroutine( "Broadcast" );
			isAscended = false;
		}


		Debug.Log ( "PipCrystal" );

		if ( pipInput.Equals ( "Relaxing" ) ) 
		{	
		
			Debug.Log ( "Relaxed" );

			userState = 1;

			isStressed = false;
		
		
		} 
		else if ( pipInput.Equals ( "Steady" ) ) 
		{
			Debug.Log ( "Steady" );
			userState = 2;


		} 
		else if ( pipInput.Equals ( "Stressing" ) ) 
		{
			Debug.Log ( "Stressed" );

			isStressed = true;

			userState = 3;
		
		} 
		else 
		{

		
		}

	}



	private IEnumerator Ascend()
	{


		float maxTime = 3.0f;
		float currentTime = 0.0f;
		float increment = 0.025f;

	
		//Ascend
		while ( currentTime <= maxTime ) 
		{


			Move ( target.position , increment );

			yield return new WaitForSeconds ( increment );


			currentTime += increment;
		
		}

		StartCoroutine ( "RotateCrystal" );


		StartCoroutine ( "Pulse" );


 		

	
	}


	private IEnumerator Descend()
	{
	
		float maxTime = 3.0f;
		float currentTime = 0.0f;
		float increment = 0.025f;


		//Descend
		while ( currentTime <= maxTime ) 
		{


			Move ( origin , increment );

			yield return new WaitForSeconds ( increment );


			currentTime += increment;

		}

		isDescended = true;
		isAscended = false;
	
	}


	private IEnumerator Broadcast()
	{

		yield return new WaitForSeconds ( 2.0f );

		bool energySourceActive = false;
	

		while ( true ) 
		{
			if ( this.emotion < min_emotion_limit ) 
			{
				Debug.Log ( "BROADCASTING TO FIRE FLIES" );

				if( !energySourceActive )
				{
					topEnergySource.SetActive ( true );
					//bottomEnergySource.SetActive ( true );
					energySourceActive = true;
				}

				Messenger.Broadcast ( "ManaRequest" ); 

			} 
			else if( this.emotion >= max_emotion_limit )
			{
				//topEnergySource.SetActive ( true );
				//bottomEnergySource.SetActive ( true );

				//Messenger.Broadcast ( "StopRequest" ); 
			}

			yield return new WaitForSeconds ( 1.0f );

		}

	}


	private IEnumerator RotateCrystal()
	{
	
	
		float maxTime = 3.0f;
		float currentTime = 0.0f;
		float increment = 0.025f;

		//Rotate



		while ( true ) 
		{

			Rotate ( m_Transform, rotationSpeed );

			yield return new WaitForSeconds ( increment );

			currentTime += increment;

			if(  !isUserActive )
			{
				break;
			}


		}
	
	
	}

	private void Move( Vector3 target ,  float timer )
	{
	
		float speed = 0.5f;

		float step = speed * timer;
		m_Transform.position = Vector3.MoveTowards( m_Transform.position, target, step);
	
	}


	private void Rotate( Transform target , float speed )
	{

		target.Rotate( 0, speed * Time.deltaTime , 0 );
	
	}

	private void ChangeCrystalColour( float colourChange )
	{

		rend.material.color = Color.Lerp( Color.red, Color.green,  colourChange ); 

	}


	private void SetEmotion( float value )
	{
	


		this.emotion = value;

		ChangeCrystalColour ( this.emotion );


		float speedMultiplyer = ( 1 - emotion) * 10;

		rotationSpeed = 20 * speedMultiplyer;
	
	}


	IEnumerator Pulse()
	{

		int count = 3; 

		while (count >= 0) 
		{

			Debug.Log ( "Firing Pulse to Reveal Fire Flies" );

			//Pulse Effects fired here.

			yield return new WaitForSeconds ( 1.0f );

			count--;

		}

		//Send message to reveal the Fire Flies
		Messenger.Broadcast ( "Activate" );

		yield return new WaitForSeconds ( 1.0f );

		StartCoroutine ( "Broadcast" );

		//Recipient FireFlyGameManager
		Messenger.Broadcast ( "StartCountdown" );


	}


	void GameOver()
	{


		Debug.Log ( "Game OVER CALLED" );
		topEnergySource.SetActive ( false );

	}

	

}
