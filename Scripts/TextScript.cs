using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextScript : MonoBehaviour {

	public TMP_Text myText;
	public int currentStep;

	// Use this for initialization
	void Start () {
		currentStep = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void setInformationText(){
		myText.text = "Point with your controller and click to select an object. Use the touchpad to rotate. Click any object to begin.";
	}
	public void setInitialText(){
		myText.text = "First, wash your hands. Click on the sink handle to do so.";
	}
	public void setText2(){
		myText.text = "Now, find a saucepan in the island and some pasta from a top cabinet.";
	}
	public void setText3(){
		myText.text = "Our next step is to boil some water. Go to the sink and fill up the saucepan.";
	}
	public void setText4(){
		myText.text = "Place the pan on the stove, then turn on the correct fire beneath it.";
	}
	public void setText5(){
		// If using spoon add line for it here
		myText.text = "In the meantime, get tomato sauce from the fridge and another saucepan.";
	}
	public void setText6(){
		myText.text = "Put the saucepan on the fire like before and add the sauce.";
	}
	public void setText7(){
		myText.text = "Great! The water has boiled. Now add some pasta and get a plate and fork from the island.";
	}
	public void setText8(){
		myText.text = "Put the pasta in the strainer in the sink.";
	}
	public void setText9(){
		myText.text = "Place some pasta on a plate, add some sauce, and dig in!";
	}
}