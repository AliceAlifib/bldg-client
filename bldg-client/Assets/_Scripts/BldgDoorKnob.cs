using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using UnityEngine.Events;
using Models;


public class BldgDoorKnob : MonoBehaviour
{

    public int level = 0;

    private string bldgName = "";
    private string bldgAddress = "";
    private string bldgURL = "";

    private Color initialColor;


    // Start is called before the first frame update
    void Start()
    {
        initialColor = GetComponent<Renderer>().material.color;
        GameObject bldg = transform.parent.gameObject;
        BldgObject bldgObject = bldg.GetComponent<BldgObject>();
        if (bldgObject != null) {
            Debug.Log("[DoorKnob] parent bldg object: " + bldgObject);
            bldgName = bldgObject.model.name;
            bldgAddress = bldgObject.model.address;
            bldgURL = bldgObject.model.bldg_url;
        }
    }

    void OnMouseOver()
    {
        GetComponent<Renderer>().material.color = Color.cyan;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = initialColor;
    }

    void OnMouseDown() {
        Debug.Log("~~~~~ Clicked on bldg door knob");
        // if (EditorUtility.DisplayDialog ("Entering " + bldgName, "You're about to enter the " + bldgName + " team HQ. Please note that due to the Alice effect, everything is 10x smaller inside buildings.", "Ok", "Cancel")) {
        // check whether this door knob is at the current flr, in which case we need to exit the bldg
        string doorKnobAddress = bldgAddress + "/l" + level;
        if (doorKnobAddress != CurrentResidentController.Instance.resident.flr) {
            Debug.Log("~~~~ Door Knob of another bldg was clicked, so we're entering that bldg");
            EventManager.Instance.TriggerEvent("EnteringBldg");
            Debug.Log("Invoking enter bldg action");
            CurrentResidentController crc = CurrentResidentController.Instance;
            Debug.Log("Sending enter bldg action for resident " +  crc.resident.email);
            crc.SendEnterBldgFlrAction(new EnterBldgFlrAction() {
                resident_email = crc.resident.email,
                action_type = "ENTER_BLDG_FLR",
                bldg_address = bldgAddress,
                bldg_url = bldgURL,
                flr_level = level
            });
        } else {
            Debug.Log("~~~~ Door Knob of current bldg was clicked, so we're exiting that bldg");
            EventManager.Instance.TriggerEvent("ExitingBldg");
            Debug.Log("Invoking exit bldg action");
            CurrentResidentController crc = CurrentResidentController.Instance;
            Debug.Log("Sending exit bldg action for resident " +  crc.resident.email);
            crc.SendExitBldgAction(new ExitBldgAction() {
                resident_email = crc.resident.email,
                action_type = "EXIT_BLDG",
                bldg_address = bldgAddress,
                bldg_url = bldgURL
            });
        }
    }
}