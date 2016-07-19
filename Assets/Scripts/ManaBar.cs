using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG;

public class ManaBar : MonoBehaviour 
{


	public Image manaBar;

	void OnEnable()
	{

		Messenger.AddListener< float > ( "AddMana" , IncreaseMana );

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

		if (manaBar.rectTransform.sizeDelta.x <= 195.0f) {
			manaBar.rectTransform.sizeDelta += new Vector2 (mana, 0.0f);
		} 
		else 
		{



		}

	}
}
