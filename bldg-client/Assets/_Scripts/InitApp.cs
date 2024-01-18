using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Events;
using TMPro;
using Michsky.UI.Shift;
using Models;
using ImageUtils;


public class InitApp : MonoBehaviour
{

    [Header("Resources")]
    public GameObject baseResidentObject;
    public BldgController bldgController;

    public TMP_Text residentName;
    public TMP_Text residentName2;
    public TMP_Text currentAddress;

    public ModalWindowManager quickActionsDialog;

    public TimedEvent startTimedEvent;
    public Animator splashScreenAnimator;


    // TODO is there a better place for the cameras?
    public CinemachineVirtualCamera flyCamera;
    public CinemachineVirtualCamera walkCamera;
    

    private UnityAction onFlying;
    private UnityAction onWalking;
    private UnityAction onLogin;
    private UnityAction onQuickActions;
    private UnityAction onPromoteOrDemote;
    private UnityAction onRenderingComplete;
	
    // TODO move to shared constants/configuration file
	public float floorStartX = -8f;
	public float floorStartZ = -6f;


    private void startLoadingAnimation() {
        startTimedEvent.StartIEnumerator();
    }

    private void initCurrentResidentUI(Resident rsdt) {
        float height = 0.5F;
        // TODO: remove the following
        // if (rsdt.flr != "g") {
        //     height = 2.5F;  // bldg is larger when inside a bldg, so floor is higher
        // }
        Vector3 baseline = new Vector3(floorStartX, height, floorStartZ);	// WHY? if you set the correct Y, some images fail to display
        baseline.x += rsdt.x;
        baseline.z += rsdt.y;
        Debug.Log("Rendering current resident " + rsdt.alias + " at " + baseline.x + ", " + baseline.z);
        Quaternion qrt = Quaternion.identity;
        qrt.eulerAngles = new Vector3(0, rsdt.direction, 0);
        GameObject rsdtClone = (GameObject) Instantiate(baseResidentObject, baseline, qrt);
        rsdtClone.tag = "Resident";

        walkCamera.Follow = rsdtClone.transform;
        walkCamera.LookAt = rsdtClone.transform;
        flyCamera.Follow = rsdtClone.transform;
        flyCamera.LookAt = rsdtClone.transform;
        ResidentController rsdtObject = rsdtClone.AddComponent<ResidentController>();
        rsdtObject.initialize(rsdt, true);

        // RETURN: replace all of these with event handling on bldg controller
        bldgController.SetCurrentResident(rsdt);
        bldgController.SetCurrentResidentController(rsdtObject);
    }

    private void loadBldgs(Resident rsdt) {
        bldgController.SetAddress(rsdt.flr);        
    }

    private void setLabelsInUI(Resident rsdt) {
        residentName.text = rsdt.alias;
        residentName2.text = rsdt.alias;
        currentAddress.text = rsdt.flr_url;
    }

    private void animateOutOfLogin() {
        Scene scene = SceneManager.GetActiveScene();
        try {
            splashScreenAnimator.Play("Login to Loading");
        } catch (Exception e) {
			Debug.Log("~~~~~~ Failed to animate loading: splashScreenAnimator is `" + splashScreenAnimator + "` " + e.ToString());
		}
        
        ////////////////////////////////
    }

    void OnEnable() {
        onFlying = new UnityAction(OnFlying);
        onWalking = new UnityAction(OnWalking);
        onLogin = new UnityAction(OnLogin);
        onQuickActions = new UnityAction(OnQuickActions);
        onPromoteOrDemote = new UnityAction(OnPromoteOrDemote);
        onRenderingComplete = new UnityAction(OnRenderingComplete);
        EventManager.Instance.StartListening("SwitchToFlying", onFlying);
        EventManager.Instance.StartListening("SwitchToWalking", onWalking);
        EventManager.Instance.StartListening("LoginSuccessful", onLogin);
        EventManager.Instance.StartListening("OpenQuickActions", onQuickActions);
        EventManager.Instance.StartListening("PromoteOrDemote", onPromoteOrDemote);
        EventManager.Instance.StartListening("RenderingComplete", onRenderingComplete);
    }


    void Awake() {    
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (crc.isInitialized()) {
            animateOutOfLogin();

            startLoadingAnimation();

            //initCurrentResidentUI(crc.resident);

            loadBldgs(crc.resident);

            setLabelsInUI(crc.resident);
        }
    }




    private void OnLogin()
    {
        Debug.Log("~~~~~ *********************   Init App - On Login  *********************");

        startLoadingAnimation();
        
        CurrentResidentController crc = CurrentResidentController.Instance;
        if (!crc.isInitialized()) {
            Debug.LogError("This cannot happen - OnLogin called but current resident isn't initialized yet");
            return;
        }

        // initCurrentResidentUI(crc.resident);

        loadBldgs(crc.resident);

        setLabelsInUI(crc.resident);
    }

    private void OnFlying()
    {
        Debug.Log("On Flying");
        flyCamera.gameObject.SetActive(true);
        walkCamera.gameObject.SetActive(false);
    }

    private void OnWalking()
    {
        Debug.Log("On Walking");
        walkCamera.gameObject.SetActive(true);
        flyCamera.gameObject.SetActive(false);
    }

    private void OnQuickActions() {
        quickActionsDialog.ModalWindowIn();
    }

    private void OnPromoteOrDemote() {
        bldgController.reloadContainerBldg();
    }

    private void OnRenderingComplete() {
        Debug.Log("On Rendering Complete");
        initCurrentResidentUI(CurrentResidentController.Instance.resident);
    }

    public static void startWalking()
    {
        EventManager.Instance.TriggerEvent("StartWalking");
    }

    public static void startFlyingLow()
    {
        EventManager.Instance.TriggerEvent("StartFlyingLow");
    }

    public static void startFlyingHigh()
    {
        EventManager.Instance.TriggerEvent("StartFlyingHigh");
    }

    public void setAnimators(Animator sAnimator, TimedEvent stEvent) {
        startTimedEvent = stEvent;
        splashScreenAnimator = sAnimator;
    }
}
