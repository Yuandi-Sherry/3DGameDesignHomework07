using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionBasicCodes;
using SceneBasicCodes;

public class PhysicsManager : SSActionManager, ISSActionCallback, IActionManager {
	public FirstController sceneController;

	protected void Start () {
		sceneController = (FirstController)Director.getInstance().currentSceneController;
		sceneController.actionManager = this;
	}
	
	private List<SSAction> used = new List<SSAction>();
	private List<SSAction> free = new List<SSAction>();

	SSAction GetSSAction() {
		SSAction action = null;
		if(free.Count > 0) {
			action = free[0];
			free.Remove(free[0]);
		}
		else {
			action = ScriptableObject.Instantiate<CCFlyAction>(CCFlyAction.getSSAction());
		}
		used.Add(action);
		return action;
	}

	public void FreeSSAction (SSAction action) {//回收used中的动作，重新放入free
		SSAction temp = null;
		foreach(SSAction i in used) {
			if(i.gameobject.GetInstanceID() == action.GetInstanceID()) {
				temp = i;
			}
		}
		if(temp != null) {
			temp.reset();
			free.Add(temp);
			used.Remove(temp);
		}
	}
	
	protected new void FixedUpdate () {
		foreach (SSAction ac in waitingAdd) 
				actions[ac.GetInstanceID()] = ac;
			waitingAdd.Clear();

			foreach(KeyValuePair <int, SSAction> kv in actions) {
				SSAction ac = kv.Value;
				if(ac.destroy) {
					waitingDelete.Add(ac.GetInstanceID());
				}
				else if (ac.enable) {
					ac.FixedUpdate();
				}
			}

			foreach(int key in waitingDelete) {
				SSAction ac = actions[key];
				actions.Remove(key);
				DestroyObject(ac);
			}

			waitingDelete.Clear();
	}

	public void startThrow(Queue <GameObject> diskQueue) {
		foreach (GameObject temp in diskQueue) {
			RunAction(temp, GetSSAction(), this);
		}
	}

	public void SSActionEvent(SSAction source,  
		SSActionEventType events = SSActionEventType.Competeted,  
		int intParam = 0,  
		string strParam = null,  
		Object objectParam = null) {
		if(source is CCFlyAction) {
			sceneController.numberOfDisksThrown++;
			DiskFactory df = Singleton<DiskFactory>.Instance;  
	        df.FreeDisk(source.gameobject);  //可以用这种方式获取了被挂在了特定component的游戏对象
	        FreeSSAction(source);
		}
	}
}
