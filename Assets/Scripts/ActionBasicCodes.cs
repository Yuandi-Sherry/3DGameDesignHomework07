using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneBasicCodes;

namespace ActionBasicCodes {
	public enum SSActionEventType:int { Started, Competeted }  

	public interface ISSActionCallback {
		void SSActionEvent(SSAction source,  
			SSActionEventType events = SSActionEventType.Competeted,  
			int intParam = 0,  
			string strParam = null,  
			Object objectParam = null);
	}


	public abstract class SSAction : ScriptableObject {
		public bool enable = false;
		public bool destroy = false;
		public GameObject gameobject { get; set; }
		public Transform transform {get; set; }
		public ISSActionCallback callback {get; set; }

		protected SSAction() { }

		public virtual void Start () {
			throw new System.NotImplementedException();  
		}
		public virtual void Update () {
			throw new System.NotImplementedException();  
		}  
		/**************************************
		 * new codes *
		 **************************************/
		 public virtual void FixedUpdate() {
		 	throw new System.NotImplementedException(); 
		 }

		/**************************************
		 * new codes *
		 **************************************/
		public void reset () {
			enable = false;
			destroy = false;
			gameobject = null;
			transform = null;
			callback = null;
		}
	}

	
	public abstract class SSActionManager: MonoBehaviour {
		protected Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
		protected List<SSAction> waitingAdd = new List<SSAction>();
		protected List<int> waitingDelete = new List<int>();

		public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager) {
			action.gameobject = gameobject;
			action.callback = manager;
			waitingAdd.Add(action);
			action.Start();
		}

		protected void Start() { }
	}

	/**************************************
	 * new codes *
	 **************************************/
	public interface IActionManager {
		void startThrow(Queue <GameObject> diskQueue);
	}
	/**************************************
	 * new codes *
	 **************************************/


}