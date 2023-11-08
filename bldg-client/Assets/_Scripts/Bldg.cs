using System;
using System.Collections.Generic;

namespace Models
{
	public enum MovementType
	{
		WALK,
		FLY_LOW,
		FLY_HIGH
	}

	[Serializable]
	public class WrappedBldg
	{
		public Bldg data;
	}


	[Serializable]
	public class Bldg
	{
		public int id;
		public int x;
		public int y;
		public string bldg_url;
		public string web_url;
		public string summary;
		public bool is_composite;
		public string flr;
		public string flr_url;
		public int flr_level;
		public int nesting_depth;
		public string entity_type;
		public string name;
		public string state;
		public string category;
		public string[] tags;
		public string address;
		public string picture_url;
		public string[] owners;
		public string[] previous_messages;
		public string updated_at;
		public string data;

		public override string ToString(){
			return UnityEngine.JsonUtility.ToJson (this, true);
		}
	}

	[Serializable]
	public struct ResidentAttributes
	{

	}

	[Serializable]
	public struct Resident
	{
		public string alias;
		public int direction;
		public string email;
		public string home_bldg;
		public int id;
		public string text;
		public bool is_online;
		public DateTime last_login_at;
		public string location;
		public string name;
		public ResidentAttributes other_attributes;
		public string[] previous_messages;
		public string session_id;
		public int x;
		public int y;
		public string flr;
		public string flr_url;
		public string updated_at;
		public int nesting_depth;
	}

	[Serializable]
	public struct Road 
	{
		public int id;
		public string flr;
		public string from_address;
		public int from_x;
		public int from_y;
		
		public string to_address;
		public int to_x;
		public int to_y;
		public string[] owners;
	}

	[Serializable]
	public struct LoginRequest
	{
		public string email;
	}

	[Serializable]
	public struct LoginResponse
	{
		public Resident data;	// containing just the email & session_id attributes
	}

	[Serializable]
	public struct VerificationRequest
	{
		public string email;
		public string session_id;
	}

	
	[Serializable]
	public struct ActionResponse
	{
		public Resident data;
	}

	[Serializable]
	public struct MoveAction
	{
		public string resident_email;
		public string action_type;
		public string move_location;
		public int move_x;
		public int move_y;
	}


	[Serializable]
	public struct EnterBldgAction
	{
		public string resident_email;
		public string action_type;
		public string bldg_address;
		public string bldg_url;
	}

	[Serializable]
	public struct EnterBldgFlrAction
	{
		public string resident_email;
		public string action_type;
		public string bldg_address;
		public string bldg_url;
		public int flr_level;
	}

	[Serializable]
	public struct ExitBldgAction
	{
		public string resident_email;
		public string action_type;
		public string bldg_address;
		public string bldg_url;
	}

	[Serializable]
	public struct TurnAction
	{
		public string resident_email;
		public string action_type;
		public int turn_direction;
	}

	[Serializable]
	public struct SayAction
	{
		public string resident_email;
		public string bldg_url;
		public string action_type;
		public string say_speaker;
		public string say_text;
		public long say_time;
		public string say_flr;
		public string say_flr_url;
		public string say_location;
		public string say_mimetype;
		public string say_recipient;
	}

}

