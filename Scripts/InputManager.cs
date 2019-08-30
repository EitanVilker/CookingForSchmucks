using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class InputManager : MonoBehaviour {
    public TextScript theScript;
    public Transform anchor;        // get the transform of the OculusGo Controller device
    public float RotateSpeed;
    public GameObject indicatorObj; // get the object to use to indicate the proposed teleportation spot
    public GameObject player;
	public static UnityAction onTriggerUp = null;
    public static UnityAction onTriggerDown = null;
    public static UnityAction onTouchpadDown = null;
    public static float TriggerValue = 0.0f;
	public float cubeADown;
	public float cubeALeft;
	public float cubeABackward;
	public float cubeBDown;
	public float cubeBLeft;
	public float cubeBBackward;
    public float stoveBoundLeft;
    public float stoveBoundDown;
    public float stoveBoundBackward;
    public float sinkBoundDown;
    public float sinkBoundLeft;
    public float sinkBoundBackward;
    public float indicatorOffset;
    public float MAX_DISTANCE;
    public Material selectedMaterial;        // material to indicate selection
    private Material _unselectedMaterial;   // save to enable restore
    private Vector2 _touchValue;            // touchpad value to indicate direction
    private bool _objSelected;           // is an object currently selected
    private Renderer _curRenderer;
    private GameObject _curSelectedObject;
    private float _playerHeightOffset;
    public bool handsWashed;
    public GameObject sinkParticle;
    public GameObject sinkParticle1;
    public GameObject sinkParticle2;
    public GameObject sinkParticle3;
    public GameObject pit0fire;
    public GameObject pit1fire;
    public GameObject pit2fire;
    public GameObject pit3fire;
    private static int panOut = 0;
    private static bool panFull = false;
    private static bool sinkOccupied = false;
    private static bool sinkOn = false;
    private static bool panOnPit0 = false;
    private static bool panOnPit1 = false;
    private static bool panOnPit2 = false;
    private static bool panOnPit3 = false;
    private static bool sauceOnCounter = false;
    private static bool spoonOut = false;
    private static bool sauceCooking = false;
    private static bool plateOut = false;
    private static bool pastaOut = false;
    private static bool forkOut = false;
    private static bool pastaCooking = false;
    private static bool strainerOut = false;
    private static bool pastaInStrainer = false;
    private string stoveObj0Type;
    private string stoveObj2Type;
    private GameObject stoveObject0;
    private GameObject stoveObject2;
    private GameObject plateObject;
    private GameObject pastaObject;
    public GameObject strainerObject;
    private void Awake(){
        InputManager.onTriggerDown += TriggerDown;
        InputManager.onTriggerUp += TriggerUp;
        InputManager.onTouchpadDown += TouchpadDown;
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy(){
        InputManager.onTriggerDown -= TriggerDown;
        InputManager.onTriggerUp -= TriggerUp;
        InputManager.onTouchpadDown -= TouchpadDown;
    }
    // Use this for initialization
    void Start (){
        _objSelected = false;
        _playerHeightOffset = player.transform.position.y;
        _curSelectedObject = null;
        theScript.setInformationText();
    }
	
	// Update is called once per frame
	void Update () {

        indicatorObj.SetActive(true);

        if (pastaOut && panOut > 0 && theScript.currentStep == 2){
            theScript.setText3();
            theScript.currentStep = 3;
        }
        if (panFull && theScript.currentStep == 3){
            theScript.setText4();
            theScript.currentStep = 4;
        }
        if (((panOnPit0 && pit0fire.activeSelf) | (panOnPit1 && pit1fire.activeSelf) | 
                (panOnPit2 && pit2fire.activeSelf) | (panOnPit3 && pit3fire.activeSelf)) && 
                theScript.currentStep == 4){
            theScript.setText5();
            theScript.currentStep = 5;
        }
        // Require spoonOut to be true here if using spoon
        if (stoveObject0 && stoveObject2 && sauceOnCounter && 
                theScript.currentStep == 5){
            theScript.setText6();
            theScript.currentStep = 6;
        }
        if (theScript.currentStep == 6 && sauceCooking && panOnPit0 && 
                pit0fire.activeSelf && panOnPit2 && pit2fire.activeSelf){
            theScript.setText7();
            theScript.currentStep = 7;
        }
        if (theScript.currentStep == 7 && pastaCooking && plateOut && forkOut){
            theScript.setText8();
            theScript.currentStep = 8;
        }
        if (theScript.currentStep == 8 && strainerOut && pastaInStrainer){
            theScript.setText9();
            theScript.currentStep = 9;
        }

        Ray ray = new Ray(anchor.position, anchor.forward); // create ray from controller
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit)){   
            if (hit.transform.gameObject.CompareTag("sink") && 
                hit.transform.gameObject != _curSelectedObject){

                if (_objSelected){
                    // unselect current object
                    _curRenderer.material = _unselectedMaterial; // reset to first material
                }
                _objSelected = true;
                _curSelectedObject = hit.transform.parent.gameObject;
                _curRenderer = _curSelectedObject.GetComponent<Renderer>();
                _unselectedMaterial = _curRenderer.material; 
                _curRenderer.material = selectedMaterial; // set material to "selected"
            }

            else if ((hit.transform.gameObject.CompareTag("ingredient") | 
                    hit.transform.gameObject.CompareTag("stoveWare") | 
                    hit.transform.gameObject.CompareTag("kitchenware")) && 
                    hit.transform.gameObject != _curSelectedObject){
                
                if (_objSelected){
                    // unselect current object
                    _curRenderer.material = _unselectedMaterial; // reset to first material
                }
                _curSelectedObject = hit.transform.gameObject;
                _curRenderer = _curSelectedObject.GetComponent<Renderer>();
                _unselectedMaterial = _curRenderer.material; 
                _curRenderer.material = selectedMaterial; // set material to "selected"
                _objSelected = true;

                // indicatorObj.SetActive(false); // turn off navigation indicator
            }
            else if (hit.transform.gameObject.CompareTag("ground")){

                // update navigation indicator
                Vector3 temp = new Vector3(hit.point.x, hit.point.y + indicatorOffset, hit.point.z);
                indicatorObj.transform.position = temp;
                if (!indicatorObj.activeSelf) indicatorObj.SetActive(true);
                _curSelectedObject = hit.transform.gameObject;

                if (_objSelected){
                    // unselect object
                    _curRenderer.material = _unselectedMaterial; // reset to first material
                    _objSelected = false;
                    _curSelectedObject = null;
                }
            }
            else if (hit.transform.gameObject.CompareTag("oven") | 
                    hit.transform.gameObject.CompareTag("islandDrawer") | 
                    hit.transform.gameObject.CompareTag("drawer")){
                if (_objSelected){
                    _curRenderer.material = _unselectedMaterial; // reset to first material
                }
                _objSelected = true;
                _curSelectedObject = hit.transform.parent.gameObject;
                _curRenderer = _curSelectedObject.GetComponent<Renderer>();
                _unselectedMaterial = _curRenderer.material; 
                _curRenderer.material = selectedMaterial; // set material to "selected"
                indicatorObj.SetActive(false); // turn off navigation indicator
            }
            else if (hit.transform.gameObject.CompareTag("wall")){
                if (_objSelected){
                    // unselect object
                    _curRenderer.material = _unselectedMaterial; // reset to first material
                    _objSelected = false;
                    _curSelectedObject = null;
                }
                indicatorObj.SetActive(true);
            }
            else if (hit.transform.gameObject.CompareTag("pasta")){
                if (_objSelected){
                    _curRenderer.material = _unselectedMaterial; // reset to first material
                }
                _objSelected = true;
                _curSelectedObject = hit.transform.parent.gameObject;
                _curRenderer = _curSelectedObject.GetComponent<Renderer>();
                _unselectedMaterial = _curRenderer.material; 
                _curRenderer.material = selectedMaterial; // set material to "selected"
            }

        } 
        else{
            // nothing was hit

            if (_objSelected){
                // Try catch used because program will sometimes try to make an already null _curRenderer have no material
                try{   
                    // unselect object
                    _curRenderer.material = _unselectedMaterial; // reset to first material
                }
                catch{
                    Debug.Log("I caught a scurvy criminal!");
                }
                _objSelected = false;
                _curSelectedObject = null;
            }  
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)){
            if (onTriggerDown != null)
                onTriggerDown();
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)){
            if (onTriggerUp != null)
                onTriggerUp();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryTouchpad)){
            if (onTouchpadDown != null)
                onTouchpadDown();
        }

        TriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        _touchValue  = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
    }
    private void TriggerDown()
    {
        if (_objSelected)
        {
            if (theScript.currentStep == 0){
                theScript.setInitialText();
                theScript.currentStep = 1;
            }

            // something was hit
            if (!handsWashed){
                if (_curSelectedObject.name == "tapHandle"){
                    // Turn sink on and allow progression to next step
                    handsWashed = true;
                    theScript.setText2();
                    theScript.currentStep = 2;
                    Animator sinkAnim = _curSelectedObject.GetComponent<Animator>();
                    sinkAnim.SetBool("open", true);
                    sinkParticle.SetActive(true);
                    sinkParticle1.SetActive(true);
                    sinkParticle2.SetActive(true);
                    sinkParticle3.SetActive(true);
                    sinkOn = true;
                }
            }
            else if (_curSelectedObject.name == "tapHandle"){
                // Turn sink on if off and vice versa
                Animator sinkAnim = _curSelectedObject.GetComponent<Animator>();
                if (sinkAnim.GetBool("open")){
                    sinkAnim.SetBool("open", false);
                    sinkParticle.SetActive(false);
                    sinkParticle1.SetActive(false);
                    sinkParticle2.SetActive(false);
                    sinkParticle3.SetActive(false);
                    sinkOn = false;
                }
                else{
                    sinkAnim.SetBool("open", true);
                    sinkParticle.SetActive(true);
                    sinkParticle1.SetActive(true);
                    sinkParticle2.SetActive(true);
                    sinkParticle3.SetActive(true);
                    sinkOn = true;
                }
            }
            else if (_curSelectedObject.CompareTag("islandDrawerParent")){
                Animator islandDrawerAnim = _curSelectedObject.transform.parent.gameObject.GetComponent<Animator>();
                if(islandDrawerAnim.GetBool("Open")){
                    islandDrawerAnim.SetBool("Open", false);
                }
                else{
                    islandDrawerAnim.SetBool("Open", true);
                }
            }
            else if (_curSelectedObject.CompareTag("drawerParent")){
                Animator drawerAnim = _curSelectedObject.GetComponent<Animator>();
                if(drawerAnim.GetBool("Open")){
                    drawerAnim.SetBool("Open", false);
                }
                else{
                    drawerAnim.SetBool("Open", true);
                }
            }
            else if (_curSelectedObject.CompareTag("ovenParent")){
                // Toggle fires when oven button clicked
                Animator ovenAnim = _curSelectedObject.GetComponent<Animator>();
                if(ovenAnim.GetBool("Open")){
                    ovenAnim.SetBool("Open", false);
                    if (_curSelectedObject.name == "backLeftSwitch"){
                        pit0fire.SetActive(false);
                    }
                    else if (_curSelectedObject.name == "frontLeftSwitch"){
                        pit1fire.SetActive(false);
                    }
                    else if (_curSelectedObject.name == "frontRightSwitch"){
                        pit2fire.SetActive(false);
                    }
                    else if (_curSelectedObject.name == "backRightSwitch"){
                        pit3fire.SetActive(false);
                    }
                }
                else{
                    ovenAnim.SetBool("Open", true);
                    if (_curSelectedObject.name == "backLeftSwitch"){
                        pit0fire.SetActive(true);
                    }
                    else if (_curSelectedObject.name == "frontLeftSwitch"){
                        pit1fire.SetActive(true);
                    }
                    else if (_curSelectedObject.name == "frontRightSwitch"){
                        pit2fire.SetActive(true);
                    }
                    else if (_curSelectedObject.name == "backRightSwitch"){
                        pit3fire.SetActive(true);
                    }
                }
            }
        }
        else{
            // check for teleportation
            Ray ray = new Ray(anchor.position, anchor.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, MAX_DISTANCE))
            {
                if (_curSelectedObject.CompareTag("ground"))
                {
                    // transport the player
                    Vector3 newpos = new Vector3(hit.point.x, hit.point.y + _playerHeightOffset - 5.3f, hit.point.z);
                    player.transform.position = newpos;
                }
            }
        }
    }
    private void TriggerUp()
    {
        if (_objSelected)
        {
            if (_curSelectedObject.CompareTag("ingredient")){

                if (_curSelectedObject.name == "Box"){
                    if (!pastaOut){
                        pastaOut = true;
                        // Place object in boxA
                        _curSelectedObject.transform.parent = null;
                        _curSelectedObject.transform.Rotate(180f, 0f, 180f, Space.World);
                        _curSelectedObject.transform.position = new Vector3(cubeALeft + 0.8f, cubeADown + 0.4f, cubeABackward + 0.8f);
                    }
                    else if (theScript.currentStep == 7){
                        GameObject pasta = _curSelectedObject.transform.Find("Pasta").gameObject;
                        GameObject pastaChild = pasta.transform.Find("pastaPiece").gameObject;
                        pastaObject = pastaChild;
                        if (stoveObj0Type == "sauce"){
                            pastaChild.transform.parent = null;
                            pastaChild.transform.position = stoveObject2.transform.position;
                            pastaChild.transform.parent = stoveObject2.transform;
                            pastaCooking = true;
                        }
                        else if (stoveObj2Type == "sauce"){
                            pastaChild.transform.position = stoveObject0.transform.position;
                            pastaChild.transform.parent = stoveObject0.transform;
                            pastaCooking = true;
                        }
                        _curSelectedObject.SetActive(false);
                    }
                }
                else if (_curSelectedObject.name == "sauceJar" && sauceOnCounter){
                    // Put sauce in pot
                    if (stoveObject0){
                        GameObject sauce = stoveObject0.transform.Find("ColdSauce").gameObject;
                        _curSelectedObject.SetActive(false);
                        sauceCooking = true;
                        sauce.SetActive(true);
                        stoveObj0Type = "sauce"; 
                    }
                    else if (stoveObject2 && theScript.currentStep == 6){
                        GameObject sauce = stoveObject2.transform.Find("ColdSauce").gameObject;
                        sauce.SetActive(true);
                        _curSelectedObject.SetActive(false);
                        sauceCooking = true;
                        stoveObj2Type = "sauce";
                    }
                }
                else if(_curSelectedObject.name == "sauceJar"){
                    // Place object in boxA
                    _curSelectedObject.transform.parent = null;
                    _curSelectedObject.transform.position = new Vector3(cubeALeft + 0.8f, cubeADown + 0.4f, cubeABackward + 1.6f);
                    sauceOnCounter = true;
                }

                // let go of object
                _curSelectedObject.transform.parent = null;
                _curSelectedObject = null;

            }
            else if (_curSelectedObject.CompareTag("stoveWare")){

                if (_curSelectedObject.name == "Pan"){
                    if(sinkOccupied | panFull){
                        _curSelectedObject.transform.position = new Vector3(stoveBoundLeft + 0.2f, stoveBoundDown + 0.08f, stoveBoundBackward + 0.35f);
                        stoveObject2 = _curSelectedObject;
                        panOnPit2 = true;
                    }
                    else if (theScript.currentStep < 8){
                        _curSelectedObject.transform.position = new Vector3(sinkBoundLeft + 0.35f, sinkBoundDown + 0.1f, sinkBoundBackward + 0.6f);
                        if (sinkOn){
                            panFull = true;
                            GameObject water = _curSelectedObject.transform.Find("PanWater").gameObject;
                            water.SetActive(true);
                        }
                    }
                    else if (theScript.currentStep == 8 && !pastaInStrainer && strainerOut){
                        pastaObject.transform.parent = null;
                        pastaObject.transform.position = new Vector3(strainerObject.transform.position.x, strainerObject.transform.position.y + 0.1f, strainerObject.transform.position.z);
                        pastaObject.transform.parent = strainerObject.transform;
                        pastaInStrainer = true;
                    }
                }
                else if (_curSelectedObject.name == "Pan2"){
                    if(sinkOccupied | panFull){
                        _curSelectedObject.transform.position = new Vector3(stoveBoundLeft + 0.2f, stoveBoundDown + 0.08f, stoveBoundBackward + 0.95f);
                        stoveObject0 = _curSelectedObject;
                        panOnPit0 = true;
                    }
                    else if (theScript.currentStep < 8){
                        _curSelectedObject.transform.position = new Vector3(sinkBoundLeft + 0.35f, sinkBoundDown + 0.1f, sinkBoundBackward + 0.6f);
                        if (sinkOn){
                            panFull = true;
                            GameObject water = _curSelectedObject.transform.Find("PanWater").gameObject;
                            water.SetActive(true);
                        }
                    }
                    else if (theScript.currentStep == 8 && !pastaInStrainer && strainerOut){
                        pastaObject.transform.parent = null;
                        pastaObject.transform.position = new Vector3(strainerObject.transform.position.x, strainerObject.transform.position.y + 0.3f, strainerObject.transform.position.z);
                        pastaObject.transform.parent = strainerObject.transform;
                        pastaInStrainer = true;
                    }
                }
                panOut += 1;

                // let go of object
                _curSelectedObject = null;
            }
            else if (_curSelectedObject.CompareTag("kitchenware")){

                    if (_curSelectedObject.name == "Strainer" && !sinkOccupied && panFull){
                        // Place strainer in sink if unoccupied
                        sinkOccupied = true;
                       _curSelectedObject.transform.position = new Vector3(sinkBoundLeft + 0.35f, sinkBoundDown + 0.1f, sinkBoundBackward + 0.6f);
                        strainerOut = true;
                        strainerObject = _curSelectedObject;
                    }
                    else if (_curSelectedObject.name == "Strainer" && theScript.currentStep == 9){
                        pastaObject.transform.parent = null;
                        pastaObject.transform.position = plateObject.transform.position;
                        pastaObject.transform.parent = plateObject.transform;
                        plateObject.transform.Find("SaucePlane").gameObject.SetActive(true);
                        plateObject.transform.Find("SaucePlane2").gameObject.SetActive(true);
                    }
                    else{
                        // Place object in boxB
                        if (_curSelectedObject.name == "Fork"){
                            _curSelectedObject.transform.parent = null;
                            _curSelectedObject.transform.position = new Vector3(cubeBLeft + 0.2f, cubeBDown - 0.2f, cubeBBackward + 4.5f);
                            forkOut = true;
                        }
                        else if (_curSelectedObject.name == "Plate"){
                            _curSelectedObject.transform.parent = null;
                            _curSelectedObject.transform.position = new Vector3(cubeBLeft + 0.2f, cubeBDown - 0.2f, cubeBBackward + 4.9f);
                            plateObject = _curSelectedObject;
                            plateOut = true;
                        }
                        else if (_curSelectedObject.name == "scoop"){
                            Debug.Log("Found the scoop");
                            _curSelectedObject.transform.parent.parent = null;
                            _curSelectedObject.transform.parent.position = new Vector3(cubeBLeft + 0.2f, cubeBDown + 0.3f, cubeBBackward + 5.5f);
                            spoonOut = true;
                        }
                    }

                    // let go of object
                    _curSelectedObject = null;
            }
            else if (_curSelectedObject.CompareTag("sink")){
                // let go of object
                _curSelectedObject = null;
                theScript.setText2();
            }
            else if (_curSelectedObject.CompareTag("drawer") | 
                    _curSelectedObject.CompareTag("ovenParent")){
                // let go of object
                _curSelectedObject = null;
            }
        }
    }
    // Rotate player in direction of touchpad press
    private void TouchpadDown()
    {
        float direction = (_touchValue.x > 0.0f) ? 1.0f : -1.0f; // "ternery conditional"
        player.transform.Rotate(direction * Vector3.up * RotateSpeed);
    }
}