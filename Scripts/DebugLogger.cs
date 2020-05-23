using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugLogger : MonoBehaviour
{

    public static DebugLogger current;

    /*
    [System.Serializable]
    public class CustomTextBox {
        public string id;
        public TextMeshProUGUI textbox;
        public List<string> lines;
        private int m_maxLines;
        public int maxLines {
            get {   return m_maxLines;  }
            set {   m_maxLines = value; }
        }
        public void AddLine(string s) {
            if (lines.Count > m_maxLines) {  lines.RemoveAt(0);  }
            lines.Add(s);
            string toPrint = "";
            for (int i = 0; i < lines.Count; i++) {
                if (i == lines.Count-1) toPrint = toPrint + lines[i];
                else toPrint = toPrint + lines[i] + "\n";
            }
            textbox.text = toPrint;
        }
    }
    */

    [SerializeField] [Tooltip("Max number of lines in the debugger")]
    private int m_maxLines = 10;
    public int maxLines {
        get {   return m_maxLines;  }
        set {   m_maxLines = value; }
    }
    [SerializeField] [Tooltip("Font Size")]
    private float m_fontSize = 24f;
    public float fontSize {
        get {   return m_fontSize;  }
        set {   m_fontSize = value; }
    }
    [SerializeField] [Tooltip("Textbox used for Debugging")]
    private TextMeshProUGUI m_defaultTextbox;
    // NOT SERIALIZED [Tooltip("Reference to the canvas group of the debug canvas")]
    private CanvasGroup m_canvasGroup;

    /*
    [SerializeField] [Tooltip("all text boxes who should be updated")]
    private List<CustomTextBox> m_textBoxes = new List<CustomTextBox>();
    */
    // NOT SERIALIZED
    private List<string> m_lines = new List<string>();
    // NOT SERIALIZED
    /*
    private Dictionary<string, List<CustomTextBox>> m_textBoxDictionary = new Dictionary<string, List<CustomTextBox>>();
    */
    [SerializeField] [Tooltip("List of all gameObjects listed under the debugger")]
    private List<GameObject> m_debugObjects = new List<GameObject>();

    [SerializeField] [Tooltip("Set Active Status")]
    private bool m_activeStatus = false;
    public bool activeStatus {
        get {   return m_activeStatus;  }
        set {   SetStatus(value);       }
    }

    private void Awake() {
        current = this;
        m_canvasGroup = this.GetComponent<CanvasGroup>();
    }
    private void Start() {
        SetStatus(m_activeStatus);
    }
    private void Update() {
        if (m_defaultTextbox == null) return;

        m_defaultTextbox.fontSize = m_fontSize;
        
        if (m_lines.Count == 0) return;
        string toPrint = "";
        for (int i = 0; i < m_lines.Count; i++) {
            if (i == m_lines.Count-1) toPrint = toPrint + m_lines[i];
            else toPrint = toPrint + m_lines[i] + "\n";
        }
        m_defaultTextbox.text = toPrint;
    }
    public void AddLine(string s) {
        if (m_lines.Count > m_maxLines) m_lines.RemoveAt(0);
        m_lines.Add(s);
        return;
    }

    public void SetStatus(bool en) {
        m_activeStatus = en;
        m_canvasGroup.alpha = (en) ? 1 : 0;
        //m_defaultTextbox.gameObject.SetActive(en);
        foreach(GameObject g in m_debugObjects) {
            g.SetActive(en);
        }
    }

}
