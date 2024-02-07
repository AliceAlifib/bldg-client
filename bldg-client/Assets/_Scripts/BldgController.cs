using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using ImageUtils;
using BrowserUtils;
using Models;
using Proyecto26;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


public class BldgController : MonoBehaviour
{
	public string DEFAULT_BLDG = "fromteal.app";

	public float floorStartX = -8f;
	public float floorStartZ = -6f;

	public List<string> SIZE_CATEGORIES = new List<string>() {"xs", "s", "m", "l", "xl", "2xl", "3xl"}; 

	public GameObject baseResidentObject;
	public GameObject roadObject;

	public GameObject contextMenu;

	public Texture2D relocateCursorTexture;

	public string targetURL;

	public BldgObject clickedObject;
    public Bldg clickedModel; 

	

    // SHAPES
	[Header("Bldg Templates")]
    // TODO: change to list
	public GameObject whiteboard;
	public GameObject presentationStand;
	public GameObject trafficSign;
	public GameObject streetSign;	
	public GameObject greenLot;
	public GameObject blueLot;
	public GameObject yellowLot;
	public GameObject buildingWithStorefront;
	// ----
	public GameObject coffeeTable;
	public GameObject gardenRoundTable;
	public GameObject livingRoomTable;
	// -----
	public GameObject couch;
	public GameObject sofa;
	public GameObject fancyChair;
	public GameObject coffeeTableChair;
	// -----
	public GameObject standingLight;
	public GameObject candle;
	public GameObject fireplace;
	// -----
	public GameObject pottedPlant;
	public GameObject tree;
	public GameObject largePottedPlant;
	// -----
	public GameObject tableWithDrawers;
	public GameObject chestOfDrawers;
	public GameObject bookCabinet;
	// -----
	public GameObject owlSculpture;
	public GameObject headSculpture;
	// -----
	public GameObject tv;


	bool isRelocating = false;
	BldgObject relocatedObject; 
	bool isShowingContextMenu = false;
	bool isReloadingInLoop = false;
	bool isRenderingCompleteEventFired = false;

    // public ChaseCamera camera;

    string currentAddress;
	string currentFlr;

	BldgChatController bldgChatController;

	Stack<string> addressStack = new Stack<string>();
	IDictionary<string, Vector3> addressToLocation = new Dictionary<string, Vector3>();
	IDictionary<string, float> addressToFlrHeight = new Dictionary<string, float>();
	IDictionary<string, float> addressToFlr0Height = new Dictionary<string, float>();


	// RETURN:
	//private UnityAction onLogin;


    // Start is called before the first frame update
    void Start()
    {
    	// #if !UNITY_EDITOR && UNITY_WEBGL
		// 	WebGLInput.captureAllKeyboardInput = false;
		// #endif
        Debug.Log("BldgController Started");

		bldgChatController = gameObject.GetComponent<BldgChatController>();
        
		// RETURN:
		//onLogin = new UnityAction(OnLogin);
		//EventManager.Instance.StartListening("LoginSuccessful", onLogin);

        // if (currentAddress == null) {
			// TODO change to user's home bldg (received from login?)
			// SetAddress("g");

//			string lastAddress = PlayerPrefs.GetString ("currentAddress");
//			if (lastAddress != null && lastAddress != "") {
//				SetAddress(lastAddress);
//			} else {
        		// Debug.Log("Going to default bldg: " + DEFAULT_BLDG);
				// EnterBuilding(DEFAULT_BLDG);
//			}
		// }

    }

	// RETURN:
	// private void OnLogin()
    // {
	// 	Debug.Log("~~~~ Bldg Controller On Login");
    //     CurrentResidentController crc = CurrentResidentController.Instance;
	// 	SetCurrentResident(crc.resident);
	// 	SetAddress(crc.resident.flr);
	// }



	GameObject getPrefabByEntityClass(string entity_type) {
		Debug.Log("Getting prefab for entity class " + entity_type);
		
		switch (entity_type) {
			case "purpose":
				return whiteboard;
			case "cantata":
				return presentationStand;
			case "neighborhood":
				return trafficSign;
			case "street":
				return streetSign;
			case "lot":
				return greenLot;
			case "green-lot":
				return greenLot;
			case "blue-lot":
				return blueLot;
			case "yellow-lot":
				return yellowLot;
			case "team":
				return buildingWithStorefront;
			// ----
			case "goal":
				return coffeeTable;
			case "milestone":
				return gardenRoundTable;
			case "problem":
				return livingRoomTable;
			// ----
			case "solution":
				return couch;
			case "product":
				return sofa;
			case "service":
				return fancyChair;
			case "capability":
				return coffeeTableChair;
			// ----
			case "member":
				return standingLight;
			case "customer":
				return candle;
			case "community":
				return fireplace;
			// ----
			case "task":
				return pottedPlant;
			case "project":
				return tree;
			case "action":
				return largePottedPlant;
			// ----
			case "equity":
				return tableWithDrawers;
			case "costs":
				return chestOfDrawers;
			case "sales":
				return bookCabinet;
			// ----
			case "agreement":
				return owlSculpture;
			case "decision":
				return headSculpture;
			// ----
			default:
				return tv;
		}
	}


	public void SetCurrentResidentController(ResidentController rsdtController) {
		if (bldgChatController == null) {
			bldgChatController = gameObject.GetComponent<BldgChatController>();
		}
		bldgChatController.SetResidentController(rsdtController);
	}

	void showContextMenu() {
		isShowingContextMenu = true;
		contextMenu.gameObject.SetActive(true);
	}

	void hideContextMenu() {
		isShowingContextMenu = false;
		contextMenu.gameObject.SetActive(false);
	}

	// public void handleFloorClick(Vector3 point) {
	// 	Debug.Log("Floor click at " + point);
	// 	if (isRelocating) {
	// 		completeRelocatingBldg(point);
	// 	} else if (!isShowingContextMenu) {
	// 		// camera.moveToStart();
	// 		contextMenu.gameObject.SetActive(false);
	// 		clickedModel = null;
	// 		clickedObject = null;
	// 	}
	// }


    // public void handleClick(BldgObject bldgObject, Bldg bldgModel, Vector3 position) {

    //     //camera.moveToTarget(position);
	// 	if (clickedModel != bldgModel) {
	// 		Debug.Log("Clicked on different object: " + clickedModel.name);
	// 		clickedObject = bldgObject;
	// 		clickedModel = bldgModel;
	// 		hideContextMenu();
	// 		targetURL = null;
	// 	}
    // }

    // public void handleLongClick(BldgObject bldgObject, Bldg bldgModel, Vector3 position) {
    //     Debug.Log("long click on: " + bldgModel.name);
	// 	//showContextMenu();
    // }

    // public void handleRightClick(Bldg model, Vector3 position) {
    //     Debug.Log("right click on: " + model.name);
    // }

	public void browse() {
		if (targetURL != null) {
			if (!(targetURL.StartsWith("http://") || targetURL.StartsWith("https://"))) {
				targetURL = "https://" + targetURL;
			}
			Debug.Log("Browsing to: " + targetURL);
			Application.OpenURL (targetURL);
			targetURL = null;
		} else if (clickedModel != null && clickedModel.web_url != null) {
			targetURL = clickedModel.web_url;
		}
	}

	public void startRelocating() {
		Debug.Log("Starting to relocate");
		isRelocating = true;
		relocatedObject = clickedObject;
		relocatedObject.indicateBeingRelocated();
		hideContextMenu();
		// camera.moveToStart();
		Cursor.SetCursor(relocateCursorTexture, new Vector2(30, 60), CursorMode.Auto);
	}


	public void closeMenu() {
		hideContextMenu();
	}

	string generateNewAddress(string oldAddress, int newX, int newY) {
		// generate new address based on new coordinates
		Debug.Log("Old address is: " + oldAddress);
		string[] addressParts = oldAddress.Split(AddressUtils.DELIM_CHAR);
		string[] newAddressParts = new string[addressParts.Length];
		for (int i = 0; i < addressParts.Length - 1; i++) {
			newAddressParts[i] = addressParts[i];
		}
		newAddressParts[newAddressParts.Length - 1] = "b(" + newX + "," + newY + ")";
		return string.Join(AddressUtils.DELIM, newAddressParts);
	}

	public void completeRelocatingBldg(Vector3 point) {
		// figure out new coordinates
		int newX = (int)(point.x - floorStartX);
		int newY = (int)(point.z - floorStartZ);
		Debug.Log("Invoking relocate API to move bldg " + clickedModel.name + ", from (" + clickedModel.x + ", " + clickedModel.y + ") to (" + newX + ", " + newY + ")");
		string newAddress = generateNewAddress(clickedModel.address, newX, newY);
		GlobalConfig conf = GlobalConfig.Instance;
		string url = conf.bldgServer + conf.bldgsBasePath + "/" + clickedModel.address + "/relocate_to/" + newAddress;
		Debug.Log("url = " + url);
		// invoke relocate API
		RequestHelper req = RestUtils.createRequest("POST", url);
		RestClient.Post(req).Then(res => {
			Debug.Log("Relocation done");
			isRelocating = false;
			relocatedObject = null;
			clickedModel = null;
			clickedObject = null;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			// fetch all bldgs again
			this.AddressChanged();	// TODO remove, once receiving updates from server
		});
	}

	// public void GoToBldg(string address) {
	// 	currentAddress = address;
	// 	if (AddressUtils.isBldg(address)) {
	// 		currentAddress = AddressUtils.generateInsideAddress (address);
	// 	}
	// 	AddressChanged ();
	// }

	// public void GoIn() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();
	// 	currentAddress = input.text;
	// 	AddressChanged ();
	// }


	// public void GoOut() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();

	// 	currentAddress = AddressUtils.getContainerFlr(input.text);

	// 	AddressChanged ();
	// }


	// public void GoUp() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();
	// 	currentAddress = input.text;
	// 	int level = AddressUtils.getFlrLevel(currentAddress);
	// 	currentAddress = AddressUtils.replaceFlrLevel (currentAddress, level + 1);
	// 	AddressChanged ();
	// }


	// public void GoDown() {
	// 	InputField input = GameObject.FindObjectOfType<InputField> ();
	// 	currentAddress = input.text;
	// 	int level = AddressUtils.getFlrLevel(currentAddress);
	// 	if (level > 0) {
	// 		currentAddress = AddressUtils.replaceFlrLevel (currentAddress, level - 1);
	// 	}
	// 	AddressChanged ();
	// }

	public void SetAddress(string address) {
		Debug.Log("SetAddress -> " + address);
		currentAddress = address;
		AddressChanged();
	}


	public void EnterBuildingByAddress(string address) {
		//clearEverythingBut(address);
		//address = address + AddressUtils.DELIM + "l0";	// TODO add floor only if really needed
		//SetAddress(address);
	}

	public void EnterBuilding(string web_url) {
		Debug.Log("EnterBuilding -> " + web_url);
		// lookup the address for that web_url
		// We can add default request headers for all requests 
        Debug.Log("Resolvin bldg for web_url: " + web_url);
		string address = null;
		GlobalConfig conf = GlobalConfig.Instance;
		string url = conf.bldgServer + conf.bldgsBasePath + "/resolve_address?web_url=" + UnityWebRequest.EscapeURL(web_url);
		Debug.Log(url);
		RequestHelper req = RestUtils.createRequest("GET", url);
		RestClient.Get(req).Then(res =>
			{
				Debug.Log(res);
				address = res.Text;
				Debug.Log("Resolve address was successful: " + address);
				EnterBuildingByAddress(address);
			}).Catch(err => {
				Debug.Log(err.Message);
				Debug.Log("Failed to resolve address for: " + web_url);
			});
	}

	public void AddressChanged() {
		Debug.Log ("Address changed to: " + currentAddress);
		// InputField input = GameObject.FindObjectOfType<InputField> ();
		// if (input.text != currentAddress) {
		// 	input.text = currentAddress;
		// }

		PlayerPrefs.SetString ("currentAddress", currentAddress);

		// TODO validate address

		currentFlr = AddressUtils.extractFlr(currentAddress);

		CurrentMetadata cm = CurrentMetadata.Instance;
		cm.clearEntities();

		// TODO check whether it changed

		// TODO DECIDE WHETHER WE NEED DIFFERENT SCENES FOR G & FLR
		
		// check whether we need to switch scene
		// if (currentAddress.ToLower () == "g" && SceneManager.GetActiveScene().name == "Floor") {
		// 	SceneManager.LoadScene ("Ground");
		// 	return;
		// }
		// if (currentAddress.ToLower () != "g" && SceneManager.GetActiveScene().name == "Ground") {
		// 	SceneManager.LoadScene ("Floor");
		// 	return;
		// }

		// load the new address
		Reload();
	}

	string getCurrentAddressFromResident() {
		CurrentResidentController crc = CurrentResidentController.Instance;
		return crc.resident.flr;
	}


	public void Reload() {
		Debug.Log("Reload invoked");
		switchAddress (getCurrentAddressFromResident());
		if (!isReloadingInLoop) {
			Debug.Log("Starting reload loop");
			isReloadingInLoop = true;
		}
		Invoke("Reload", 5.0f);
	}

	void switchAddress(string address) {
		addressToLocation.Clear();
		addressStack.Clear();
		Debug.Log("Cleared address stack. Size: " + addressStack.Count);
		addressStack.Push("g");
		loadNextBldgInStack();
	}

	void loadNextBldgInStack() {
		while (addressStack.Count > 0) {
			string address = addressStack.Pop();
			Debug.Log("Loading everything in address: " + address);
			reloadBuildings(address);
			reloadResidents(address);
			reloadRoads(address);
		}
	}

	float getCategoryScaleFactor(string category) {
		switch (category) {
			case "xs": return 0.2F;
			case "s": return 0.5F;
			case "m": return 1F;
			case "l": return 1.5F;
			case "xl": return 2F;
			case "2xl": return 3F;
			case "3xl": return 4F;
			default: return 1F;
		}
	}

	void renderModelData(GameObject bldg, Bldg data) {
		Dictionary<string, string> data_attributes = new Dictionary<string, string>() {};
		if (data.data != null) {
			Dictionary<string, string> da = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.data);
			if (da != null)
				data_attributes = da;
			else
				Debug.Log("Failed to load data attributes JSON - got null");
		}

		if (data.category != null) {
			if (SIZE_CATEGORIES.Contains(data.category.ToLower())) {
				float scaleFactor = getCategoryScaleFactor(data.category);
				bldg.transform.localScale *= scaleFactor;
			}
		}
		TMP_Text[] labels = bldg.GetComponentsInChildren<TMP_Text>();
		foreach (TMP_Text label in labels) {
			if (label.name == "summary")
				label.text = data.summary;
			else if (label.name == "summary_top")
				label.text = data.summary;
			else if (label.name == "entity_type")
				label.text = data.entity_type;
			else if (label.name == "name")
				label.text = data.name;
			else if (label.name == "name2")
				label.text = data.name;		
			else if (label.name == "name_top")
				label.text = data.name;
			else if (label.name == "state")
				label.text = data.state;
			else if (data_attributes.ContainsKey(label.name)) {
				label.text = data_attributes[label.name];
			}
			
			if (data_attributes.ContainsKey("flr_height")) {
				float flrHeight = float.Parse(data_attributes["flr_height"]);
				Debug.Log("Setting flr height for " + data.address + " to " + flrHeight);
				addressToFlrHeight[data.address] = flrHeight;
			}

			if (data_attributes.ContainsKey("flr0_height")) {
				float flr0Height = float.Parse(data_attributes["flr0_height"]);
				Debug.Log("Setting flr0 height for " + data.address + " to " + flr0Height);
				addressToFlr0Height[data.address] = flr0Height;
			}
		}
		
		try {
			ImageController[] imageDisplays = bldg.GetComponentsInChildren<ImageController>(true);
			foreach (ImageController imgDisplay in imageDisplays) {
				if (data_attributes.ContainsKey(imgDisplay.imageName)) {
					imgDisplay.gameObject.SetActive(true);
					imgDisplay.SetImageURL(data_attributes[imgDisplay.imageName]);
				} else
					if (imgDisplay.gameObject.active) 
						imgDisplay.SetImageURL(data.picture_url);
			}
		} catch (Exception e) {
			Debug.Log("Failed to render images: " + e.ToString());
		}

		LinkController[] linkObjects = bldg.GetComponentsInChildren<LinkController>();
		foreach (LinkController linkObj in linkObjects) {
			if (data_attributes.ContainsKey(linkObj.linkName))
					linkObj.SetLinkURL(data_attributes[linkObj.linkName]);
				else
					linkObj.SetLinkURL(data.web_url);
		}
		// TODO else, if linkObj name matches a key in data_attributes, take value from there

	}

	void reloadBuildings(string address) {
		CurrentMetadata cm = CurrentMetadata.Instance;
		CurrentResidentController crc = CurrentResidentController.Instance;
		bool dataChanged = false;
		var idsCache = new Dictionary<int, GameObject>();
		var addrCache = new Dictionary<int, string>();
		var lastUpdateCache = new Dictionary<int, string>();
		GameObject[] currentFlrBuildings = GameObject.FindGameObjectsWithTag("Building");
		foreach (GameObject bldg in currentFlrBuildings) {
			BldgObject bObj = bldg.GetComponent<BldgObject>();
			if (!idsCache.ContainsKey(bObj.model.id)) {
				idsCache.Add(bObj.model.id, bldg);
				addrCache.Add(bObj.model.id, bObj.model.address);
				lastUpdateCache.Add(bObj.model.id, bObj.model.updated_at);
			} else {
				Debug.LogWarning("Building rendered twice! " + bObj.model.name);
			}
		}
		// escape the address
		address = Uri.EscapeDataString(address);
		Debug.Log("address escaped to: " + address);
		GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.bldgsBasePath + "/look/" + address;
		// Debug.Log("Loading buildings from: " + url);
		RequestHelper req = RestUtils.createRequest("GET", url);
		RestClient.GetArray<Bldg>(req).Then(res =>
			{
				int count = 0;
				foreach (Bldg b in res) {
					count += 1;
					Debug.Log("processing bldg " + count + ": " + b.name);
					
					// // The area is 16x12, going from (8,6) - (-8,-6)

					bool newBldg = !idsCache.ContainsKey(b.id);
					bool movedBldg = false;
					bool changedBldg = false;
					if (!newBldg) {
						movedBldg = addrCache[b.id] != b.address;
						changedBldg = lastUpdateCache[b.id] != b.updated_at;
					}
					if (!(newBldg || movedBldg || changedBldg)) {
						// don't draw existing or unmoved or unchanged residents
						continue;
					}
					if (movedBldg || changedBldg) {
						GameObject.Destroy (idsCache[b.id]);
					}

					// new entity so add to metadata of entities belonging to current user
					if (Array.IndexOf(b.owners, crc.resident.email) > -1) {
						cm.addEntity(b.entity_type, b.name);
					}
					dataChanged = true;

					if (b.previous_messages.Length > 0) {
						Debug.Log("Found " + b.previous_messages.Length + " previous messages for " + b.name);
						bldgChatController.AddHistoricMessages(b.previous_messages);
					}

					float scaleDiffBetweenPlayerAndBldg = crc.resident.nesting_depth - b.nesting_depth;
					float scaleFactor = (float)Math.Pow(10F, scaleDiffBetweenPlayerAndBldg);


					float height = 0F;
					float flr0Height = 0F;
					//if (address != "g") {
					//	height = 2F;  // bldg is larger when inside a bldg, so floor is higher
					//}
					int flrLevel = AddressUtils.getFlrLevel(b.flr);
					string flrAddress = AddressUtils.getBldg(b.flr);
					Debug.Log("level of " + b.name + " is " + flrLevel);
					Debug.Log("flrAddress of " + b.name + " is " + flrAddress);
					float flrHeight = 0.385F * scaleFactor * 10F;	// flr is part of the container bldg, which is 10x scale
					Debug.Log("Checking flr height for " + flrAddress);
					if (addressToFlrHeight.ContainsKey(flrAddress)) {
						Debug.Log("Found flr height for " + flrAddress + ": " + addressToFlrHeight[flrAddress]);
						flrHeight = addressToFlrHeight[flrAddress] * scaleFactor * 10F;	// flr is part of the container bldg, which is 10x scale
					}
					if (addressToFlr0Height.ContainsKey(flrAddress)) {
						Debug.Log("Found flr0 height for " + flrAddress + ": " + addressToFlr0Height[flrAddress]);
						flr0Height = addressToFlr0Height[flrAddress] * scaleFactor * 10F;	// flr is part of the container bldg, which is 10x scale
					}
					if (flrLevel > 0) {
						height = flrHeight * flrLevel;
					}
					if (flr0Height > 0) {
						height += flr0Height;
					}

					// TODO: if the current bldg is in depth larger than 0, get the location of the container bldg
					float originX = floorStartX;
					float originZ = floorStartZ;

					if (b.nesting_depth > 0) {
						Debug.Log("~~~~ " + b.name + " is in " + b.flr + " and rendered in scale " + scaleFactor);
						string parentContainerAddress = AddressUtils.getBldg(b.flr);
						Vector3 parentLocation = addressToLocation.TryGetValue(parentContainerAddress, out parentLocation) ? parentLocation : new Vector3(0, 0, 0);
						originX = parentLocation.x;
						originZ = parentLocation.z;
						Debug.Log("Setting origin to " + originX + ", " + originZ + " and scale to " + scaleFactor);
					}
					else {
						Debug.Log("Setting origin to " + originX + ", " + originZ);
					}

					// TODO change size based on nesting depth

					Vector3 baseline = new Vector3(originX, height, originZ);	// WHY? if you set the correct Y, some images fail to display
					baseline.x += b.x * scaleFactor;
					baseline.z += b.y * scaleFactor;
					if (b.is_composite) {
						// store the rendered locations of container bldgs 
						addressToLocation.Add(b.address, baseline);
					}
					GameObject prefab = getPrefabByEntityClass(b.entity_type);
					GameObject bldgClone = null;
					try {
						Debug.Log("~~~~~~~ About to instantiate " + b.name + " at " + baseline + " with scale " + scaleFactor + " and prefab " + prefab.name);
						bldgClone = (GameObject) Instantiate(prefab, baseline, Quaternion.identity);
						bldgClone.tag = "Building";
						//if (b.nesting_depth > 0) {
						bldgClone.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
						//}
						BldgObject bldgObject = bldgClone.AddComponent<BldgObject>();
						bldgObject.initialize(b, this);
						renderModelData(bldgClone, b);
					} catch (Exception e) {
						Debug.Log("Failed to instantiate object: " + b.name);
						Debug.LogError(e.ToString());
					}
					
					if (b.is_composite) {
						Debug.Log("Adding " + b.address + " to address stack");
						addressStack.Push(b.address + "/l0");	// TODO need to somehow go over all floors
						addressStack.Push(b.address + "/l1");
						addressStack.Push(b.address + "/l2");
						addressStack.Push(b.address + "/l3");
						addressStack.Push(b.address + "/l4");
						addressStack.Push(b.address + "/l5");
						Debug.Log("Address stack size: " + addressStack.Count);
					}
					
					//Debug.Log("About to call renderAuthorPicture on bldg " + count);
                    // TODO create picture element
					// controller.renderMainPicture();
				}
				if (dataChanged) {
					EventManager.Instance.TriggerEvent("EntitiesChanged");
					if (addressStack.Count > 0) {
						loadNextBldgInStack();
					}
					else {
						if (!isRenderingCompleteEventFired) {
							isRenderingCompleteEventFired = true;
							Debug.Log("~~~~~ Done loading all bldgs - next move player to current location");
							// calculate the location of the current player
							try {
								string playerFlr = crc.resident.flr;
								Debug.Log("~~~~~ Player is in " + playerFlr);
								string playerAddress = AddressUtils.getBldg(playerFlr);
								float playerFlr0Height = addressToFlr0Height[playerAddress] * 10F;	// flr is part of the container bldg, which is 10x scale
								int playerFlrLevel = AddressUtils.getFlrLevel(playerFlr);
								float playerFlrHeight = playerFlr0Height + addressToFlrHeight[playerAddress] * playerFlrLevel * 10F + 0.8F;	// add also the initial resident standing height. TODO fix this
								// TODO this shouldn't be fixed - user may have moved around, signed-off & signed-in again
								Vector3 playerLocation = addressToLocation[playerAddress];
								// set the location of the player on the crc
								crc.currentRenderedPosition = new Vector3(playerLocation.x, playerFlrHeight, playerLocation.z);
								crc.containerRenderedPosition = addressToLocation[playerAddress];
							} catch (Exception e) {
								Debug.Log("Failed to move player to current location: " + e.ToString());
							}
							EventManager.Instance.TriggerEvent("RenderingComplete");
						}
					}
				}
				Debug.Log("Rendered " + count + " bldgs");
			});
	}

	void reloadResidents(string address) {
		CurrentResidentController crc = CurrentResidentController.Instance;
		var idsCache = new Dictionary<int, GameObject>();
		var addrCache = new Dictionary<int, string>();
		var lastUpdateCache = new Dictionary<int, string>();
		GameObject[] currentFlrResidents = GameObject.FindGameObjectsWithTag("Resident");
		int cou = 0;
		foreach (GameObject rsdnt in currentFlrResidents) {
			cou++;
			ResidentController rObj = rsdnt.GetComponent<ResidentController>();
			Debug.Log("~~~~~~~~~~ " + cou + " checking RENDERED resident id: " + rObj.resident.id);
			Debug.Log("~~~~~~~~~~ " + cou + " checking RENDERED resident alias: " + rObj.resident.alias);			
			if (!idsCache.ContainsKey(rObj.resident.id)) {
				Debug.Log("~~~~~~~~~~ " + cou + " not in cache so adding " + rObj.resident.id);
				idsCache.Add(rObj.resident.id, rsdnt);
				addrCache.Add(rObj.resident.id, rObj.resident.location);
				lastUpdateCache.Add(rObj.resident.id, rObj.resident.updated_at);
			} else {
				Debug.LogWarning("Resident rendered twice! " + rObj.resident.name);
			}
		}
		
		// escape the address
		address = Uri.EscapeDataString(address);
		Debug.Log("address escaped to: " + address);
		GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.residentsBasePath + "/look/" + address;
		// Debug.Log("Loading residents from: " + url);
		RequestHelper req = RestUtils.createRequest("GET", url);
		RestClient.GetArray<Resident>(req).Then(result =>
			{
				int count = 0;
				foreach (Resident r in result) {
					count += 1;
					// Debug.Log("processing resident " + count);

					if (r.previous_messages.Length > 0) {
						bldgChatController.AddHistoricMessages(r.previous_messages);
					}

					// if it's the current user, skip
					if (r.alias == crc.resident.alias) {
						Debug.Log("~~~~~~~~~~~~ Skipping rendering current user");
						continue;
					} else {
						Debug.Log("~~~~~~~~~~~~ Rendering resident " + r.alias);
					}

					bool newResident = !idsCache.ContainsKey(r.id);

					bool movedResident = false;
					bool changedResident = false;
					if (!newResident) {
						movedResident = addrCache[r.id] != r.location;
						changedResident = lastUpdateCache[r.id] != r.updated_at;
					}

					if (!(newResident || movedResident || changedResident)) {
						// don't draw existing or unmoved or unchanged residents
						continue;
					}
					if (movedResident || changedResident) {
						GameObject.Destroy (idsCache[r.id]);
					}

					// // The area is 16x12, going from (8,6) - (-8,-6)

					float height = 0.5F;
					if (address != "g") {
						height = 2.5F;  // bldg is larger when inside a bldg, so floor is higher
					}
					Vector3 baseline = new Vector3(floorStartX, height, floorStartZ);	// WHY? if you set the correct Y, some images fail to display
					baseline.x += r.x;
					baseline.z += r.y;
					// Debug.Log("Rendering resident " + r.alias + " at " + baseline.x + ", " + baseline.z);
					Debug.Log("Changing direction of " + r.alias + " to " + r.direction);
					Quaternion qrt = Quaternion.identity;
					qrt.eulerAngles = new Vector3(0, r.direction, 0);
					GameObject rsdtClone = (GameObject) Instantiate(baseResidentObject, baseline, qrt);
					rsdtClone.tag = "Resident";
					Debug.Log("~~~~~~~~~~ rendering resident " + r.id);
                    
					ResidentController rsdtObject = rsdtClone.GetComponent<ResidentController>();
					Debug.Log("~~~~~~~ About to set resident to controller with id " + r.id);
					rsdtObject.initialize(r);
					Debug.Log("~~~~~~~ rsdtController resident id = " + rsdtObject.resident.id);

					// Debug.Log(r.alias);
					//Debug.Log("About to call renderAuthorPicture on bldg " + count);
                    // TODO create picture element
					// controller.renderMainPicture();
				};
				// Debug.Log("Rendered " + count + " residents");
			});
	}

	void reloadRoads(string address) {
		var idsCache = new Dictionary<int, GameObject>();
		GameObject[] currentFlrRoads = GameObject.FindGameObjectsWithTag("Road");
		foreach (GameObject road in currentFlrRoads) {
			RoadObject rObj = road.GetComponentsInChildren<RoadObject>()[0];
			if (!idsCache.ContainsKey(rObj.model.id)) {
				idsCache.Add(rObj.model.id, road);
			}
		}
		// escape the address
		address = Uri.EscapeDataString(address);
		Debug.Log("address escaped to: " + address);
		GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.roadsBasePath + "/look/" + address;
		Debug.Log("Loading roads from: " + url);
		RequestHelper req = RestUtils.createRequest("GET", url);
		RestClient.GetArray<Road>(req).Then(res =>
			{
				Debug.Log("Got response for look roads");
				int count = 0;
				foreach (Road r in res) {
					count += 1;

					bool newRoad = !idsCache.ContainsKey(r.id);
					if (!newRoad) {
						// don't draw existing roads
						// TODO this is just a temporary measure - roads could change & need redraw
						continue;
					}

					renderRoad(address, r, r.from_x, r.from_y, r.to_x, r.to_y);
				}
				Debug.Log("Rendered " + count + " roads");
			});
	}


	void renderRoad(string address, Road r, int from_x, int from_y, int to_x, int to_y)
	{	
		int d_x = 0;
		if (to_x != from_x) {
			d_x = to_x - from_x;
		}
		int d_y = 0;
		if (to_y != from_y) {
			d_y = to_y - from_y;
		}
		
		// if straight line, draw 1 segment
		if (d_x == 0 || d_y == 0) {
			renderRoadSegment(address, r, from_x, from_y, d_x, d_y);
		}
		// else break to 2 segments
		else {
			int mid_x = from_x + d_x;
			int mid_y = from_y;

			if (from_x > to_x) {
				from_x = mid_x;
				d_x = -1 * d_x;
			}

			if (from_y > to_y) {
				mid_y = mid_y + d_y;
				d_y = -1 * d_y;
			}
			renderRoadSegment(address, r, from_x, from_y, d_x, 0);
			renderRoadSegment(address, r, mid_x, mid_y, 0, d_y);
		}
	}

	void renderRoadSegment(string address, Road r, int from_x, int from_y, int d_x, int d_y) 
	{
		float height = 0.01F;
		if (address != "g") {
			height = 2.01F;  // bldg is larger when inside a bldg, so floor is higher
		}
		Vector3 baseline = new Vector3(floorStartX, height, floorStartZ);
		float default_road_scale = 10.01F;
		baseline.x += from_x;
		baseline.z += from_y;
		GameObject roadClone = (GameObject) Instantiate(roadObject, baseline, Quaternion.identity);
		RoadObject roadObj = roadClone.AddComponent<RoadObject>();
		roadObj.initialize(r);
		roadClone.transform.Translate((d_x / 2), 0, (d_y / 2));
		roadClone.transform.localScale += new Vector3(d_x / default_road_scale, 0, d_y / default_road_scale);
		roadClone.tag = "Road";
	}


	GameObject getContainerBldg() {
		GameObject[] found = GameObject.FindGameObjectsWithTag("ContainerBuilding");
		if (found.Length == 0) return null;
		return found[0];
	}


	string removeFlrFromAddress(string address) {
		string[] addressParts = address.Split(AddressUtils.DELIM_CHAR);
		string lastPart = addressParts[addressParts.Length - 1];
		if (lastPart.Substring(0, 1) == "l") {
			string[] newParts = new string[addressParts.Length - 1];
			for (int i = 0; i < addressParts.Length - 1; i++) {
				newParts[i] = addressParts[i];
			}
			return string.Join(AddressUtils.DELIM_CHAR, newParts);
		}
		return address;
	}
	

	public void reloadContainerBldg() {
		// Debug.Log("~~~~~ Reloading container bldg");

		// check whether the container bldg already has a model object
		// Debug.Log("~~~~~ Currently inside scene " + SceneManager.GetActiveScene().name);
		GameObject container = getContainerBldg();
		// Debug.Log("~~~~~ and container bldg is: " + container);
		
		if (container == null) return;
		BldgObject bldgObj = container.GetComponent<BldgObject>();
		// if (bldgObj.model != null && bldgObj.model.address != null && bldgObj.model.address != "") return;

		// Debug.Log("~~~~~~~~~~~ moving on with reload container bldg...");
		// if not: load the data of the container bldg
		// remove floor from address
		string address = removeFlrFromAddress(currentAddress);
		// url encode the address
		string encodedAddress = Uri.EscapeDataString(address);
		// invoke the get bldg API
		GlobalConfig conf = GlobalConfig.Instance;
        string url = conf.bldgServer + conf.bldgsBasePath + "/" + encodedAddress;
		// Debug.Log("~~~~ Loading container bldg model from: " + url);
		RequestHelper req = RestUtils.createRequest("GET", url);
		RestClient.Get<WrappedBldg>(req).Then(res =>
		{
			bldgObj.model = res.data;
			// Debug.Log("~~~~ Loaded container bldg data: " + bldgObj.model.name);
			renderModelData(container, res.data);
			// Debug.Log("~~~~ Done rendering container bldg data.");
		}).Catch(err => {
			Debug.Log(err.Message);
			Debug.Log("Failed to load container bldg model: " + address);
		});
	}

	void updateFloorSign() {
		//TODO ADD FLOOR SIGN TO BLDG PREFAB & uncomment

		//TextMesh flrSign = GameObject.FindGameObjectWithTag ("FloorSign").GetComponent<TextMesh>();
		//flrSign.text = currentFlr.ToUpper ();
	}

}
