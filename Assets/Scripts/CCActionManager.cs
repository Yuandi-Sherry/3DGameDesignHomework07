using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneBasicCodes;
using ActionBasicCodes;

	public class CCActionManager : SSActionManager, ISSActionCallback, IActionManager {
		public FirstController sceneController;
		protected new void Start() {
			sceneController = (FirstController)Director.getInstance().currentSceneController;
			sceneController.actionManager = this;
		}

		private List<SSAction> used = new List<SSAction>();
		private List<SSAction> free = new List<SSAction>();
		//利用一个类似于动作工厂借出回收每一个动作
		//从工厂中获得动作
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
		/**
		*游戏开始时，
		*1. 将飞碟队列中的每一个飞碟从动作工厂中拿到一个动作
		*2. 用SSActionManager中RunAction让每个动作得以执行同时调用callback
		*/
		public void startThrow(Queue <GameObject> diskQueue) {
			foreach (GameObject temp in diskQueue) {
				RunAction(temp, GetSSAction(), this);
			}
		}
		// implment interface
		/**
		*当一个飞碟飞起落下的全部动作完成之后
		*1. 需要做的动作总数减少
		*2. 飞碟工厂回收一个飞碟
		*3. 动作管理者回收一个动作
		*/
		public void SSActionEvent(SSAction source,  
			SSActionEventType events = SSActionEventType.Competeted,  
			int intParam = 0,  
			string strParam = null,  
			Object objectParam = null) {
			if(source is CCFlyAction) {
				
				DiskFactory df = Singleton<DiskFactory>.Instance;  
	            df.FreeDisk(source.gameobject);  //可以用这种方式获取了被挂在了特定component的游戏对象
	            FreeSSAction(source);
			}
		}

		private void Update() {
			Debug.Log("Update in SSAM");
			foreach (SSAction ac in waitingAdd) 
				actions[ac.GetInstanceID()] = ac;
			waitingAdd.Clear();

			foreach(KeyValuePair <int, SSAction> kv in actions) {
				SSAction ac = kv.Value;
				if(ac.destroy) {
					waitingDelete.Add(ac.GetInstanceID());
				}
				else if (ac.enable) {
					ac.Update();
				}
			}

			foreach(int key in waitingDelete) {
				SSAction ac = actions[key];
				actions.Remove(key);
				DestroyObject(ac);
			}

			waitingDelete.Clear();
		}
	}