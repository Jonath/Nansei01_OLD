using UnityEngine;
using System.Text.RegularExpressions;

// In order of load
public enum EPaperDoll { AKARI_IDLE, AKARI_SURPRISED, AKARI_HAPPY, AKARI_ANNOYED,
                         IIYONA_SURPRISED, IYONA_HAPPY, IYONA_IDLE,
                         MIKO_IDLE, MIKO_HAPPY, MIKO_SURPRISED, MIKO_COCKY, MIKO_ANNOYED, MIKO_ANGRY
                         };

public enum ESpeaker { LEFT, RIGHT }; // Maybe change it for other types if more than 1 adversary

[ExecuteInEditMode]
public class Dialogue : Entity {
    public float margin;
    public float linejump;

    public TextAsset file;
    public Sprite[] paper_dolls;
    private Vector3[] positions;

    public SpriteRenderer left_doll;
    public SpriteRenderer right_doll;
    private Text text;

    // Read form text file
    private ESpeaker[] speaker;
    private EPaperDoll[] emotions;
    private string[] dialogue;
    private int diagidx;

    private string str = "Default text";
    private int nblines = 0;

    public override void Init() {
        base.Init();

		if (obj != null) {
			obj.Type = EType.EFFECT;
			obj.Scale = GameScheduler.ComputeScale (new Vector3 (0.7f, 0.35f));
			//obj.Color = new Color32(255, 255, 255, 150); // TODO : change to 0 for init
			pool.ChangeBulletAppearance (obj, sprite, EMaterial.DIALOGUE);

			Vector3 position = obj.OBB.FL + new Vector3 (margin, -margin);
			text = pool.AddText (str, this, EType.EFFECT, EMaterial.TEXT, ETextStyle.SIMPLE, position);

			paper_dolls = Resources.LoadAll<Sprite> ("PaperDolls");

			// @TODO 1 pos per paper doll ?
			positions = new Vector3[] {
				GameScheduler.ComputePosition (new Vector3 (-190, -110, 0)),
				GameScheduler.ComputePosition (new Vector3 (-10, -82, 0))
			};

			ReadFile ();
			if (diagidx < dialogue.Length) {
				pool.ChangeText (text, dialogue [diagidx]);
			}
			++diagidx;

			left_doll.sprite = paper_dolls [7];
			left_doll.transform.position = positions [0];
			left_doll.transform.localScale = GameScheduler.ComputeScale (new Vector3 (0.3f, 0.3f));

			right_doll.sprite = paper_dolls [0];
			right_doll.transform.position = positions [1];
			right_doll.transform.localScale = GameScheduler.ComputeScale (new Vector3 (0.3f, 0.3f));
		}
    }

    private bool ReadFile() {
        try {
            string str = file.text;
            string[] lines = Regex.Split(str, "\n");
            nblines = lines.Length - 1;

            speaker = new ESpeaker[nblines];
            emotions = new EPaperDoll[nblines];
            dialogue = new string[nblines];

            for (int i = 0; i < lines.Length; ++i) {
                // Separator is %
                string[] entries = lines[i].Split('%');

                // Character%Emotion%Text is the format
                if (entries.Length == 4) {
                    speaker[i] = (ESpeaker)System.Enum.Parse(typeof(ESpeaker), entries[0]);
                    emotions[i] = (EPaperDoll)System.Enum.Parse(typeof(EPaperDoll), entries[1]);
                    dialogue[i] = entries[2];
                }
            }          

            return true;
        } catch {
            Debug.Log("Could not read dialogue file");
            return false;
        }
    }

    // @TODO Appear disappear at Entity level (coro ?)

    /*public void UpdateAt() {
        if (Input.anyKeyDown && diagidx < nblines) {
            pool.ChangeText(text, dialogue[diagidx]);
            // @TODO start animating text
            // @TODO change dialogue background ?

            EPaperDoll emotion = emotions[diagidx];
            EMaterial material = GetMaterialFromEmotion(emotion);

            if (speaker[diagidx] == ESpeaker.LEFT) {
                left_doll = pool.ChangeBulletAppearance(left_doll, paper_dolls[(int)emotion], material);
            } else {
                right_doll = pool.ChangeBulletAppearance(right_doll, paper_dolls[(int)emotion], material);
            }

            // @TODO play sound
            diagidx++;
        }
    */

    private EMaterial GetMaterialFromEmotion(EPaperDoll emotion) {
        string enum_str = emotion.ToString();
        return (EMaterial)(System.Enum.Parse(typeof(EMaterial), enum_str));
    }
}
