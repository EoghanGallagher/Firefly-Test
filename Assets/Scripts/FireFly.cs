using UnityEngine;
using System.Collections;
using DG.Tweening;







public class FireFly : MonoBehaviour 
{


	public enum State
	{
		FadeIn,
		Idle,
		Moving,
		Pulsing

	}


	public float mana;
	public float emotion;
	public float scoreValue;

	public State state = State.Idle;





	public float xmin, xmax, ymin, ymax;

	public float time;

	public GameObject lightningBolt;
	public GameObject body;


	public Renderer glow;
	public Renderer centre;

	public Camera camera;


	public bool isMoving = false;
	public bool isIdle = false;
	public bool isPulsing = false;

	bool isLightningBoltActive = false;


	//Array of Waypoints 
	public Vector3[] wayPoints = new Vector3[ 5 ];


	void OnEnable()
	{

		Messenger.AddListener ( "Activate", Activate );  	//Message From PipCrystal.cs
		Messenger.AddListener ( "ManaRequest", SendMana );	//Message From PipCrystal.cs
		Messenger.AddListener ( "StopRequest", StopMana );	//Message From PipCrystal.cs
		Messenger.AddListener < float > ( "Emotion", Emotion ); //Message From CrystalPipTest.cs
	}

	void OnDisable()
	{
	
		Messenger.RemoveListener ( "Activate", Activate );
		Messenger.RemoveListener ( "ManaRequest", SendMana );
		Messenger.RemoveListener ( "StopRequest", StopMana );
		Messenger.RemoveListener < float > ( "Emotion", Emotion );

	}


	// Use this for initialization
	void Start () 
	{
		if (!lightningBolt) 
		{
			Debug.Log ( "Lightning Bolt Game Object Not Set" );
			return;
		}


		if ( !camera ) 
		{
			
			camera = Camera.main;
			 
		}

		Boundaries ();


	    SetMaterialAlpha ();
		SetPosition ();

		state = State.Moving;


		mana = Mathf.Floor( Random.Range ( 10.0f, 20.0f ) );

	}



	void Update()
	{

		FSM ();
	
	}


	void SendMana()
	{

		state = State.Pulsing;

		if (!isLightningBoltActive) 
		{
			lightningBolt.SetActive ( true );
			isLightningBoltActive = true;
		}


		//Send Mana to the ManaBar
		//Listeners:  ManaBar.cs

		if ( emotion <= 0.4 ) 
		{
		
			scoreValue = -1;
		
		} 
		else if ( emotion > 0.4f && emotion <= 0.7f ) 
		{
			scoreValue = 1;
		}
		else
		{
			scoreValue = 3;
		}


		//Recipient ManaBar.cs
		Messenger.Broadcast< float > ( "AddMana" , scoreValue );

		mana--;

		if ( mana <= 0 ) 
		{


			Messenger.Broadcast ( "FireFlyDrained" );
			this.gameObject.SetActive ( false );

		}
	
	}

	void StopMana()
	{
		state = State.Idle;
	
		lightningBolt.SetActive ( false );
	
	}


	void Move()
	{

		isIdle = false;
		isPulsing = false;


		float movementTime = Random.Range ( 15, 30 );

		float xAxis = 0.0f;
		float yAxis = 0.0f;

		//Generate some Random wayPoints 
		for (int x = 0; x < wayPoints.Length; x++) 
		{
			xAxis = Random.Range ( xmin , xmax );
			yAxis = Random.Range ( ymin , ymax );

			wayPoints[ x ] = new Vector3( xAxis , yAxis , 0 );

		}

		transform.DOPath ( wayPoints, movementTime, PathType.CatmullRom, PathMode.Sidescroller2D, 10, Color.red );


		//yield return new WaitForSeconds ( movementTime );

		state = State.Idle;

	
	}


	void FadeIn()
	{


	
	}

	IEnumerator Idle()
	{
		isMoving = false;
		isPulsing = false;

		yield return new WaitForSeconds ( Random.Range( 1 , 12 ) );

		state = State.Moving;




	}


	void Pulse()
	{
	
		//Firefly is Stationary and Pulses
		isMoving = false;
		isIdle = false;

		Move ();
	}

	void Boundaries()
	{
	
		Bounds bounds = CameraExtensions.OrthographicBounds ( camera );

		xmin = bounds.min.x;
		xmax = bounds.max.x;




		ymin = bounds.min.y;
		ymax = bounds.max.y;


		if (ymin < 0.0f) 
		{

			ymin = ymin + 4;

		}

	}

	void SetMaterialAlpha(  )
	{

	}

	void SetPosition()
	{
		transform.position = new Vector3 ( Random.Range( xmin, xmax ) , Random.Range( ymin , ymax )  , 0  );
	}


	void FSM()
	{
		switch ( state ) 
		{

			case State.FadeIn:

			break;


			case State.Idle:

				if (!isIdle) 
				{
					Debug.Log ("I'm Idle.");
					StartCoroutine ( "Idle" );
					isIdle = true;
				}
			
			break;
			
			case State.Moving:

				if ( !isMoving ) 
				{
					Debug.Log ("I'm Moving.");
					Move();
					isMoving = true;
				}

			break;
			
			case State.Pulsing:

				if ( !isPulsing ) 
				{
					Debug.Log ("I'm Pulsing.");
					Pulse ();
					isPulsing = true;
				}

			break;

		}
	

	}

	void Activate()
	{

		body.SetActive ( true );

	}


	void Emotion( float emotionLevel )
	{
		this.emotion = emotionLevel;
	}
	

}
