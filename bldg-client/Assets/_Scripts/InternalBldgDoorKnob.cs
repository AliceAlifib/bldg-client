using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor;
using UnityEngine.Events;
using Models;
using Utils;


public class InternalBldgDoorKnob : MonoBehaviour
{

    private string bldgName = "";
    private string bldgAddress = "";
    private string bldgURL = "";
    private Color initialColor;

    void getContainerBldgDetails()
    {
        initialColor = GetComponent<Renderer>().material.color;
        GameObject bldg = transform.parent.gameObject;
        BldgObject bldgObject = bldg.GetComponent<BldgObject>();
        if (bldgObject != null) {
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
        Debug.Log("~~~~~ Clicked on internal bldg door knob");
        if (bldgName == "") {
            getContainerBldgDetails();
        }
        
        // if (EditorUtility.DisplayDialog ("Exiting " + bldgName, "You're about to exit the " + bldgName + " team HQ.", "Ok", "Cancel")) {
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
        // }
    }
}