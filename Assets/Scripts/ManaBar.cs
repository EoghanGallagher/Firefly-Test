using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;

public class ManaBar : MonoBehaviour 
{


	public Image manaBar;

	void OnEnable()
	{

		Messenger.AddListener< float > ( "AddMana" , IncreaseMana ); //Message from Firefly.cs

	}

	void OnDisable()
	{
		Messenger.RemoveListener< float > ( "AddMana" , IncreaseMana );
	}

	// Use this for initialization
	void Start () 
	{
	
		if ( !manaBar ) 
		{
			Debug.Log ( "manabar game object not set" );
			return;

		}

	}

	void IncreaseMana( float mana )
	{

		float tmp = 0;

		tmp = tmp + mana;

		Debug.Log ( "Mana Bar : " + tmp );

		if (manaBar.rectTransform.sizeDelta.x <= 195.0f) 
		{
			manaBar.rectTransform.sizeDelta += new Vector2 ( mana, 0.0f );
		} 
		else 
		{

			Debug.Log ( "Max Size Reached" );

		}

	}
}
