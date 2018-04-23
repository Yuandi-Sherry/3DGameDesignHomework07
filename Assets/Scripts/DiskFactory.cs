using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneBasicCodes;

public class DiskFactory : MonoBehaviour {
	public GameObject disk;
	//存放已借出飞碟，active = true
	private List<DiskData> used = new List<DiskData>();
	//存放可借出飞碟，active = false
	private List<DiskData> free = new List<DiskData>();

	private void Awake() {
		//加载预制
		disk = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UFO"), Vector3.zero, Quaternion.identity);
		disk.AddComponent<DiskData>();
		disk.SetActive(false);
	}

	public GameObject getDisk(int gameRound) {//debug：飞碟出厂的时候active为false，外界扔了它才能active
		GameObject newDisk = null;
		//如果还有飞碟可以借出
		if(free.Count > 0) {
			newDisk = free[0].gameObject;//借出第一个飞碟
			free.Remove(free[0]);//借出飞碟不再在可借出链表中
		} 
		//如果没有飞碟可借出
		else {
			//从预制重新实例化一个飞碟
			newDisk = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UFO"), Vector3.zero, Quaternion.identity);
			newDisk.SetActive(false);
			newDisk.AddComponent<DiskData>();//挂载记录飞碟属性的DiskData类
		}

		/**游戏规则
		*每一回合direction随机，且均出现红黄蓝三种颜色的飞碟
		*第一回合：慢速、大飞碟
		*第二回合：慢速、小飞碟
		*第三回合：中速、大飞碟
		*第四回合：中速、小飞碟
		*第五回合：慢速、大飞碟
		*第六回合：中速、小飞碟
		*/

		int colorNo = Random.Range(0, 3)%3;
		switch(colorNo) {
			case 0:
				newDisk.GetComponent<DiskData>().color = Color.yellow;
				newDisk.GetComponent<Renderer>().material.color = Color.yellow;  
				break;
			case 1:
				newDisk.GetComponent<DiskData>().color = Color.red;
				newDisk.GetComponent<Renderer>().material.color = Color.red;  
				break;
			case 2:
				newDisk.GetComponent<DiskData>().color = Color.blue;
				newDisk.GetComponent<Renderer>().material.color = Color.blue;  
				break;
		}

		int sizeNo = Random.Range(0, 3)%3;
		switch(sizeNo) {
			case 1:
				newDisk.transform.localScale = new Vector3(1.2f,0.05f,1.2f);
				newDisk.GetComponent<DiskData>().size = 1;
				break;
			case 2:
				newDisk.transform.localScale = new Vector3(2.4f,0.1f,2.4f);
				newDisk.GetComponent<DiskData>().size = 2;
				break;
			case 0:
				newDisk.transform.localScale = new Vector3(3.6f,0.15f,3.6f);
				newDisk.GetComponent<DiskData>().size = 3;
				break;
		}
		newDisk.GetComponent<DiskData>().xSpeed =  getRand() * Random.Range( 1.5f*(gameRound -1),  1.5f*gameRound);
		newDisk.GetComponent<DiskData>().ySpeed =  getRand() * Random.Range(2f + (gameRound-1) * 0.5f, 2f + gameRound * 0.5f);
		newDisk.GetComponent<DiskData>().force = new Vector3(Random.Range(-gameRound,gameRound), getRand()* Random.Range(0.5f,gameRound*0.8f), Random.Range(-gameRound,gameRound));
		used.Add(newDisk.GetComponent<DiskData>());
		newDisk.name = newDisk.GetInstanceID().ToString();
		return newDisk;
	}

	private int getRand() {
		float tempRand = Random.Range(-1f,1f);
		if(tempRand > 0) {
			return 1;
		}
		else {
			return -1;
		}
	}
	
	public void FreeDisk(GameObject disk) {
		DiskData temp = null;//用于在工厂中标记disk所对应的
		foreach (DiskData i in used) {//遍历被用过的飞碟
			//如果其中有参数disk，可以理解为从飞碟工厂生产出飞碟开始，ID就一直不变，不管借出还是回收，永远是那固定的几个
			if(i.gameObject.GetInstanceID() == disk.GetInstanceID()) {
				temp = i;
			}
		}
		if(temp != null) {//else在已借出链表中不存在，说明已经在可借出free链表了，就不用管
			//放到free链表
			temp.gameObject.SetActive(false);
			used.Remove(temp);
			free.Add(temp);
		}
	}
}
