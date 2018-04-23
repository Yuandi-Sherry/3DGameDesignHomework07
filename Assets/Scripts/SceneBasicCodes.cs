using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneBasicCodes {
	
	public interface IUserAction {
		void getGameStart();
		void getGameOver();
		int getGameState();
		void setGameState(int gameState);
		int getScore();
		void hit(Vector3 pos);
		int getRound();
	}

	public interface ISceneController {
	}

	//导演单例模式
	public class Director : System.Object {
		public ISceneController currentSceneController {get; set; }
		private static Director director;
		public static Director getInstance() {
			if(director == null) {
				director = new Director();
			}
			return director;
		}
	}
	//customizing disk
	public class DiskData : MonoBehaviour {//debug: 这里只有变为Monobehavior才能作为组件添加到gameObject上面
		public int size;
		public Color color;
		public float xSpeed;
		public float ySpeed;
		public Vector3 force;
	}

	//Score Managing
	public class ScoreRecorder : MonoBehaviour {
		public int score;
		private Dictionary<Color, int> colorTable = new Dictionary<Color, int> ();
		private Dictionary<int, int> sizeTable = new Dictionary<int, int> ();

		void Start () {
			score = 0;
			colorTable.Add(Color.yellow, 1);
			colorTable.Add(Color.red, 2);
			colorTable.Add(Color.blue, 3);
			sizeTable.Add(1, 2);
			sizeTable.Add(2, 4);
			sizeTable.Add(3, 6);
		}

		public void Record (GameObject disk) {
			score += colorTable[disk.GetComponent<DiskData>().color] + sizeTable[disk.GetComponent<DiskData>().size];
		}

		public void reset() {
			score = 0;
		}
	}
}