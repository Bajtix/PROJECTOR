using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Manager : MonoBehaviour {
    public static Manager Instance;

    private void Awake() {
        Instance = this;
    }


    [SerializeField] private VideoPlayer m_player;
    [SerializeField] private Canvas m_canvas;
    [SerializeField] private string m_path;

    private Projector m_projector;
    private VideoScene m_currentScene;
    private VideoClip m_currentClip;
    private string m_homepath;

    private Queue<string> m_queue;
    private Stack<string> m_history;

    private void Start() {
        if(Environment.GetCommandLineArgs().Length > 1 && !Application.isEditor) {
            Load(Environment.GetCommandLineArgs()[1]);
        } else {
            Load(m_path);
        }

        PlayScene("start");
    }

    public void Load(string path) {
        m_projector = JsonConvert.DeserializeObject<Projector>(File.ReadAllText(path));
        m_homepath = Path.GetDirectoryName(path);

        Debug.Log("Loaded project in " + m_homepath);
        Debug.Log($"Starting {m_projector.Name} by {m_projector.Author} (with {m_projector.ClipCount} clips & {m_projector.SceneCount} scenes)...");

        m_history = new Stack<string>();
        m_queue = new Queue<string>();

        int desiredWidth = 1280, desiredHeight = 720;
        bool fullscreen = false;
        
        Debug.Log($"Used mouse mode: {m_projector.MouseMode}");

        try {
            desiredWidth = int.Parse(m_projector.Size.Split('x')[0]);
            desiredHeight = int.Parse(m_projector.Size.Split('x')[1]);
            fullscreen = bool.Parse(m_projector.Size.Split('x')[2]);
            if(m_projector.MouseMode == MouseMode.Hidden || m_projector.MouseMode == MouseMode.Laser) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            } else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            Screen.SetResolution(desiredWidth, desiredHeight, fullscreen);
        } catch(Exception e) {
            Debug.LogError("Failed to set resolution");
            Debug.LogError(e);
        }
        m_canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(desiredWidth,desiredHeight);
        m_player.targetTexture = new RenderTexture(desiredWidth, desiredHeight, 24);

        m_canvas.GetComponentInChildren<Image>().material.SetTexture("_MainTex", m_player.targetTexture);
    }

    private string GetAbsolutePath(string p) {
        if (Path.IsPathRooted(p)) return p;
        return Path.Combine(m_homepath, p);
    }

    public void PlayScene(string name, bool includeInHistory = true) {
        Debug.Log("Playing " + name);
        
        var scene = m_projector.GetScene(name);
        var clip = m_projector.GetClip(scene?.Clip);

        m_player.Stop();

        if (scene == null || clip == null) return;

        m_player.url = GetAbsolutePath(clip.Path);
        m_player.Play();
  
        m_currentClip = clip;
        m_currentScene = scene;

        // dont push twice
        if (includeInHistory && (m_history.Count == 0 || m_history.Peek() != name)) m_history.Push(name);
    }

    public bool IsWithinSafeMargin => m_player.length > 0.05 && m_player.time >= m_player.length - 0.05;

    public void ExecuteTransitionString(string s) {
        if (string.IsNullOrEmpty(s)) return;
        if(s.StartsWith('#')) {
            m_queue.Enqueue(s.Substring(1));
        } else {
            PlayScene(s);
        }
    }

    private void LateUpdate() {
        if(IsWithinSafeMargin) {
            if(m_queue.Count > 0) 
                PlayScene(m_queue.Dequeue());
             else if(m_currentScene.Transitions.ContainsKey("auto"))
                PlayScene(m_currentScene.Transitions["auto"]);
        }

        //check for transitions

        var values = (int[])Enum.GetValues(typeof(KeyCode));
        for (int i = 0; i < values.Length; i++) {
            var keyCode = ((KeyCode)values[i]);
            var pressed = Input.GetKeyDown((KeyCode)values[i]);
            if (pressed) { 
                var scr = m_currentScene.GetTransitionForKey(keyCode);
                if (scr == "") continue;
                PlayScene(scr);
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && m_currentScene.Transitions.ContainsKey("next")) {
            ExecuteTransitionString(m_currentScene.Transitions["next"]);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && m_history.Count > 1) {
            m_history.Pop();
            PlayScene(m_history.Pop());
        }
    }
}
