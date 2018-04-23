using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SceneBasicCodes;
using ActionBasicCodes;

public class FirstController : MonoBehaviour, IUserAction, ISceneController {
	public IActionManager actionManager {get; set;}
	public ScoreRecorder scoreRecorder {get; set;}
	private Queue<GameObject> disks = new Queue<GameObject> ();
	private int gameRound = 0;
	private int numberOfDisks; //每轮有多少个飞碟
	public int numberOfDisksThrown = 0;
	public float throwPeriod = 1; //间隔多少秒出现一个飞碟
	public int totalRound = 10;//一共有多少轮
	private float timeInterval = 0;
	private int gameState = 1;//0->not in game, 1->in game
	private UserGUI userGUI;
	// Use this for initialization
	void Awake () {
		Director director = Director.getInstance();
		director.currentSceneController = this;
		numberOfDisks = 10;
		this.gameObject.AddComponent<ScoreRecorder>();
		this.gameObject.AddComponent<DiskFactory>();
		userGUI = this.gameObject.AddComponent<UserGUI>();
		scoreRecorder = Singleton<ScoreRecorder>.Instance;
	}
	
	// Update is called once per frame
	void Update () {//manage game state
		/**
		*游戏进入下一轮，增加轮数，同时重置飞碟数重置，分数不重置
		*/
		Debug.Log("ROUND " + gameRound+ "   DIskThrow" + numberOfDisksThrown);
		if(numberOfDisksThrown == numberOfDisks && gameState == 1) {
			
			gameRound = (gameRound + 1) % (totalRound + 1);
			throwPeriod = (totalRound + 1 - gameRound)/2;
			//从游戏工厂中获得一定数目的飞碟放入飞碟队列
			DiskFactory df = Singleton<DiskFactory>.Instance;  
	        for (int i = 0; i < numberOfDisks; i++)  
	        {  
	            disks.Enqueue(df.getDisk(gameRound));  
	        }  
	        
	        //将每个动作挂在到disks链表的每个元素上面
	        actionManager.startThrow(disks);  
	        numberOfDisksThrown = 0;
		}

		if(gameRound == 0) {//初始化
			gameState = 0;
			numberOfDisksThrown = numberOfDisks;
		}

		/**
		*判断飞碟扔出的时间间隔
		*/
		if(gameState == 1) {
			if(timeInterval > throwPeriod) {
				randomThrow();
				timeInterval = 0;
			}
			else {
				timeInterval += Time.deltaTime;
			}
		}
	}

	/**
	*随机扔出飞碟的位置
	*/
	void randomThrow() {
		if(disks.Count != 0) {
			GameObject disk = disks.Dequeue();
			Vector3 randPos = Vector3.zero;
			float x = Random.Range(-0.5f * gameRound - 3f, 0.5f * gameRound + 3f);
			float y = Random.Range( - 0.1f * gameRound, 0.8f * gameRound);
			//Debug.Log("randPosX: " + x + "   randPosY: " + y);
			randPos = new Vector3(x, y, 0);
			disk.transform.position = randPos;
			disk.SetActive(true);
		}
	}

	//implementation of interfaces

	public int getRound() {
		return gameRound;
	}

	public void getGameStart() {
		this.gameState = 1;
		scoreRecorder.reset();
	}

	public void getGameOver() {
		this.gameState = 0;

	}
	public int getGameState() {
		return gameState;
	}

	public void setGameState(int gameState) {
		this.gameState = gameState;
	}
	public int getScore() {
		return scoreRecorder.score;
	}
	//鼠标打击飞碟
	public void hit(Vector3 pos) {
		Ray ray = Camera.main.ScreenPointToRay(pos);
		RaycastHit[] hits;
		hits = Physics.RaycastAll(ray);
		for(int i = 0; i < hits.Length; i++) {
			RaycastHit hit = hits[i];
			if(hit.collider.gameObject.GetComponent<DiskData>() != null) {
				scoreRecorder.Record(hit.collider.gameObject);
				hit.collider.gameObject.transform.position = new Vector3(0,-20,0);
			}
		}
	}
}
